using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainHeightSet : RPSEnemy
{
    Terrain terr; // terrain to modify
    public Terrain moddingTerrain;
    int hmWidth; // heightmap width
    int hmHeight; // heightmap height
    public Texture2D map;
    float[,] chunk;
    float averageHeight;
    int posXInTerrain; // position of the game object in terrain width (x axis)
    int posYInTerrain; // position of the game object in terrain height (z axis)
    int batchFrames, batchDivider;      //irrelevant now. Old batching Algo
    List<Vector3> batchPosses;

    public bool doMove, coRoutined, inAttackRange;
    public int width = 5, height = 10, attackTimerRandomMult; // the diameter of terrain portion that will raise under the game object
    float desiredHeight = 0; // the height we want that portion of terrain to be
    int heightFrames;
    public float moveTimer,  attackTimer, attackTime;
    float speed, timerLimit, playerDist, randomDirMult;
    
    
    Vector3 randomDir, pingPonger;
    public Transform player;

    public GameObject attackObject;
    DoAttacking attackObjectControl;
    Renderer attackObjRenderer;


    void Awake()
    {

        player = GameObject.FindGameObjectWithTag("Player").transform;
        terr = GameObject.FindGameObjectWithTag("MainTerrain").GetComponent<Terrain>();
        hmWidth = terr.terrainData.heightmapWidth;
        hmHeight = terr.terrainData.heightmapHeight;
        CreateChunk();
        batchFrames = 0;
        batchPosses = new List<Vector3>();
        moveTimer = 0;
        randomDir = Random.onUnitSphere.normalized;
        heightFrames = 1;
        coRoutined = false;
        batchDivider = 32; //Random.Range(32, 37);
        pingPonger = new Vector3();
        attackTimer = 0;
        this.RefreshAttackTimers();
        attackTime = Random.Range(attackTimeMin, attackTimeMax);
        attackElement = Attacks.AtkDefElement.Rock;
        attackType = (Attacks.AttackType)Random.Range(0, 3);
        defenseType = (DefenseType)(Random.Range(0, 3));
        attackObjectControl = attackObject.GetComponent<DoAttacking>();
        attackObjRenderer = attackObject.GetComponent<Renderer>();
        sightRange = Random.Range(200f, 350f);
        isAlive = true;
    }

    void Update()
    {
        CheckTerrainPosition();
        ClipHeight();
        moveTimer += Time.deltaTime;
        if(moveTimer>timerLimit)
        {
            doMove = !doMove;
            moveTimer = 0;
        }

        if (doMove)
        {
            // if (batchFrames > 60)
            
            if (Vector3.SqrMagnitude(transform.position - player.position) < Mathf.Pow(sightRange * transform.localScale.x, 2))
                if (Vector3.SqrMagnitude(transform.position - player.position) > Mathf.Pow(playerDist * transform.localScale.x, 2))
                    transform.Translate((player.position - transform.position).normalized * Random.Range(15f, 19f) * Time.deltaTime * speed);
            if (transform.position.y > 2400)
                transform.Translate(Vector3.down * Time.deltaTime * speed * 2.7f);
            if (transform.position.y < 0)
                transform.Translate(Vector3.up * Time.deltaTime * speed * 2.7f);
            transform.Translate(randomDir * Time.deltaTime * Random.Range(2.5f, 5f) * Random.Range(0f, randomDirMult));
            //   if (batchFrames == 5 )//|| batchFrames == 27)
          //  if ((batchFrames+1) % batchDivider == 0)        // 16 modulus is perfect dig but slow. Help somehow?
          //      batchPosses.Add(transform.position);
            Batcher.AddToBatch(this.gameObject, transform.position);
            batchFrames++;
            coRoutined = false;
            pingPonger = Random.onUnitSphere;

        }
        else
        {
            if (!coRoutined)
            {
                randomDir = Random.onUnitSphere * 5f;
               // StartCoroutine(DoTerrainFrame());
                batchFrames = 0;
                batchPosses = new List<Vector3>();
                //var pingPongPos = transform.position;
                //if (pingPonger != Vector3.zero)
                //{
                //    pingPongPos.x = Mathf.PingPong(Time.time , 2 * (pingPonger.x + 2)) + pingPonger.x - 1;
                //    pingPongPos.y = Mathf.PingPong(Time.time , 2 * (pingPonger.y + 2)) + pingPonger.y - 1;
                //    pingPongPos.z = Mathf.PingPong(Time.time , 2 * (pingPonger.z + 2)) + pingPonger.z - 1;
                //    transform.position = pingPongPos;
                //    Debug.Log("WORIHR");
                //}
            }

        }
        if (Vector3.SqrMagnitude(transform.position - player.transform.position) > Mathf.Pow(playerDist, 2))
            inAttackRange = false;
        else inAttackRange = true;

        if (inAttackRange)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer > attackTime * 2 && !attackObject.activeSelf)
            {
                attackTimer = 0;
                if (Random.Range(0, attackTimerRandomMult) == 0)
                {
                    this.RefreshAttackTimers();
                }
                attackTime = Random.Range(attackTimeMin, attackTimeMax);
                StartCoroutine(Attacks.PerformAttack(attackType, 0, attackObject, this.gameObject, attackTime));
                attackObjectControl.attackType = attackType;
                attackObjectControl.attackElement = attackElement;
                attackObjectControl.source = this.transform;
                attackObjRenderer.material.color = new Color(0.75f, 0, 0, 0.75f);
            }
        }

    }

    void  CheckTerrainPosition()
    {
        // get the normalized position of this game object relative to the terrain
        Vector3 tempCoord = (transform.position - terr.gameObject.transform.position);
        Vector3 coord;
        coord.x = tempCoord.x / terr.terrainData.size.x;
        coord.y = tempCoord.y / terr.terrainData.size.y;
        coord.z = tempCoord.z / terr.terrainData.size.z;

        // get the position of the terrain heightmap where this game object is
        posXInTerrain = (int)(coord.x * hmWidth);
        posYInTerrain = (int)(coord.z * hmHeight);
        bool endCoord = false;

        if (posXInTerrain <= width || posXInTerrain >= hmWidth - 1-width / 2)       // see that it's within bounds of terrain, also considering it's chunk sizes
        { if (posXInTerrain <= width / 2)
                posXInTerrain = hmWidth - 1 - width;
            if (posXInTerrain >= hmWidth - 1 - width / 2)
                posXInTerrain = width;
            endCoord = true;
        }
        if (posYInTerrain <= height || posYInTerrain >= hmHeight - 1 - height / 2)
        {
            if (posYInTerrain <= height / 2)
                posYInTerrain = hmHeight - 1 - height / 2;
            if (posYInTerrain >= hmHeight - 1 - height / 2)
                posYInTerrain = height;
            endCoord = true;
        }
        if (endCoord)
            transform.position = new Vector3(posXInTerrain, transform.position.y, posYInTerrain);       // Reset it to warped point on other side of terrain (Rock based AI only)

    }


        void CreateChunk()
        {
            //  activeMap = Random.Range(0, Maps.Count);
            chunk = new float[width, height];
        averageHeight = 0;
            Vector2Int startPos = new Vector2Int(Random.Range(0, 501), Random.Range(0, 501));
        //   positionPoint = new Vector2Int(Random.Range(0, Maps[activeMap].width/2), Random.Range(0, Maps[activeMap].height/2));
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                chunk[i, j] = map.GetPixel(startPos.x + i, startPos.y + j).r;
                averageHeight += chunk[i, j];
            }
        averageHeight /= (width * height);
        }

    IEnumerator DoTerrainFrame()            // I guess this is null and void now, but this was the code to the individual batching algo, which I reinterpreted into Batcher.
    {
        foreach (var pos in batchPosses)                        //if (Physics.OverlapSphere(transform.position, .25f,1<<LayerMask.NameToLayer("Terrain")).Length == 0)
        {
        for (int k  = 0; k < heightFrames; k++)
        {


            
            // get the normalized position of this game object relative to the terrain
            Vector3 tempCoord = (pos - terr.gameObject.transform.position);
            Vector3 coord;
            coord.x = tempCoord.x / terr.terrainData.size.x;
            coord.y = tempCoord.y / terr.terrainData.size.y;
            coord.z = tempCoord.z / terr.terrainData.size.z;

            // get the position of the terrain heightmap where this game object is
            posXInTerrain = (int)(coord.x * hmWidth);
            posYInTerrain = (int)(coord.z * hmHeight);

            // we set an offset so that all the raising terrain is under this game object
            int offsetx = width / 2;
            int offsety = height / 2;

            // get the heights of the terrain under this game object
            float[,] heights = terr.terrainData.GetHeights(posXInTerrain - offsetx, posYInTerrain - offsety, width, height);
            bool isChanged = false;
            // we set each sample of the terrain in the size to the desired height
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    float newHeight = (chunk[i, j] - (averageHeight - coord.y));
                    if (Mathf.Abs(heights[i, j] - newHeight)>0.0005f)
                    {
                            heights[i, j] = newHeight; // * (k+1)/heightFrames;
                       isChanged = true;
                    }
                }


            // go raising the terrain slowly
            //if (desiredHeight < coord.y)
            //    desiredHeight += Time.deltaTime / 20;
            //else desiredHeight -= Time.deltaTime / 20;


            // set the new height
            if (isChanged&& (Vector3.SqrMagnitude(transform.position - player.position) < Mathf.Pow(sightRange * transform.localScale.x, 2)))
            {
                terr.terrainData.SetHeights(posXInTerrain - offsetx, posYInTerrain - offsety, heights);
                    coRoutined = true;

            }
                yield return new WaitForSeconds(Time.deltaTime);

            }

        }

    }

    void ClipHeight()
    {

        Vector3 tempCoord = (transform.position - terr.gameObject.transform.position);
        Vector3 terrainCoord;
        terrainCoord.x = tempCoord.x / terr.terrainData.size.x;
        terrainCoord.y = tempCoord.y / terr.terrainData.size.y;
        terrainCoord.z = tempCoord.z / terr.terrainData.size.z;

        int posXInTerrain = (int)(terrainCoord.x * terr.terrainData.heightmapWidth);
        int posYInTerrain = (int)(terrainCoord.z * terr.terrainData.heightmapHeight);
        float height = terr.terrainData.GetHeights(posXInTerrain, posYInTerrain, 1, 1)[0, 0];

        if (height - terrainCoord.y > 0.0027f)
        {
            var newPos = transform.position;
            newPos.y = height * terr.terrainData.size.y + transform.localScale.x;
            transform.position = newPos;
        }
    }


    public override void RefreshStats()     // Rock
    {
        base.RefreshStats();
        width = Random.Range(1,3) * 1 * ((Moveonterrain.levels[(int)attackElement] + 1) / 10); 
        height = width;
        attackTimerRandomMult = Random.Range(0, 10-Moveonterrain.levels[(int)attackElement]);
        randomDirMult = Random.Range(1f, 2.5f) * 1f * (1 + (1 - Moveonterrain.levels[(int)attackElement] / 10));
        speed = Random.Range(3f, 5f) * 1 * (1 + (Moveonterrain.levels[(int)attackElement] + 1) / 10);
        timerLimit = Random.Range(3f,5f) * .25f * (1 + (1 - Moveonterrain.levels[(int)attackElement] / 10));
        playerDist = Random.Range(20f, 27f) * transform.localScale.x * 1 / (1 + (Moveonterrain.levels[(int)attackElement] + 1) / 10);
        maxHp = Random.Range(75f, 125f) * (CompareTag("Special") ? Random.Range(2, 3) : 1) * 1 * (1 + (Moveonterrain.levels[(int)attackElement] + 1) / 10);
        dmgValue = Random.Range(1f, 2f) * (CompareTag("Special") ? Random.Range(2, 3) : 1) * 1 * (1 + (Moveonterrain.levels[(int)attackElement] + 1) / 10);
        hp = maxHp;
    }


}
