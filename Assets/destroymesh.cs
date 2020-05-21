using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroymesh : MonoBehaviour
{
    public GameObject originalObj;
    MeshFilter originalMesh;
    MeshFilter[] newMeshes;
    public Vector3[] oMeshVerts;
    public int pieces = 5;

    // Start is called before the first frame update
    void Start()
    {
        originalObj = this.gameObject;
        originalMesh = GetComponent<MeshFilter>();
        oMeshVerts = originalMesh.mesh.vertices;
        newMeshes = new MeshFilter[pieces];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            DestroyMesh();
        if (Input.GetKeyDown(KeyCode.LeftShift))
            ReformMesh();
    }

    void DestroyMesh()
    {
        
        

       
        Vector3[] pos = new Vector3[originalMesh.mesh.vertexCount];
        int size = oMeshVerts.Length;
        for (int i = 0; i < pieces; i++)
        {
            var destroyed = GameObject.Instantiate(originalObj, transform.position + Vector3.one * i * Random.Range(-1,2), Quaternion.identity);
            var orgMesh = destroyed.GetComponent<MeshFilter>();
            destroyed.tag = "destroyed";
            newMeshes[i] = orgMesh;

            for (int j = 0; j < size / pieces; j+=1)
                pos[j] = originalMesh.mesh.vertices[j*(i)];
            for(int j=size/pieces;j<newMeshes[i].mesh.vertexCount;j++)
                pos[j] = Vector3.zero;
            
            newMeshes[i].mesh.vertices = pos;
            destroyed.GetComponent<MeshFilter>().mesh.vertices = newMeshes[i].mesh.vertices;
            destroyed.GetComponent<MeshCollider>().sharedMesh = destroyed.GetComponent<MeshFilter>().mesh;
        }
        
            
        

        originalObj.SetActive(false);
    }

    void ReformMesh()
    {
        var destroyed = GameObject.FindGameObjectsWithTag("destroyed");
        foreach (var d in destroyed)
            Destroy(d);
        originalObj.SetActive(true);
    }
}
