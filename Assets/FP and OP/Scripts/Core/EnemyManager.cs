using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BugType {Animation,Gameplay,Model}

public class EnemyManager 
{
    #region BugManagerSingleton
    private static EnemyManager instance = null;

    private EnemyManager()
    {
    }

    public static EnemyManager Instance
    {
        get
        {
            if (instance == null)            
                instance = new EnemyManager();
            
            return instance;
        }
    }
    #endregion

    //public Vector3 StartingPosition;
    //public Vector3 aimingDirection;
    //public float speed;

    public List<RPSEnemy> enemyList = new List<RPSEnemy>();
    public Transform target = GameObject.FindGameObjectWithTag("Player").transform;
    public Material[] baseMats;
    public Texture2D[] baseTexts;
    public Material dissolveMat;

    public void Initialize()
    {
        baseMats = new Material[3] { Resources.Load<Material>("Materials/RockMat"), Resources.Load<Material>("Materials/PaperMat"), Resources.Load<Material>("Materials/ScissorMat") };
        dissolveMat = Resources.Load<Material>("Materials/Dissolve");
        baseTexts = new Texture2D[3] { Resources.Load<Texture2D>("Textures/Base/Rock"), Resources.Load<Texture2D>("Textures/Base/Paper"), Resources.Load<Texture2D>("Textures/Base/Scissor") };
    }
     
    public void Refresh()
    {
       //foreach(Bug b in bugList)
       // {
       //     b.Refresh();
       // }
        foreach (RPSEnemy e in enemyList.ToArray())
        {
            // if ()
            if (!e.isAlive || Vector3.SqrMagnitude(e.gameObject.transform.position - target.position)>Mathf.Pow(1270,2))
                BugFinished(e);
        }

    }
    public void CreateBug(Attacks.AtkDefElement elemtype, Vector3 startPos)
    {
        RPSEnemy e =Factory.Instance.InitializeEnemy(elemtype,startPos);
        Renderer rend;
        if (e.attackElement == Attacks.AtkDefElement.Rock)
            rend = e.GetComponent<Renderer>();
        else rend = e.transform.Find("Shape").GetComponent<Renderer>();
        rend.material = baseMats[(int)e.attackElement];
        e.RefreshStats();
        enemyList.Add(e);


    }
    public void BugFinished(RPSEnemy enemy)
    {
        //bool bugKilled = false;
        //enemy.transform.position = enemy.transform.position;
        //bugKilled = enemy.DoKillBug();
        //if(bugKilled)
        if (enemyList.Contains(enemy))
        {
            enemyList.Remove(enemy);
            if (!enemy.gameObject.CompareTag("Special"))
            {
                enemy.KillEnemy();
            }
            
        }
    }

 


}
