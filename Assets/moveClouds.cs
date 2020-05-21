using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveClouds : MonoBehaviour
{
    Renderer mat;
    float x, y;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        x = Mathf.PingPong(Time.time / 60, 24 - 1) + 1;
        y = Mathf.PingPong(Time.time / 60, 12 - 1) + 1;
        mat.material.SetFloatArray("_Offset", new List<float>() { x, y });

    }
}
