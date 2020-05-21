using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckMesh : MonoBehaviour
{
    public int index;
    public Vector3 meshCoord;
    public GameObject positionChecker;
    MeshFilter thisMesh;
    // Start is called before the first frame update
    void Start()
    {
        thisMesh = GetComponent<MeshFilter>();
        index = 0;
        meshCoord = thisMesh.mesh.vertices[index];
    }

    // Update is called once per frame
    void Update()
    {
        meshCoord = thisMesh.mesh.vertices[index];
        positionChecker.transform.localPosition = meshCoord;
    }
}
