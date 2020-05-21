using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Statue : EnvironmentObject
{
    public Light statueLight;
    Renderer rend;
    float[] XP;
    Passives.PassivesList statuePassive;
    Attacks.AtkDefElement buffElement;
    float buffAmtPercent;
    public TextMeshProUGUI text;
    int buff = 1;
    Vector3 basePos;
    // Start is called before the first frame update
    public override void Initialize()
    {
        type = EnvironmentType.Statue;
        isComplete = false;
        rend = GetComponent<Renderer>();
        attackElement = (Attacks.AtkDefElement)Random.Range(0, 3);
        XP = new float[3] { Random.Range(4f, 20f), Random.Range(4f, 20f), Random.Range(4f, 20f) };
        for (int i = 0; i < 3; i++)
        {
            XP[i] *= (Moveonterrain.levels[i] + 1) / 10;
        }
        statueLight.color = RPSEnemy.RPSColors[(int)attackElement];
        rend.material.color = RPSEnemy.RPSColors[(int)attackElement] /2;
        rend.material.SetColor("_ReflectionColor", RPSEnemy.RPSColors[(int)attackElement] / 2);
        buffElement = attackElement;
        statuePassive = (Passives.PassivesList)(15 + (int)buffElement + 3 * Random.Range(0, 2));
        buffAmtPercent = Random.Range(5f, 50f);
        text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = statuePassive.ToString() + "  ++ " + buffAmtPercent.ToString("F0") + "%";
        text.color = Color.green;
        basePos = transform.localPosition;
    }

    // Update is called once per frame
    public override void Refresh()
    {
        transform.Rotate(transform.forward * Time.deltaTime * 45f);
        var pos = transform.localPosition;
        pos.z = basePos.z+ Mathf.PingPong(Time.time /  4.5f, 3 - 1f) + 1f;
        transform.localPosition = pos;
        //transform.localPosition = pos;

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            var player = other.GetComponent<Moveonterrain>();
            for (int i = 0; i < 3; i++)
            {
                player.XP[i] += XP[i];
                player.XP[i] = Mathf.Clamp(player.XP[i], 0, player.XPLevelThreshold[i] + 27);
            }
            PerformPassive(player);
            isComplete = true;
        }
    }

    void PerformPassive(Moveonterrain player)
    {
        switch (statuePassive)
        {
            case Passives.PassivesList.JumpForce:
                player.flySpeed += 5 * (buffAmtPercent / 100) * buff;
                break;
            case Passives.PassivesList.FlySpeed:
                player.rockSpeed += 127 * (buffAmtPercent / 100) * buff;
                break;
            case Passives.PassivesList.RunSpeed:
                player.speed += 1800 * (buffAmtPercent / 100) * buff;
                break;

            case Passives.PassivesList.HP:
                player.maxHp += 279 * (buffAmtPercent / 100) * buff;
                break;
            case Passives.PassivesList.Regen:
                player.regenRate += 1.5f * (buffAmtPercent / 100) * buff;
                break;
            case Passives.PassivesList.BaseDmg:
                player.dmgValue += 2.7f * (buffAmtPercent / 100) * buff;
                break;
        }
    }
}
