using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaTerrific : MonoBehaviour
{
    public float f, ping, pong, slowDown;
    List<Renderer> waterials;
    // Start is called before the first frame update
    void Start()
    {
        waterials = GetComponentsInChildren<Renderer>().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        f = Mathf.PingPong(Time.time / slowDown, pong - ping) + ping;
        foreach (var w in waterials)
        {
            w.material.SetFloat("_BumpScale", (f + ping) / (waterials.IndexOf(w)+1) );
            Color c = w.material.color;
            c.a = f + pong*2;
            w.material.color = c;
        }
    }
}
