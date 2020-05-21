using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentObject : MonoBehaviour
{
    public bool isComplete = false;
    public EnvironmentType type;
    public Attacks.AtkDefElement attackElement;
    // Start is called before the first frame update
    public virtual void Initialize()
    {
        
    }

    // Update is called once per frame
    public virtual void Refresh()
    {
        
    }
}
