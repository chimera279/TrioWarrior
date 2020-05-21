using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightmappedMesh : MonoBehaviour
{
    public Texture2D map;
    int width, height;
    MeshFilter thisMesh;
    public float[,] chunk;
    // Start is called before the first frame update
    void Start()
    {
        thisMesh = GetComponent<MeshFilter>();
        width = GetComponent<ProceduralCylinder>().radialSegments;
        height = GetComponent<ProceduralCylinder>().heightSegments;
        CreateChunk();
        var newMesh = thisMesh.mesh.vertices;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                newMesh[j * i] += (newMesh[j*i]) * (chunk[i,j]) * .24f/Vector3.Distance(transform.position,newMesh[j*i]);
            }

        }
        thisMesh.mesh.vertices = newMesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateChunk()
    {
        //  activeMap = Random.Range(0, Maps.Count);
        chunk = new float[width, height];
        Vector2Int startPos = new Vector2Int(Random.Range(0, 501), Random.Range(0, 501));
        //   positionPoint = new Vector2Int(Random.Range(0, Maps[activeMap].width/2), Random.Range(0, Maps[activeMap].height/2));
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                chunk[i, j] = map.GetPixel(startPos.x + i, startPos.y + j).r/2;
    }
}
