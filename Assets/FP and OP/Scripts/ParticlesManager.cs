using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ParticlesManager
{
    #region ParticlesManagerSingleton
    private static ParticlesManager instance = null;

    private ParticlesManager()
    {
    }

    public static ParticlesManager Instance
    {
        get
        {
            if (instance == null)
                instance = new ParticlesManager();

            return instance;
        }
    }
    #endregion

    //public Vector3 StartingPosition;
    //public Vector3 aimingDirection;
    //public float speed;

    public List<ParticleSystem> particleList = new List<ParticleSystem>();

    public void Initialize()
    {

    }
    public void Refresh()
    {
        //foreach(Bug b in bugList)
        // {
        //     b.Refresh();
        // }
        foreach (ParticleSystem p in particleList.ToArray())
        {
            // if ()
            if (p.isStopped)
                BugFinished(p);
        }

    }
    public void CreateParticle(Attacks.AttackType atkType, Attacks.AtkDefElement elemType, Vector3 startPos)
    {
        ParticleSystem p = Factory.Instance.InitializeParticles(atkType, startPos);
        if (atkType == Attacks.AttackType.Ranged)
        {
            var rend = p.GetComponent<Renderer>();
            rend.material.SetVector("_Color", RPSEnemy.RPSColors[(int)elemType]/2 * 3f);
            rend.material.SetVector("_EmissionColor", RPSEnemy.RPSColors[(int)elemType] * 3f);
        }
        else if (atkType == Attacks.AttackType.AoE)
        {
            var childPart = p.transform.Find("Embers").GetComponent<ParticleSystem>();
            var childPartMain = childPart.main;
            childPartMain.startColor = RPSEnemy.RPSColors[(int)elemType];
            var rends = p.transform.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < rends.Length; i++)
                rends[i].material.SetVector("_Color", RPSEnemy.RPSColors[(int)elemType]/2 * 3f);
        }
        else if (atkType == Attacks.AttackType.Melee)
        {
            var rend = p.GetComponent<Renderer>();
            for (int i = 0; i < rend.materials.Length; i++)
            {
                rend.materials[i].SetVector("_Color", RPSEnemy.RPSColors[(int)elemType]/2 * 3f);
            }
            //p.GetComponentInChildren<Renderer>().material.SetVector("_Color", RPSEnemy.RPSColors[(int)elemType] * 2f);
        }
        p.Play();
        particleList.Add(p);

        

        


    }
    public void BugFinished(ParticleSystem enemy)
    {
        //bool bugKilled = false;
        //enemy.transform.position = enemy.transform.position;
        //bugKilled = enemy.DoKillBug();
        //if(bugKilled)
        if (particleList.Contains(enemy))
        {
            particleList.Remove(enemy);
            enemy.gameObject.SetActive(false);
            Attacks.AttackType attackType = Attacks.AttackType.Melee;
            switch(enemy.gameObject.name[0])
            {
                case 'M':
                    attackType = Attacks.AttackType.Melee;
                    break;
                case 'R':
                    attackType = Attacks.AttackType.Ranged;
                    break;
                case 'A':
                    attackType = Attacks.AttackType.AoE;
                    break;
            }
            ObjectPool.Instance.PoolParticle(attackType, enemy);


        }
    }


}
