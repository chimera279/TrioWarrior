using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followPlayer : MonoBehaviour
{
    public GameObject player;
    Quaternion camRotation;
    Vector3 terrainCoord;
    public Vector3 positionOffset, defaultPos;
    Terrain terr;
    float x, y;
    Rigidbody rb;
    public bool isColliding;
    // Start is called before the first frame update
    void Start()
    {
        terr = GameObject.FindGameObjectWithTag("MainTerrain").GetComponent<Terrain>();
        terrainCoord = new Vector3();
        x = 0f;
        y = 0f;
        positionOffset = new Vector3(0, 2, -4f);
        defaultPos = positionOffset;
        rb = GetComponent<Rigidbody>();
        isColliding = false;
    }

    private void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        x += Input.GetAxis("Mouse Y");
        y += Input.GetAxis("Mouse X");
        x = Mathf.Clamp(x, -45, 45);
        camRotation = Quaternion.Euler(-x, y, 0);

        if (Input.GetKey(KeyCode.P))
            positionOffset += Vector3.one * Time.deltaTime * 30f;
        if (Input.GetKey(KeyCode.C))
            camRotation = Quaternion.Euler(0, 0, 0);

        //else
        // if (Vector3.SqrMagnitude(positionOffset - defaultPos) > 1f)
        //    positionOffset += (defaultPos - positionOffset) * Time.deltaTime;
        if (Input.GetKey(KeyCode.Q))
        {
        }
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        SetTerrainCoordinates();

        
        int posXInTerrain = (int)(terrainCoord.x * terr.terrainData.heightmapWidth);
        int posYInTerrain = (int)(terrainCoord.z * terr.terrainData.heightmapHeight);
        float height = terr.terrainData.GetHeights(posXInTerrain, posYInTerrain, 1, 1)[0, 0];
        Vector3 newPos;
        float heightInc;

        newPos = player.transform.position + camRotation * positionOffset;
        //if (height - terrainCoord.y > 0.0025f)
        //{
        //    heightInc = 1 + (height * terr.terrainData.size.y) - transform.position.y;
        //    newPos = transform.position;
        //    newPos.y += heightInc;
        //}
        
        //if(!isColliding)
        transform.position = newPos;
        transform.LookAt(player.transform);
    }

    void SetTerrainCoordinates()
    {
        Vector3 tempCoord = (transform.position - terr.gameObject.transform.position);

        terrainCoord.x = tempCoord.x / terr.terrainData.size.x;
        terrainCoord.y = tempCoord.y / terr.terrainData.size.y;
        terrainCoord.z = tempCoord.z / terr.terrainData.size.z;

    }

    //private void OnCollisionStay(Collision collision)
    //{
    //    isColliding = true;
    //    Vector3 centroid = new Vector3();
    //    foreach (var c in collision.contacts)
    //        centroid += c.normal;
    //    positionOffset += centroid * 1f;    //rb.AddForce(centroid * 60000 * Time.deltaTime, ForceMode.Acceleration);
    //    defaultPos = positionOffset;
    //    player.transform.rotation = Quaternion.Euler(0, 0, 0);
    //    camRotation = Quaternion.Euler(0, 0, 0);

    //}

    //private void OnCollisionEnter(Collision collision)
    //{
    //    isColliding = true;
    //    rb.velocity = Vector3.zero;
    //    rb.angularVelocity = Vector3.zero;
    //}
    //private void Enter(Collision collision)
    //{
    //    isColliding = true;
    //}



    private void OnCollisionExit(Collision collision)
    {
        isColliding = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
     //   StartCoroutine(LerpToDefault());    
   //     positionOffset = defaultPos;    
    }

    IEnumerator LerpToDefault()
    {
        float timer = 0;
        var positionOffsetStart = positionOffset;
        while(timer<2f)
        {
            timer += Time.deltaTime;
            positionOffset = Vector3.Lerp(positionOffsetStart, defaultPos, timer / 2);
            yield return new WaitForEndOfFrame();
        }
    }
}
