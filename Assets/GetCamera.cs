using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        GetComponent<Canvas>().planeDistance = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
