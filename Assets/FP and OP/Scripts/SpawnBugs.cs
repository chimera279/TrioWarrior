using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBugs : MonoBehaviour
{
    // Start is called before the first frame update
   public static Vector3 SpawnBox = new Vector3(500,500,500);
    public Transform player;
    float time = 0f;
    public float timeDelay=1.55f;
    public int listCount,rockCount,paperCount,scissorCount;
    int[] counts;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        counts = new int[3];

    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        listCount = EnemyManager.Instance.enemyList.Count;
        counts = new int[3];
        foreach (var e in EnemyManager.Instance.enemyList)
            counts[(int)e.attackElement]++;
        int bugGen = Random.Range(0, 6);
        int count = EnemyManager.Instance.enemyList.Count;
        SpawnBox = new Vector3(500, 500, 500) * (1 - Moveonterrain.totalLevel / 28);
        foreach (var e in EnemyManager.Instance.enemyList)
            if (e.CompareTag("Special"))
                count--;
        if (time > timeDelay&&count< 7 + (Moveonterrain.totalLevel+1)/3)
        {
            Vector3 spawnPos = new Vector3(Random.Range(player.position.x - SpawnBox.x, player.position.x + SpawnBox.x),
                   Random.Range(player.position.y - SpawnBox.y, player.position.y + SpawnBox.y),
                   Random.Range(player.position.z - SpawnBox.z, player.position.z + SpawnBox.z));
           if(Vector3.SqrMagnitude(spawnPos - player.position)<Mathf.Pow(50/ (Moveonterrain.totalLevel + 1) / 3,2))
            spawnPos += -(player.position - spawnPos).normalized/Vector3.Distance(player.position,spawnPos);
            for (int i = 0; i < counts.Length; i++)
            {
                if (counts[i] < listCount / 9)
                    EnemyManager.Instance.CreateBug((Attacks.AtkDefElement)i, spawnPos);
            }
            switch (bugGen)
            {
                case 0:
                    EnemyManager.Instance.CreateBug(Attacks.AtkDefElement.Rock, spawnPos);
                    break;
                case 1:
                    EnemyManager.Instance.CreateBug(Attacks.AtkDefElement.Paper, spawnPos);
                    break;
                case 2:
                    EnemyManager.Instance.CreateBug(Attacks.AtkDefElement.Scissors, spawnPos);
                    break;
                default:
                    break;
            }
            time = 0f;
        }

       
    }
}
