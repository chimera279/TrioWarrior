using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetMeshPoints : MonoBehaviour
{
    MeshFilter thisMesh, newMesh;
    public int armSeed;
    // Start is called before the first frame update
    void Start()
    {
        thisMesh = GetComponent<MeshFilter>();
        Vector3[] verts = thisMesh.mesh.vertices;
        List<Vector3> newVerts = new List<Vector3>();
        for (int i = 0; i < verts.Length; i++)
        {
             if (Mathf.Abs(verts[i].x) < 0.25f)

                verts[i] = Mathf.PerlinNoise(verts[i].x, Random.value) * Vector3.right * 2 * (verts[i].x > 0 ? 1 : -1f);
            if (Mathf.Abs(verts[i].z) < 0.25f)
                verts[i] = Mathf.PerlinNoise(verts[i].z, Random.value) * Vector3.forward * 2 * (verts[i].z > 0 ? 1 : -1f);    //   Spike
            if (Mathf.Abs(verts[i].y) < 0.5f)
                verts[i] = Mathf.PerlinNoise(verts[i].y, Random.value) * Vector3.up * 2 * (verts[i].y > 0 ? 1 : -1f);
            //   verts[i].x += verts[i].x > 0 ? 1 : -1;      // Flat
            // verts[i] = Random.onUnitSphere*2 + Vector3.right * 5f * (verts[i].x > 0 ? 1 : -1);     // Sphere-Spread
        }
    //    newMesh = new MeshFilter
    //    {
    //        mesh = new Mesh
    //        {
    //            vertices = new Vector3[newVerts.Count]
    //}
    //    };
    //    newMesh.mesh.vertices = newVerts.ToArray();
    //    thisMesh.mesh = newMesh.mesh;

      thisMesh.mesh.vertices = verts;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
