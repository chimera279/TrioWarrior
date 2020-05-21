using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPSEnemy : MonoBehaviour
{
    public enum DefenseType { Dmg, DrainHP, BolsterAtk };
    public DefenseType defenseType;
    public Attacks.AttackType attackType;
    public Attacks.AtkDefElement attackElement;
    public float hp, maxHp, dmgValue, maxDmg, attackTimeMin, attackTimeMax, sightRange;
    public bool isAlive = true;
    public static Color[] RPSColors = new Color[3] { Color.red, Color.blue, Color.yellow };
    public void DoDamage(float dmg, Attacks.AttackType atkType, Attacks.AtkDefElement playerElement, Moveonterrain player=null)
    {
        int atkMultiplier = 0;
        float playerMult = 1;
        if (player != null)
            playerMult = player.AttackDmgMults[(int)atkType, (int)playerElement];
        switch(atkType)
        {
            case Attacks.AttackType.AoE:
                atkMultiplier = 1;
                break;
            case Attacks.AttackType.Ranged:
                atkMultiplier = 3;
                break;
            case Attacks.AttackType.Melee:
                atkMultiplier = 5;
                break;
        }
        hp -= dmg * atkMultiplier * playerMult;

        if (hp < 1)
            isAlive = false;

    }

    public void DoDefense(int defenseMode, RPSEnemy source,Attacks.AtkDefElement attackingElement,Attacks.AttackType atkType)
    {
        int XPMult = 2;
        if(defenseMode==1)      // Good Defense
        {
            XPMult = -1;
            switch(defenseType)
            {
                case DefenseType.Dmg:
                    source.hp -= dmgValue * 5;
                    break;
                case DefenseType.DrainHP:
                    hp += dmgValue * 3;
                    source.hp -= dmgValue * 3;
                    break;
                case DefenseType.BolsterAtk:
                    if(dmgValue<20)
                    dmgValue *= 2;
                    break;
            }
        }
        else if(defenseMode==0)
        {
            XPMult = 4;
            switch (defenseType)
            {
                case DefenseType.Dmg:
                    hp -= dmgValue * 5;
                    break;
                case DefenseType.DrainHP:
                    hp -= dmgValue * 3;
                    source.hp += dmgValue * 3;
                    break;
                case DefenseType.BolsterAtk:
                    if(dmgValue>float.MinValue)
                    dmgValue /= 2;
                    break;
            }

        }

        if (source.CompareTag("Player"))
            source.GetComponent<Moveonterrain>().CalculateXPDamage(XPMult, attackingElement, atkType);
        else Debug.Log("PAJWO");
    }

    public void RefreshAttackTimers()
    {
            attackTimeMin = Random.Range(1f, 1.5f) * 1.5f * (1-Moveonterrain.levels[(int)attackElement]/10);
            attackTimeMax = Random.Range(1.75f,2.25f) * 1.5f * (1 - Moveonterrain.levels[(int)attackElement]/10);  
    }

    public virtual void RefreshStats()
    {
        isAlive = true;
    }

    public void KillEnemy()
    {
        StartCoroutine(DissolveBug(this));
    }
    public IEnumerator DissolveBug(RPSEnemy e)
    {
         
        Renderer rend;
        if (attackElement == Attacks.AtkDefElement.Rock)
            rend = e.GetComponent<Renderer>();
        else rend = e.transform.Find("Shape").GetComponent<Renderer>();
        rend.material = EnemyManager.Instance.dissolveMat;
        rend.material.SetVector("_EdgeColor1", RPSColors[(int)e.attackElement] * 4f);
        rend.material.SetVector("_Color", RPSColors[(int)e.attackElement] * 1f);
        float timer = 0f;
        while (timer < 2f)
        {
            timer += Time.deltaTime;
            float cutoff = Mathf.Lerp(0, 1, timer / 2f);
            rend.material.SetFloat("_cutoff", cutoff);
            //Debug.Log(timer / 3f);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        rend.material = EnemyManager.Instance.baseMats[(int)attackElement];
        e.gameObject.SetActive(false);
        ObjectPool.Instance.PoolBug(e.attackElement, e);

    }
}
