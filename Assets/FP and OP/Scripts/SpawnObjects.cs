using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjects : MonoBehaviour
{
    // Start is called before the first frame update
    public static Vector3 SpawnBox = new Vector3(4000, 4000, 4000);
    public Transform player;
    float time = 0f;
    public float timeDelay = 1.55f;
    public int listCount,towers,statues;
    int[] towercounts,statuecounts;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        int bugGen = Random.Range(0, 3);
        listCount = EnvironmentManager.Instance.enemyList.Count;
        towercounts = new int[3];
        statuecounts = new int[3];
        towers = 0;
        statues = 0;
        for (int i = 0; i < listCount; i++)
        {
            var e = EnvironmentManager.Instance.enemyList[i];
            if (e.type == EnvironmentType.Tower)
            {
                towers++;
                towercounts[(int)e.attackElement]++;
            }
            else
            {
                statues++;
                statuecounts[(int)e.attackElement]++;
            }
        }
        if (EnvironmentManager.Instance.enemyList.Count < 150)
        {
            Vector3 spawnPos = new Vector3(Random.Range(player.position.x - SpawnBox.x, player.position.x + SpawnBox.x),
                   Random.Range(player.position.y - SpawnBox.y, player.position.y + SpawnBox.y),
                   Random.Range(player.position.z - SpawnBox.z, player.position.z + SpawnBox.z));
            if (Vector3.SqrMagnitude(spawnPos - player.position) < Mathf.Pow(60, 2)) 
            spawnPos += -(player.position - spawnPos).normalized / Vector3.Distance(player.position, spawnPos);
            if(towers<30)
            {
                EnvironmentManager.Instance.CreateObject(EnvironmentType.Tower, spawnPos);

            }
            else if(statues<50)
                EnvironmentManager.Instance.CreateObject(EnvironmentType.Statue, spawnPos);
            else
            {
                Vector3 otherSpawnPos = new Vector3(Random.Range(player.position.x - SpawnBox.x, player.position.x + SpawnBox.x),
                      Random.Range(player.position.y - SpawnBox.y, player.position.y + SpawnBox.y),
                      Random.Range(player.position.z - SpawnBox.z, player.position.z + SpawnBox.z));
                if (Vector3.SqrMagnitude(spawnPos - player.position) < Mathf.Pow(60, 2))
                    spawnPos += -(player.position - spawnPos).normalized / Vector3.Distance(player.position, spawnPos);
                switch (bugGen)
                {
                    case 0:
                        EnvironmentManager.Instance.CreateObject(EnvironmentType.Tower, otherSpawnPos);
                        break;

                    //case 2:
                    //    EnemyManager.Instance.CreateBug(Attacks.AtkDefElement.Scissors, spawnPos);
                    //    break;
                    default:
                        EnvironmentManager.Instance.CreateObject(EnvironmentType.Statue, otherSpawnPos);
                        break;
                }

            }



            time = 0f;
        }


    }
}
