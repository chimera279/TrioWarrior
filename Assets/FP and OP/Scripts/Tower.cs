using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : EnvironmentObject
{
    Renderer towerMat;
    // Start is called before the first frame update
    public override void Initialize()
    {
        type = EnvironmentType.Tower;
        towerMat = GetComponent<Renderer>();
        attackElement = (Attacks.AtkDefElement)Random.Range(0, 3);
        Color c = new Color() ;
        switch(attackElement)
        {
            case Attacks.AtkDefElement.Rock:
                c = Color.red;
                break;
            case Attacks.AtkDefElement.Paper:
                c = Color.blue;
                break;
            case Attacks.AtkDefElement.Scissors:
                c = Color.yellow;
                break;
        }
        towerMat.material.SetColor("_RimColor", c);
        isComplete = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PerformLevelUp(Moveonterrain player, int mode)
    {
        if(Moveonterrain.levels[(int)attackElement]!=9)
        switch(mode)
        {
            case 0:
                Moveonterrain.levels[(int)attackElement]++;
                player.XPLevelThreshold[(int)attackElement] = 100 + Moveonterrain.levels[(int)attackElement] * 50;
                player.XP[(int)attackElement] = 0;
                break;
            case 1:
                Moveonterrain.levels[(int)attackElement]++;
                player.XPLevelThreshold[(int)attackElement] = 100 + Moveonterrain.levels[(int)attackElement] * 50;
                player.XP[(int)attackElement] = player.XPLevelThreshold[(int)attackElement]/3;
                break;
        }
        isComplete = true;
        player.isGrounded = false;
    }
}
