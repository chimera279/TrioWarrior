using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeneratePerlinWorld : MonoBehaviour
{

    [SerializeField]
    private GameObject blockPrefab;//use a unit cube (1x1x1 like unity's default cube)

    [SerializeField]
    private int chunkSize = 50;


    [SerializeField]
    private int seed = 50;

    [SerializeField]
    private float noiseScale = .05f;

    [SerializeField, Range(0, 1)]
    private float threshold = .5f;

    [SerializeField]
    private Material material;

    [SerializeField]
    private bool sphere = false;

    [SerializeField]
    private bool cube = false;

    [SerializeField]
    private bool rhombus = false;

    private List<Mesh> meshes = new List<Mesh>();//used to avoid memory issues
    List<GameObject> meshObjects = new List<GameObject>();
    public Terrain terr;

    private void Start()
    {
        terr = transform.parent.parent.GetComponent<Terrain>();
    }

    public void Generate()
    {
        float startTime = Time.realtimeSinceStartup;

        #region Create Mesh Data

        List<CombineInstance> blockData = new List<CombineInstance>();//this will contain the data for the final mesh
        MeshFilter blockMesh = Instantiate(blockPrefab, Vector3.zero, Quaternion.identity).GetComponentInChildren<MeshFilter>();//create a unit cube and store the mesh from it

        //go through each block position
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {

                    float noiseValue = Perlin3D(x * noiseScale, y * noiseScale, z * noiseScale, seed);//get value of the noise at given x, y, and z.
                    if (noiseValue >= threshold)
                    {//is noise value above the threshold for placing a block?

                        //ignore this block if it's a sphere and it's outside of the radius (ex: in the corner of the chunk, outside of the sphere)
                        //distance between the current point with the center point. if it's larger than the radius, then it's not inside the sphere.
                        float radius = chunkSize / 2;
                        if (sphere && Vector3.Distance(new Vector3(x, y, z), Vector3.one * radius) > radius)
                            continue;
                        if (cube && Vector3.Distance(new Vector3(x, y, z), Vector3.one * radius) < radius * 0.6f)
                            continue;
                        if (rhombus && Vector3.Distance(new Vector3(x, y, z), Vector3.one * radius) > radius * 0.8f && Vector3.Distance(new Vector3(x, y, z), Vector3.one * radius) < radius)
                            continue;

                        blockMesh.transform.position = new Vector3(x, y, z);//move the unit cube to the intended position
                        CombineInstance ci = new CombineInstance
                        {//copy the data off of the unit cube
                            mesh = blockMesh.sharedMesh,
                            transform = blockMesh.transform.localToWorldMatrix,
                        };
                        blockData.Add(ci);//add the data to the list
                    }

                }
            }
        }

        Destroy(blockMesh.gameObject);//original unit cube is no longer needed. we copied all the data we need to the block list.

        #endregion

        #region Separate Mesh Data

        //divide meshes into groups of 65536 vertices. Meshes can only have 65536 vertices so we need to divide them up into multiple block lists.

        List<List<CombineInstance>> blockDataLists = new List<List<CombineInstance>>();//we will store the meshes in a list of lists. each sub-list will contain the data for one mesh. same data as blockData, different format.
        int vertexCount = 0;
        blockDataLists.Add(new List<CombineInstance>());//initial list of mesh data
        for (int i = 0; i < blockData.Count; i++)
        {//go through each element in the previous list and add it to the new list.
            vertexCount += blockData[i].mesh.vertexCount;//keep track of total vertices
            if (vertexCount > 65536)
            {//if the list has reached it's capacity. if total vertex count is more then 65536, reset counter and start adding them to a new list.
                vertexCount = 0;
                blockDataLists.Add(new List<CombineInstance>());
                i--;
            }
            else
            {//if the list hasn't yet reached it's capacity. safe to add another block data to this list 
                blockDataLists.Last().Add(blockData[i]);//the newest list will always be the last one added
            }
        }

        #endregion

        #region Create Mesh

        //the creation of the final mesh from the data.

        Transform container = new GameObject("Meshys").transform;//create container object
        container.transform.position = new Vector3(Random.Range(0, 1024), Random.Range(50, 500), Random.Range(0, 1024));
        foreach (List<CombineInstance> data in blockDataLists)
        {//for each list (of block data) in the list (of other lists)
            GameObject g = new GameObject("Meshy");//create gameobject for the mesh
            g.transform.parent = container;//set parent to the container we just made
            MeshFilter mf = g.AddComponent<MeshFilter>();//add mesh component
            MeshRenderer mr = g.AddComponent<MeshRenderer>();//add mesh renderer component
            MeshCollider mc = g.AddComponent<MeshCollider>();//add mesh collider component
            mr.material = material;//set material to avoid evil pinkness of missing texture
            mf.mesh.CombineMeshes(data.ToArray());//set mesh to the combination of all of the blocks in the list
            mc.sharedMesh = mf.mesh;//derive colllider from the combined mesh 
            meshes.Add(mf.mesh);//keep track of mesh so we can destroy it when it's no longer needed
            //g.AddComponent<MeshCollider>().sharedMesh = mf.sharedMesh;//setting colliders takes more time. disabled for testing.
            meshObjects.Add(g);
        }

        PositionOnTerrain();

        #endregion

        Debug.Log("Loaded in " + (Time.realtimeSinceStartup - startTime) + " Seconds.");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Destroy(GameObject.Find("Meshys"));//destroy parent gameobject as well as children.
            foreach (Mesh m in meshes)//meshes still exist even though they aren't in the scene anymore. destroy them so they don't take up memory.
                Destroy(m);
            Generate();
        }
    }

    void PositionOnTerrain()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        foreach(var g in meshObjects)
        { 
        g.transform.localPosition = new Vector3(Random.Range(player.position.x - SpawnBugs.SpawnBox.x * 3, player.position.x + SpawnBugs.SpawnBox.x * 3),
                   Random.Range(player.position.y - SpawnBugs.SpawnBox.y * 3, player.position.y + SpawnBugs.SpawnBox.y * 3),
                   Random.Range(player.position.z - SpawnBugs.SpawnBox.z * 3, player.position.z + SpawnBugs.SpawnBox.z * 3));//(Random.Range(100f, 7000f), Random.Range(270f, 1700f), Random.Range(100f, 7000f));
            g.transform.localScale = new Vector3(2.7f, 2.7f, 2.7f) * Random.Range(0.5f,2f);
        g.transform.localRotation = Quaternion.Euler(Random.Range(0, 90), Random.Range(0, 90), Random.Range(0, 90));

            Vector3 tempCoord = g.transform.position - terr.gameObject.transform.position;
            Vector3 terrainCoord;
            terrainCoord.x = tempCoord.x / terr.terrainData.size.x;
            terrainCoord.y = tempCoord.y / terr.terrainData.size.y;
            terrainCoord.z = tempCoord.z / terr.terrainData.size.z;

            int X = (int)(terrainCoord.x * terr.terrainData.heightmapWidth);
            int Y = (int)(terrainCoord.z * terr.terrainData.heightmapHeight);
            float height = terr.terrainData.GetHeights(X, Y,1,1)[0,0];
            Vector3 newHeightPos = g.transform.position;
            newHeightPos.y = height * terr.terrainData.size.y;
            g.transform.position = newHeightPos;

            //  g.transform.localPosition = Vector3.zero +  Random.onUnitSphere * 5;
            //  g.transform.localScale = new Vector3(2, 2, 2);
        }

    }


    //dunno how this works. copied it from somewhere.
    public static float Perlin3D(float x, float y, float z, int seed)
    {
        float ab = Mathf.PerlinNoise(x, Mathf.PerlinNoise(y, seed));
        float bc = Mathf.PerlinNoise(y, Mathf.PerlinNoise(z, seed));
        float ac = Mathf.PerlinNoise(x, Mathf.PerlinNoise(z, seed));

        float ba = Mathf.PerlinNoise(y, Mathf.PerlinNoise(x, seed));
        float cb = Mathf.PerlinNoise(z, Mathf.PerlinNoise(y, seed));
        float ca = Mathf.PerlinNoise(z, Mathf.PerlinNoise(x, seed));

        float abc = ab + bc + ac + ba + cb + ca;
        return abc / 6f;
    }

}