using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnvironmentType { Tower, Statue };

public class EnvironmentManager
{
    #region EnvironmentManagerSingleton
    private static EnvironmentManager instance = null;

    private EnvironmentManager()
    {
    }

    public static EnvironmentManager Instance
    {
        get
        {
            if (instance == null)
                instance = new EnvironmentManager();

            return instance;
        }
    }
    #endregion

    //public Vector3 StartingPosition;
    //public Vector3 aimingDirection;
    //public float speed;

    public List<EnvironmentObject> enemyList = new List<EnvironmentObject>();
    public Transform target = GameObject.FindGameObjectWithTag("Player").transform;
    Terrain terr;

    public void Initialize()
    {
        terr = GameObject.FindGameObjectWithTag("MainTerrain").GetComponent<Terrain>();
    }
    public void Refresh()
    {
        //foreach(Bug b in bugList)
        // {
        //     b.Refresh();
        // }
        foreach (EnvironmentObject e in enemyList.ToArray())
        {
            e.Refresh();
            // if ()
            if (e.isComplete || Vector3.SqrMagnitude(e.gameObject.transform.position - target.position) > Mathf.Pow(5000, 2))
                BugFinished(e);
        }

    }
    public void CreateObject(EnvironmentType envtype, Vector3 startPos)
    {
        EnvironmentObject e = Factory.Instance.InitializeObject(envtype, startPos);
        Vector3 tempCoord = e.transform.position - terr.gameObject.transform.position;
        Vector3 terrainCoord;
        terrainCoord.x = tempCoord.x / terr.terrainData.size.x;
        terrainCoord.y = tempCoord.y / terr.terrainData.size.y;
        terrainCoord.z = tempCoord.z / terr.terrainData.size.z;
        e.Initialize();
        enemyList.Add(e);

        int X = (int)(terrainCoord.x * terr.terrainData.heightmapWidth);
        int Y = (int)(terrainCoord.z * terr.terrainData.heightmapHeight);
        if (X < 0 || Y < 0 || X > terr.terrainData.heightmapWidth - 2 || Y > terr.terrainData.heightmapHeight - 2)
        {
            BugFinished(e);
        }
        else
        {
            float height = terr.terrainData.GetHeights(X, Y, 1, 1)[0, 0];
            Vector3 newHeightPos = e.transform.position;
            newHeightPos.y = height * terr.terrainData.size.y;
            e.transform.position = newHeightPos;
        }


    }
    public void BugFinished(EnvironmentObject enemy)
    {
        //bool bugKilled = false;
        //enemy.transform.position = enemy.transform.position;
        //bugKilled = enemy.DoKillBug();
        //if(bugKilled)
        if (enemyList.Contains(enemy))
        {
            enemyList.Remove(enemy);
            enemy.gameObject.SetActive(false);
            ObjectPool.Instance.PoolObject(enemy.type, enemy);
            

        }
    }


}
