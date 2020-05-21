using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPFlame : MonoBehaviour
{
    RPSEnemy enemy;
    Renderer hpflameSet;
    // Start is called before the first frame update
    void Start()
    {
        enemy = transform.parent.GetComponent<RPSEnemy>();
        hpflameSet = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        hpflameSet.material.color = Color.Lerp(Color.red, Color.green, enemy.hp / enemy.maxHp);
        hpflameSet.material.SetColor("_Emission",Color.Lerp(Color.red, Color.green, enemy.hp / enemy.maxHp));
    }
}
