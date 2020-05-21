using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScissorMove : RPSEnemy
{
    public Transform target;
    public GameObject attackObject;
    DoAttacking attackObjectControl;
    Renderer attackObjRenderer;
    float lungeValue, playerDist, timeToLunge,  speed, lungeTimeMilliseconds,  timeToWander, lungingTime;
    public int attackTimerRandomMult;
    public bool isLunging, isGrounded, isWandering,inAttackRange;

    public float lungeTimer,lungingTimer,  wanderTimer,  attackTimer, attackTime;
    Rigidbody rb;
    public Vector3 centroid,fwdDir, terrainCoord, randomDir;
    Terrain terr, manipTerr;
    // Start is called before the first frame update
    void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        terr = GameObject.FindGameObjectWithTag("MainTerrain").GetComponent<Terrain>();
        manipTerr = GameObject.FindGameObjectWithTag("ManipulationTerrain").GetComponent<Terrain>();
        rb = GetComponent<Rigidbody>();
        lungeTimer = 0;
        lungingTimer = 0;
        wanderTimer = 0;
        randomDir = Random.onUnitSphere.normalized;
        attackElement = Attacks.AtkDefElement.Scissors;
        attackType = (Attacks.AttackType)Random.Range(0, 3);
        defenseType = (DefenseType)(Random.Range(0, 3));
        attackTimer = 0;
        this.RefreshAttackTimers();
        attackTime = Random.Range(attackTimeMin, attackTimeMax);
        //  attackObject = transform.Find("AttackObject").gameObject;
        attackObjectControl = attackObject.GetComponent<DoAttacking>();
        attackObjRenderer = attackObject.GetComponent<Renderer>();
        sightRange = Random.Range(300f, 450f);
        isAlive = true;
    }

    // Update is called once per frame
    void Update()
    {
        //    if (Input.GetKey(KeyCode.Space))
        //        isLunging = true;
        //    else isLunging = false;
        CheckTerrainPosition();
        if (isGrounded)
        {
            if (!isWandering)
                transform.LookAt(target);
            else transform.LookAt(randomDir);
            if (!isLunging)
            {
                lungeTimer+=Time.deltaTime;
                if (lungeTimer > timeToLunge)
                {
                    isLunging = true;
                    lungeTimer = 0;
                }
               
            }

        }

        if(isLunging)
        {
            lungingTimer += Time.deltaTime;
            if (lungingTimer > lungingTime)
            {
                lungingTimer = 0;
                isLunging = false;
            }
        }

        if (Vector3.SqrMagnitude(target.position - transform.position) > Mathf.Pow(sightRange * 2, 2))
            isWandering = true;
        else isWandering = false;

        if (Vector3.SqrMagnitude(target.position - transform.position) < Mathf.Pow(playerDist, 2) && !isWandering)
            inAttackRange = true;
        else inAttackRange = false;


        if (isWandering)
        {
            wanderTimer+=Time.deltaTime;
            if(wanderTimer>timeToWander)
            {
                wanderTimer = 0;
                randomDir = Random.onUnitSphere.normalized;
            }
        }

        if(inAttackRange)
        {
            CheckTextureMap();
            attackTimer += Time.deltaTime;
            if(attackTimer>attackTime * 2 && !attackObject.activeSelf)
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
                attackObjRenderer.material.color = new Color(0.75f, 0.75f, 0, 0.75f);
            }
        }

            ClipHeight();
    }

    private void FixedUpdate()
    {
        if (isLunging)
        {
            if (Vector3.SqrMagnitude(transform.position - target.position) > Mathf.Pow(playerDist, 2)&&!isWandering&&isLunging)
                rb.AddForce(-(transform.position - target.position) * lungeValue * Vector3.Distance(transform.position, target.position) /
                Mathf.Pow(Time.deltaTime * lungeTimeMilliseconds, 2));
            else isLunging = false;
        }
        else
        {
            if (!isGrounded)
                rb.AddForce(Vector3.down * 9.81f * 1 * (1+(Moveonterrain.levels[(int)attackElement]+1)/10));
            else
            {
                    if (Vector3.SqrMagnitude(target.position - transform.position) > Mathf.Pow(playerDist * 2, 2)&&!isWandering)
                    {
                      //  randomDir = Random.onUnitSphere.normalized;
                        rb.AddForce(Vector3.Scale(-(transform.position - target.position), fwdDir));
                    }
                    else 
                    rb.AddForce(Vector3.Scale(randomDir, fwdDir) * speed /** randomDirMult*/);
                
                
                if (rb.velocity.magnitude > speed)
                    rb.velocity = rb.velocity.normalized * speed;
            }
        }
        
        if(rb.velocity.magnitude>speed * 27)
            rb.velocity = rb.velocity.normalized * speed * 27;
    }

    private void OnCollisionEnter(Collision collision)
    {
        rb.velocity = rb.velocity.normalized * 0.1f;
        rb.angularVelocity = rb.angularVelocity.normalized * 0.1f;
    }

    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
        centroid = Vector3.zero;
        foreach (ContactPoint contact in collision.contacts)
        {
            centroid += contact.normal;
            //   Debug.DrawRay(contact.point, contact.normal, Color.red, 20f);
        }
        Vector3 playerRelative = Vector3.Scale((target.position - transform.position), new Vector3(1, 0, 1));

        fwdDir = Vector3.Scale(Vector3.Cross(centroid, -transform.right),playerRelative);
        transform.forward = fwdDir;
    }

    private void OnCollisionExit(Collision collision)
    {
            isGrounded = false;
    }

    void ClipHeight()
    {
        SetTerrainCoordinates();

        int posXInTerrain = (int)(terrainCoord.x * terr.terrainData.heightmapWidth);
        int posYInTerrain = (int)(terrainCoord.z * terr.terrainData.heightmapHeight);
        float height = terr.terrainData.GetHeights(posXInTerrain, posYInTerrain, 1, 1)[0, 0];
        if (height - terrainCoord.y > 0.027f)
        {
            rb.velocity = Vector3.zero;
            var newPos = transform.position;
            newPos.y = height * terr.terrainData.size.y + 5f;
            transform.position = newPos;
            rb.AddForce(-transform.forward * 100f,ForceMode.Impulse);
        }
        float manipHeight = manipTerr.terrainData.GetHeights(posXInTerrain, posYInTerrain, 1, 1)[0, 0];
        if (manipHeight - terrainCoord.y > 0.0027f)
        {
            rb.velocity = Vector3.zero;
            var newPos = transform.position;
            newPos.y = height * manipTerr.terrainData.size.y + 15f;
            transform.position = newPos;
            rb.AddForce(-Vector3.forward * 30f,ForceMode.Impulse);
        }
    }
    void SetTerrainCoordinates()
    {
        Vector3 tempCoord = (transform.position - terr.gameObject.transform.position);

        terrainCoord.x = tempCoord.x / terr.terrainData.size.x;
        terrainCoord.y = tempCoord.y / terr.terrainData.size.y;
        terrainCoord.z = tempCoord.z / terr.terrainData.size.z;

    }

    void CheckTerrainPosition()
    {
        // get the normalized position of this game object relative to the terrain
        Vector3 tempCoord = (transform.position - terr.gameObject.transform.position);
        Vector3 coord;
        coord.x = tempCoord.x / terr.terrainData.size.x;
        coord.y = tempCoord.y / terr.terrainData.size.y;
        coord.z = tempCoord.z / terr.terrainData.size.z;

        // get the position of the terrain heightmap where this game object is
        int hmWidth = terr.terrainData.heightmapWidth;
        int hmHeight = terr.terrainData.heightmapHeight;

        int posXInTerrain = (int)(coord.x * hmWidth);
        int posYInTerrain = (int)(coord.z * hmHeight);
        bool endCoord = false;

        if (posXInTerrain <= 1 || posXInTerrain >= hmWidth - 2)       // see that it's within bounds of terrain, also considering it's chunk sizes
        {
            if (posXInTerrain <= 1)
                posXInTerrain = hmWidth - 2;
            if (posXInTerrain >= hmWidth - 2)
                posXInTerrain = 2;
            endCoord = true;
        }
        if (posYInTerrain <= 1 || posYInTerrain >= hmWidth - 2)
        {
            if (posYInTerrain <= 1)
                posYInTerrain = 2;
            if (posYInTerrain >= hmWidth - 2)
                posYInTerrain = 2;
            endCoord = true;
        }
        if (endCoord)
            transform.position = new Vector3(posXInTerrain, transform.position.y, posYInTerrain);       // Reset it to warped point on other side of terrain (Rock based AI only)

    }

    void CheckTextureMap()
    {
        Vector3 tempPlayerCoord = transform.position - terr.gameObject.transform.position;
        Vector3 playerCoord;
        playerCoord.x = tempPlayerCoord.x / terr.terrainData.size.x;
        playerCoord.y = tempPlayerCoord.y / terr.terrainData.size.y;
        playerCoord.z = tempPlayerCoord.z / terr.terrainData.size.z;

        int playerX = (int)(playerCoord.x * terr.terrainData.heightmapWidth);
        int playerY = (int)(playerCoord.z * terr.terrainData.heightmapHeight);
        for (int i = 1; i < 2; i += 2)
        {
            for (int j = 1; j < 2; j += 2)
            {
                Vector2 alphamapPos = new Vector2((playerX + i) * terr.terrainData.alphamapWidth / terr.terrainData.heightmapWidth,
                                (playerY + j) * terr.terrainData.alphamapHeight / terr.terrainData.heightmapHeight);
                var newMap = terr.terrainData.GetAlphamaps((int)alphamapPos.x, (int)alphamapPos.y, 1, 1);
                if (newMap[0, 0, 14] == 0)
                {
                    StartCoroutine(TrailManage(new Vector2Int((int)alphamapPos.x, (int)alphamapPos.y), newMap));
                }
            }
        }
    }

    IEnumerator TrailManage(Vector2Int alphamapPos, float[,,] newMap)
    {
        newMap[0, 0, 14] = 1;
        terr.terrainData.SetAlphamaps((int)alphamapPos.x, (int)alphamapPos.y, newMap);
        newMap[0, 0, 14] = 0;
        yield return new WaitForSeconds(1.5f);
        terr.terrainData.SetAlphamaps(alphamapPos.x, alphamapPos.y, newMap);

    }

    public override void RefreshStats()     // Scissor
    {
        base.RefreshStats();
        speed = Random.Range(2f, 3.5f) * 1 * (1 + (Moveonterrain.levels[(int)attackElement] + 1) / 10); 
        lungeTimeMilliseconds = Random.Range(300f, 330f) * 1 *  (1 + (1 - Moveonterrain.levels[(int)attackElement] / 10));
        timeToWander = Random.Range(7f, 9f) * .25f * ( 1 + (1 - Moveonterrain.levels[(int)attackElement] / 10));
        lungingTime = Random.Range(3f, 5f) * .25f * (1 +  (1 + Moveonterrain.levels[(int)attackElement] / 10));
        timeToLunge = Random.Range(2f, 4f) * .25f * (1 + (1 - Moveonterrain.levels[(int)attackElement] / 10));
        lungeValue = Random.Range(1f, 3f) * 0.25f * (1 + (Moveonterrain.levels[(int)attackElement ] + 1) / 10);
        attackTimerRandomMult = Random.Range(0, 10 - Moveonterrain.levels[(int)attackElement]);
        playerDist = Random.Range(10f, 15f) * transform.localScale.x * 1 / (1 +  (Moveonterrain.levels[(int)attackElement] + 1) / 10);
        maxHp = Random.Range(75f, 125f) * (CompareTag("Special") ? Random.Range(2, 3) : 1) * 1 * (1 + (Moveonterrain.levels[(int)attackElement] + 1) / 10);
        dmgValue = Random.Range(1f, 2f) * (CompareTag("Special") ? Random.Range(2, 3) : 1) * 1 * (1 + (Moveonterrain.levels[(int)attackElement] + 1) / 10);
        hp = maxHp;
    }
}
