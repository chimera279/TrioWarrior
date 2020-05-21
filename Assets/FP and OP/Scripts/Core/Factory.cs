using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory 
{
    #region FactorySingleton
    private static Factory instance = null;
    private Factory() { }

    public static Factory Instance 
    {
        get
        {
            if(instance==null)            
                instance = new Factory();

            return instance;
        }
    }
    #endregion

    

    RPSEnemy rock,paper,scissors;
    EnvironmentObject tower,statue;
    ParticleSystem melee, ranged, aoe;
    GameObject BulletParent = new GameObject("AI Parent");
    GameObject EnvironmentParent = new GameObject("Object Parent");
    GameObject ParticlesParent = new GameObject("Particles Parent");
    public void Initialize()
    {
        rock = Resources.Load<RPSEnemy>("Prefabs/RockAI");
        paper = Resources.Load<RPSEnemy>("Prefabs/Paper");
        scissors = Resources.Load<RPSEnemy>("Prefabs/ScissorAI");

        tower = Resources.Load<EnvironmentObject>("Prefabs/Tower");
        statue = Resources.Load<EnvironmentObject>("Prefabs/Statue");

        melee = Resources.Load<ParticleSystem>("Particles/Melee");
        ranged = Resources.Load<ParticleSystem>("Particles/Ranged");
        aoe = Resources.Load<ParticleSystem>("Particles/AoE");
    }
    public void Refresh()
    {

    }

    public RPSEnemy InitializeEnemy(Attacks.AtkDefElement elemtype, Vector3 startpos)
    {
        RPSEnemy tempEnemy = null;

        if (ObjectPool.Instance.HasBug(elemtype))
        {
            tempEnemy = ObjectPool.Instance.DepoolBug(elemtype);
            // tempBullet.gameObject.transform.position = startpos;

        }
        else
        {

            switch (elemtype)
            {
                case Attacks.AtkDefElement.Rock:
                    tempEnemy = GameObject.Instantiate(rock).GetComponent<RPSEnemy>();
                    break;
                case Attacks.AtkDefElement.Paper:
                    tempEnemy = GameObject.Instantiate(paper).GetComponent<RPSEnemy>();
                    break;
                case Attacks.AtkDefElement.Scissors:
                    tempEnemy = GameObject.Instantiate(scissors).GetComponent<RPSEnemy>();
                    break;
            }
        }
        tempEnemy.gameObject.transform.position = startpos;
        tempEnemy.gameObject.transform.parent = BulletParent.transform;

        tempEnemy.gameObject.SetActive(true);
        return tempEnemy;
    }

    public EnvironmentObject InitializeObject(EnvironmentType elemtype, Vector3 startpos)
    {
        EnvironmentObject tempObj = null;

        if (ObjectPool.Instance.HasObject(elemtype))
        {
            tempObj = ObjectPool.Instance.DepoolObject(elemtype);
            // tempBullet.gameObject.transform.position = startpos;

        }
        else
        {

            switch (elemtype)
            {
                case EnvironmentType.Tower:
                    tempObj = GameObject.Instantiate(tower).GetComponent<EnvironmentObject>();
                    break;
               case EnvironmentType.Statue:
                    tempObj = GameObject.Instantiate(statue).GetComponent<EnvironmentObject>();
                    break;
                //case Attacks.AtkDefElement.Scissors:
                //    tempEnemy = GameObject.Instantiate(scissors).GetComponent<RPSEnemy>();
                //    break;
            }
        }
        tempObj.gameObject.transform.position = startpos;
        tempObj.gameObject.transform.parent = EnvironmentParent.transform;

        tempObj.gameObject.SetActive(true);
        return tempObj;
    }
    
    public ParticleSystem InitializeParticles(Attacks.AttackType elemtype, Vector3 startpos)
    {
        ParticleSystem tempEnemy = null;

        if (ObjectPool.Instance.HasParticle(elemtype))
        {
            tempEnemy = ObjectPool.Instance.DepoolParticle(elemtype);
            // tempBullet.gameObject.transform.position = startpos;

        }
        else
        {

            switch (elemtype)
            {
                case Attacks.AttackType.Melee:
                    tempEnemy = GameObject.Instantiate(melee).GetComponent<ParticleSystem>();
                    break;
                case Attacks.AttackType.Ranged:
                    tempEnemy = GameObject.Instantiate(ranged).GetComponent<ParticleSystem>();
                    break;
                case Attacks.AttackType.AoE:
                    tempEnemy = GameObject.Instantiate(aoe).GetComponent<ParticleSystem>();
                    break;
            }
        }
        tempEnemy.gameObject.transform.position = startpos;
        tempEnemy.gameObject.transform.parent = ParticlesParent.transform;

        tempEnemy.gameObject.SetActive(true);
        return tempEnemy;
    }

}