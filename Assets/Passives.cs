using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class Passives : MonoBehaviour
{
    public int buff, activeScript;
    Moveonterrain player;
    public TextMeshProUGUI text;
    List<RPSEnemy> childScripts;
    public enum PassivesList {MeleeRange,AoERange,RangedRange, MeleeDmgMult, AoEDmgMult, RangedDmgMult, MeleeAtkCooldown, RangedAtkCooldown, AoEAtkCooldown, MeleeSpeed, RangedSpeed, AoESpeed, MeleeXP,RangedXP, AoEXP, JumpForce,FlySpeed, RunSpeed, HP, Regen, BaseDmg,EnumLength }
    public PassivesList passive;
    public float buffAmtPercent, switchingTimer, switchingTime;
    public Attacks.AtkDefElement buffElement;
    // Start is called before the first frame update
    void Start()
    {
        childScripts = GetComponents<RPSEnemy>().ToList();
        text = GetComponentInChildren<TextMeshProUGUI>();
        foreach (var c in childScripts)
        {
            EnemyManager.Instance.enemyList.Add(c);
            c.RefreshStats();
            c.sightRange = 15000;
        }
        switchingTime = Random.Range(3f, 5f);
        switchingTimer = 0;
        activeScript = Random.Range(0, childScripts.Count);
    }

    // Update is called once per frame
    void Update()
    {
        switchingTimer += Time.deltaTime;
        if(switchingTimer>switchingTime)
        {
            switchingTimer = 0;
            switchingTime = Random.Range(3f, 5f);
            activeScript = Random.Range(0, childScripts.Count);
        }
        for (int i = 0; i < childScripts.Count; i++)
        {
            if (!EnemyManager.Instance.enemyList.Contains(childScripts[i]))
            {
                Destroy(childScripts[i]);
                childScripts.Remove(childScripts[i]);
            }
            if (i != activeScript)
                childScripts[i].enabled = false;
            else childScripts[i].enabled = true;
        }
        if (childScripts.Count == 0)
            PerformPassive();
    }

    public void InitializePassives(Attacks.AtkDefElement mineElement, Attacks.AtkDefElement playerElement, Moveonterrain _player)
    {
        player = _player;
        if (mineElement == (Attacks.AtkDefElement)(((int)playerElement + 1) % 3))
            buff = -1;       //  Debug.Log(source + " Made Bad Attack On " + other);
        else if (playerElement == (Attacks.AtkDefElement)(((int)mineElement + 1) % 3))
            buff = 1;        //Debug.Log(source + " Made Good Attack On " + other);

        else if (playerElement == mineElement)
            buff = 0;     //Debug.Log(source + " Made Neutral Attack On " + other);
        passive = (PassivesList)Random.Range(0, 15);
        if (Random.Range(0, 100) == 27)
            buffAmtPercent = Random.Range(27f, 54f);
        else buffAmtPercent = Random.Range(127, 279f);
        buffElement = mineElement;

        text.text = passive.ToString() + "  " + ((int)passive < 15 ? buffElement.ToString():" ");
        switch(buff)
        {
            case -1:
                text.color = Color.red;
                text.text += "-- " + buffAmtPercent.ToString("F0") + "%";
                break;
            case 0:
                text.color = Color.clear;
                break;
            case 1:
                text.color = Color.green;
                text.text += "++ " + buffAmtPercent.ToString("F0") + "%";
                break;
        }
    }

    void PerformPassive()
    {
        switch(passive)
        {
            case PassivesList.MeleeRange:
                player.AttackRanges[0, (int)buffElement] += 1 * (buffAmtPercent / 100) * buff;
                break;
            case PassivesList.RangedRange:
                player.AttackRanges[1, (int)buffElement] += 1 * (buffAmtPercent / 100) * buff;
                break;
            case PassivesList.AoERange:
                player.AttackRanges[2, (int)buffElement] += 1 * (buffAmtPercent / 100) * buff;
                break;

            case PassivesList.MeleeDmgMult:
                player.AttackDmgMults[0, (int)buffElement] += 1 * (buffAmtPercent / 100) * buff;
                break;
            case PassivesList.RangedDmgMult:
                player.AttackDmgMults[1, (int)buffElement] += 1 * (buffAmtPercent / 100) * buff;
                break;
            case PassivesList.AoEDmgMult:
                player.AttackDmgMults[2, (int)buffElement] += 1 * (buffAmtPercent / 100) * buff;
                break;

            case PassivesList.MeleeAtkCooldown:
                player.AttackTimers[0, (int)buffElement] += 1 * (buffAmtPercent / 100) * buff;
                break;
            case PassivesList.RangedAtkCooldown:
                player.AttackTimers[1, (int)buffElement] += 1 * (buffAmtPercent / 100) * buff;
                break;
            case PassivesList.AoEAtkCooldown:
                player.AttackTimers[2, (int)buffElement] += 1 * (buffAmtPercent / 100) * buff;
                break;

            case PassivesList.MeleeSpeed:
                player.AttackSpeeds[0, (int)buffElement] += 1 * (buffAmtPercent / 100) * buff;
                break;
            case PassivesList.RangedSpeed:
                player.AttackSpeeds[1, (int)buffElement] += 1 * (buffAmtPercent / 100) * buff;
                break;
            case PassivesList.AoESpeed:
                player.AttackSpeeds[2, (int)buffElement] += 1 * (buffAmtPercent / 100) * buff;
                break;

            case PassivesList.MeleeXP:
                player.XPGainAmt[0, (int)buffElement] += 1 * (buffAmtPercent / 100) * buff;
                break;
            case PassivesList.RangedXP:
                player.XPGainAmt[1, (int)buffElement] += 1 * (buffAmtPercent / 100) * buff;
                break;
            case PassivesList.AoEXP:
                player.XPGainAmt[2, (int)buffElement] += 1 * (buffAmtPercent / 100) * buff;
                break;

            case PassivesList.FlySpeed:
                player.flySpeed += 5 * (buffAmtPercent / 100) * buff;
                break;
            case PassivesList.JumpForce:
                player.rockSpeed += 127 * (buffAmtPercent / 100) * buff;
                break;
            case PassivesList.RunSpeed:
                player.speed += 1800 * (buffAmtPercent / 100) * buff;
                break;

            case PassivesList.HP:
                player.maxHp += 279 * (buffAmtPercent / 100) * buff;
                break;
            case PassivesList.Regen:
                player.regenRate += 1.5f * (buffAmtPercent / 100) * buff;
                break;
            case PassivesList.BaseDmg:
                player.dmgValue += 2.7f * (buffAmtPercent / 100) * buff;
                break;

        }
        Destroy(this.gameObject);
    }
}


