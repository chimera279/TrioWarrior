using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageLevels : MonoBehaviour
{
    public Image[] XPBars;
    public Text[] XPLevels;
    public Text totalLevel;
    Moveonterrain player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Moveonterrain>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 3; i++)
        {
            XPBars[i].fillAmount = player.XP[i] / player.XPLevelThreshold[i];
            XPLevels[i].text = Moveonterrain.levels[i].ToString();
            totalLevel.text = Moveonterrain.totalLevel.ToString();
            
            switch(i)
            {
                case 0:
                    XPBars[i].material.SetVector("_AlbedoColor", Color.red * 2.7f);
                    break;
                case 1:
                    XPBars[i].material.SetVector("Color", Color.blue * 2.7f);
                    break;
                case 2:
                    XPBars[i].material.SetVector("Color", Color.yellow * 2.7f);
                    break;
            }
        }
    }
}
