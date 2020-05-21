using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Moveonterrain : RPSEnemy
    {
        public Vector3 dir,rightDir, centroid,camForward,camRight, terrainCoord, defaultCamBounds ;
    Quaternion camRotation;
        Transform player, lockonTarget;
        Camera mainCam;
    Transform cameraBounds;
   // followPlayer mainCamFollow;
        Rigidbody rb;
        Animator playerAnim;
    Terrain terr, manipulatorTerrain;
    public Texture2D map;
    public Light miningLight;
    public GameObject attackObject;
    Renderer attackObjRenderer, defenseMatRenderer;
    public ParticleSystem defenseParts;
    public DoAttacking attackObjectControl;
        public float speed, velocityClamp, velMagger, flyFactor, rockSpeed, gravFactor, attacksCooldown, maxGravity, flyingLimit, flySpeed, jumpingTime, regenRate;
        public bool isGrounded, isAttacking, lockOn;
    float jumpedTimer;
    bool isJumping;
    float[,] chunk;
    float averageHeight;
    float attackTimer;
    int width, height;
    public float[,] AttackRanges,AttackTimers, AttackDmgMults, AttackSpeeds, XPGainAmt;
    public float[] XP,XPLevelThreshold;
    public static int[] levels;
    public static int totalLevel;
    

        // Start is called before the first frame update
    void Start()
        {
        Application.targetFrameRate = 50;

        mainCam = Camera.main;
        cameraBounds = mainCam.transform.parent;
        defaultCamBounds = cameraBounds.transform.localPosition - transform.forward;
        camRotation = mainCam.transform.localRotation;
     //   mainCamFollow = mainCam.GetComponent<followPlayer>();
            player = GameObject.FindGameObjectWithTag("PlayerModel").transform;
            isGrounded = false;
            playerAnim = player.GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            terr = GameObject.FindGameObjectWithTag("MainTerrain").GetComponent<Terrain>();
        manipulatorTerrain = GameObject.FindGameObjectWithTag("ManipulationTerrain").GetComponent<Terrain>();
        SetTerrainCoordinates();
        jumpedTimer = 0;
        flyFactor = flySpeed;
        isJumping = false;
        width = 3;
        height = 3;
        attackTimer = 0;
        isAttacking = false;
        attackObjRenderer = attackObject.GetComponent<Renderer>();
        defenseMatRenderer = GameObject.FindGameObjectWithTag("PlayerMat").GetComponent<Renderer>();
        attackObjectControl = attackObject.GetComponent<DoAttacking>();
        attackObjectControl.source = this.transform;
        maxHp = 279;
        hp = maxHp;
        maxDmg = 2.7f;
        dmgValue = maxDmg;
        isAlive = true;

        lockOn = false;
        lockonTarget = null;
        AttackRanges = new float[3, 3] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };                // How far it goes
        AttackTimers = new float[3, 3] { { 1, 1, 1 }, { .5f, .75f, .45f }, { 1.5f, 1.45f, 1.65f } };        // How soon it can be triggered
        AttackDmgMults = new float[3, 3] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };                         // How much it does
        AttackSpeeds = new float[3, 3] { { 2, 2, 2 }, { 3, 3, 3 }, { .5f, .5f, .5f } };                  // How fast it moves
        XPGainAmt = new float[3, 3] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
        XP = new float[3] { 0, 0, 0 };
        XPLevelThreshold = new float[3] { 100, 100, 100 };
        levels = new int[3] {0,0,0};
        totalLevel = 0;
        miningLight.color = new Color(248f / 255f, 211f / 255f, 167f / 255f);
        defenseParts.Play();
    }

    // Update is called once per frame
    void Update()
        {
        //playerAnim.SetBool("OnGround", isGrounded);
        var newForward = transform.forward;
        newForward.x = dir.normalized.x;
        newForward.z = dir.normalized.z;
        camForward = Vector3.Scale(mainCam.transform.forward, dir.normalized).normalized;
      //  dir = Vector3.Scale(dir.normalized,mainCam.transform.forward).normalized;
        camRight = Vector3.Scale(mainCam.transform.right, Vector3.one - centroid).normalized;
        // transform.forward = newForward;
       // transform.forward = dir.normalized;
       // transform.right = rightDir.normalized;
            player.position = transform.position - transform.up * 0.154f;
        var rots = transform.rotation;
        rots.y = player.transform.rotation.y;
      //  transform.rotation = rots;
        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0) * 9f) ;
            player.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0,0,0));
        mainCam.GetComponent<Rigidbody>().velocity = -rb.velocity/3;
            velMagger = rb.velocity.magnitude;

        SetTerrainCoordinates();
        if (!rb.isKinematic)                                            // Clip Player Height if too low in terrain. Formerly pushed him to nearest grid space, but once map got bigger, so did displacement
        {
            int posXInTerrain = (int)(terrainCoord.x * terr.terrainData.heightmapWidth);
            int posYInTerrain = (int)(terrainCoord.z * terr.terrainData.heightmapHeight);
            float height = manipulatorTerrain.terrainData.GetHeights(posXInTerrain, posYInTerrain, 1, 1)[0, 0];
            float baseheight = terr.terrainData.GetHeights(posXInTerrain, posYInTerrain, 1, 1)[0, 0];
            float slope = manipulatorTerrain.terrainData.GetSteepness(posXInTerrain / terr.terrainData.size.x, posYInTerrain / terr.terrainData.size.z);
            if (height - terrainCoord.y > 0.003f && !isGrounded)
            {
                var newPos = transform.position;

                rb.velocity = Vector3.zero;
                //newPos.y = height * terr.terrainData.size.y;
                //transform.position = newPos;

                //for (int i = -1; i < 2; i++)
                //{
                //    for (int j = -1; j < 2; j++)
                //    {
                //        float neighborHeight = terr.terrainData.GetHeights(posXInTerrain + i, posYInTerrain + j, 1, 1)[0, 0];
                //        if (/*neighborHeight - terrainCoord.y > 0.0027f&&*/ neighborHeight - terrainCoord.y < .002f)
                //        {
                //            posXInTerrain += i;
                //            posYInTerrain += j;
                //            if(neighborHeight>terrainCoord.y)
                //                newPos.y = neighborHeight * terr.terrainData.size.y + 1f;
                //            break;
                //        }

                //    }
                //}
                //newPos.x = posXInTerrain;
                //newPos.z = posYInTerrain;
                if (height - terrainCoord.y > 0.003f /*&& slope < 80*/)
                    newPos.y = height * terr.terrainData.size.y + 0.05f;
                transform.position = newPos;
          //      Debug.Log(height);
                gravFactor = 0;
                rb.AddForce(dir * Time.deltaTime * 2700f);
            }
            if (terrainCoord.y - baseheight > 0.1f)
                isGrounded = false;
            
        }

        if (!isGrounded)
        {
            mainCam.GetComponent<Rigidbody>().isKinematic = true;
            cameraBounds.localPosition = defaultCamBounds;
            mainCam.transform.localRotation = camRotation;
            if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
            {
                ClampVelocity(velocityClamp * flyFactor * gravFactor);
            }
            else
            {
                ClampVelocity(velocityClamp * flyFactor);
            }

            transform.Rotate(-Input.GetAxis("Mouse Y") * 2.79f, 0, 0);
            dir = transform.forward;
            rightDir = transform.right;
            flyFactor -= Time.deltaTime/flyingLimit;
            flyFactor = Mathf.Clamp(flyFactor, 0f, flyFactor);
            //  player.rotation = transform.rotation;
        }
        else
        {
          //  mainCam.GetComponent<Rigidbody>().isKinematic = false;
            mainCam.transform.LookAt(player);
            cameraBounds.transform.Translate(Vector3.up * -Input.GetAxis("Mouse Y") * .72f) ;
            cameraBounds.transform.localPosition = -(Vector3.zero - cameraBounds.transform.localPosition).normalized * 72f;
            flyFactor = 0f;
            ClampVelocity(velocityClamp);
        }

        if (isJumping)
        {
            jumpedTimer += Time.deltaTime;
            ClampVelocity(velocityClamp * flyFactor * 5);
            gravFactor = 0;
            if (jumpedTimer > jumpingTime)
            {
                isJumping = false;
             //   isGrounded = false;
                flyFactor = flySpeed;
                gravFactor = maxGravity;
            }
        }
        else gravFactor = maxGravity;
        //  UpdateMiningLight();
        hp += Time.deltaTime * regenRate;
        if (hp > maxHp)
            hp = maxHp;
        if(hp<0)
        {
            StartCoroutine(KillMe());
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

        }
        if (!isAlive)
        {
            hp = maxHp;
            transform.position = new Vector3(Random.Range(1000f, 7192f), 2709f, Random.Range(1000f, 7192f));
            transform.rotation = Quaternion.identity;
            flyFactor = flySpeed;
            isAlive = true;
        }
        
        DoAttackControls();
        DoAnimatorControls();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if(Input.GetKeyDown("joystick button 9"))
        {
            lockOn = !lockOn;
            var enemyList = EnemyManager.Instance.enemyList;
            if (enemyList.Count >= 1)
            {
                var closest = enemyList[0].transform;
                var closestDist = Vector3.Distance(transform.position, enemyList[0].transform.position);
                for (int i = 0; i < enemyList.Count; i++)
                {
                    var dist = Vector3.Distance(transform.position, enemyList[i].transform.position);
                    if (dist<closestDist)
                    {
                        closest = enemyList[i].transform;
                        closestDist = dist;
                    }
                }
                lockonTarget = closest;
            }
        }
        if(lockOn)
        {
            DoLockOn();
        }
        int tempTotalLevel = 0;
        for (int i = 0; i < 3; i++)
        {
            tempTotalLevel += levels[i];
        }
        totalLevel = tempTotalLevel;
    }

    private void FixedUpdate()
        {
            if (isGrounded)
            {
               // rb.AddForce(dir.normalized * ((centroid==Vector3.up)?playerAnim.GetFloat("Forward"):1) * ((Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0) ? 1 : 0) * Time.deltaTime * speed);
           //     if (Input.GetKeyDown(KeyCode.Space))
            
            rb.velocity = (rb.velocity.magnitude - Time.deltaTime * 20) * rb.velocity.normalized;       // deceleration
            // rb.AddForce(transform.right *  * Time.deltaTime * speed);
            //  rb.AddForce(Vector3.Cross(transform.forward, -transform.right) * rb.mass * 9.81f);

            }
            else
            {
                rb.AddForce(-Vector3.up * 279f * ((Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)?gravFactor:1)
                    / (rb.velocity.magnitude+1) );
            //var rot = transform.rotation;
            //rot.x = 0;
            //rot.z = 0;
            //rb.MoveRotation(rot);
            }

            if (Input.GetAxisRaw("Jump")>0 && !isJumping)
            {
                //   keyHeld += Time.deltaTime;
             //   rb.isKinematic = true;

                //      if (Physics.OverlapSphere(transform.position, .25f,1<<LayerMask.NameToLayer("Terrain")).Length == 0)
                //    {
                CreateChunk();
                ManipulateTerrain();
                Batcher.AddToBatch(this.gameObject, transform.position); 
            flyFactor = flySpeed;
            rb.velocity = Vector3.zero;
            rb.AddForce(transform.up * Time.deltaTime * 50 * rockSpeed, ForceMode.Impulse);
            jumpedTimer = 0;
            isJumping = true;

            //     }
        }
            //if (!Input.GetKey(KeyCode.Space))
            //{
            //    rb.isKinematic = false;
            //}

            if (Input.GetKeyUp(KeyCode.Space)&&!isJumping)
            {
                
            }

            
            //if(Input.GetKey(KeyCode.Space))
            //    transform.Translate((transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal")) * Time.deltaTime * rockSpeed, Space.World);


        
        if (Input.GetKeyUp(KeyCode.Space))
        {
            rb.isKinematic = false;
        }


        
            //if (Input.GetKey(KeyCode.A))
            //    rb.AddForce(-rightDir * Time.deltaTime * speed * (isGrounded ? 1 : (flyFactor > 0 ? flyFactor : 0)));
            //if (Input.GetKey(KeyCode.D))
            //    rb.AddForce(rightDir * Time.deltaTime * speed * (isGrounded ? 1 : (flyFactor > 0 ? flyFactor : 0)));
            //if (Input.GetKey(KeyCode.W))
            //    rb.AddForce(dir * Time.deltaTime * speed * (isGrounded ? 1 : (flyFactor > 0 ? flyFactor : 0)));
            //if (Input.GetKey(KeyCode.S))
            //    rb.AddForce(-dir * Time.deltaTime * speed * (isGrounded ? 1 : (flyFactor > 0 ? flyFactor : 0)));

            rb.AddForce((dir*Input.GetAxis("Vertical") + rightDir * Input.GetAxis("Horizontal")) * Time.deltaTime * speed * (isGrounded ? 1 : (flyFactor > 0 ? flyFactor : 0)));
        // rb.AddForce(-transform.up * rb.mass * 7f);

        if (isJumping)
            rb.AddForce(transform.up * Time.deltaTime * 15 * rockSpeed, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var rot = transform.rotation;
          rot.x = 0;
          rot.z = 0;
        //rot.y = -mainCam.transform.forward.y;
        transform.rotation = rot;
        rb.velocity = rb.velocity.normalized * 0f;
        rb.angularVelocity = rb.angularVelocity.normalized * 0f;
    
        //mainCam.transform.position = transform.position + Quaternion.Euler(0,0,0) * new Vector3(0, 3, -3.5f);
        //mainCam.transform.LookAt(player.transform);
    }

    private void OnCollisionStay(Collision collision)
        {

            isGrounded = true;
            centroid = Vector3.zero;
        Vector3 contacts = new Vector3();
            foreach (ContactPoint contact in collision.contacts)
            {
            contacts += contact.point;
                centroid += contact.normal;
             //   Debug.DrawRay(contact.point, contact.normal, Color.red, 20f);
            }
            dir = Vector3.Cross(centroid, -transform.right);
            rightDir = Vector3.Cross(centroid, transform.forward);
        
        if (collision.contactCount == 2)
        {
        //    rb.AddForce(dir * Time.deltaTime * speed / 2);            Very Very Old code. Mess is there for mess' sake
        }
     //   if(collision.contactCount>1)

          //  ClampVelocity();
        // dir = centroid;
    }

        private void OnCollisionExit(Collision collision)
        {
            isGrounded = false;
        rb.AddForce(-dir * Time.deltaTime * speed / 2);
        dir = transform.forward;
     //   mainCam.transform.LookAt(player);
    }
        void ClampVelocity(float velClamp)
        {
            var vel = rb.velocity;
            var velmag = rb.velocity.magnitude;
            velmag = Mathf.Clamp(velmag, 0, velClamp);
        if (velmag < 0.2f)
            velmag = 0;
            rb.velocity = vel.normalized * velmag;
    }

    void SetTerrainCoordinates()                // Basically normalize transform position to terrain coordinates 
    {
        Vector3 tempCoord = (transform.position - terr.gameObject.transform.position);

        terrainCoord.x = tempCoord.x / terr.terrainData.size.x;
        terrainCoord.y = tempCoord.y / terr.terrainData.size.y;
        terrainCoord.z = tempCoord.z / terr.terrainData.size.z;

    }

    void ManipulateTerrain()                    // Jump function. Async from normal batching cause it's a one shot call that's gotta happen on jump press
    {
        SetTerrainCoordinates();
        // get the position of the terrain heightmap where this game object is
        int posXInTerrain = (int)(terrainCoord.x * terr.terrainData.heightmapWidth);
        int posYInTerrain = (int)(terrainCoord.z * terr.terrainData.heightmapHeight);

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
                float newHeight = (chunk[i, j]); //- (averageHeight - terrainCoord.y));
                if (heights[i, j] != newHeight)
                {
                    heights[i, j] = newHeight-0.01f;
                    isChanged = true;
                }
            }


        // go raising the terrain slowly
        //if (desiredHeight < coord.y)
        //    desiredHeight += Time.deltaTime / 20;
        //else desiredHeight -= Time.deltaTime / 20;


        // set the new height
        if (isChanged)
        {
            manipulatorTerrain.terrainData.SetHeights(posXInTerrain - offsetx, posYInTerrain - offsety, heights);

        }

    }

    void CreateChunk()
    {
      //  width = Random.Range(10, 20);
     //   height = width;
        //  activeMap = Random.Range(0, Maps.Count);
        chunk = new float[width, height];
        averageHeight = 0;
        Vector2Int startPos = new Vector2Int(Random.Range(0, 501), Random.Range(0, 501));
        //   positionPoint = new Vector2Int(Random.Range(0, Maps[activeMap].width/2), Random.Range(0, Maps[activeMap].height/2));
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
             //     chunk[i, j] = map.GetPixel(startPos.x + i, startPos.y + j).r;
                chunk[i, j] = terrainCoord.y + 0.0001f;
                averageHeight += chunk[i, j];
            }
        averageHeight /= (width * height);
        averageHeight -= terrainCoord.y;
    }

    void UpdateMiningLight()
    {
        Vector3 tempPlayerCoord = player.transform.position - terr.gameObject.transform.position;
        Vector3 playerCoord;
        playerCoord.x = tempPlayerCoord.x / terr.terrainData.size.x;
        playerCoord.y = tempPlayerCoord.y / terr.terrainData.size.y;
        playerCoord.z = tempPlayerCoord.z / terr.terrainData.size.z;

        int playerX = (int)(playerCoord.x * terr.terrainData.heightmapWidth);
        int playerY = (int)(playerCoord.z * terr.terrainData.heightmapHeight);
        Color miningColor = new Color(248f / 255f, 211f / 255f, 167f / 255f);
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (CreateTerrain.alphaMaps[playerX + i, playerY + j, 10] > 0)                             // Custom colors I picked on testing
                {
                    miningColor = new Color(255f / 255f, 0 / 255f, 15f / 255f);
                    if(i==0&&j==0)
                    break;
                }
                else
                if (CreateTerrain.alphaMaps[playerX + i, playerY + j, 11] > 0)
                {
                    miningColor = new Color(0 / 255f, 94f / 255f, 255f / 255f);
                    if (i == 0 && j == 0)
                        break;
                }
                else
                if (CreateTerrain.alphaMaps[playerX + i, playerY + j, 12] > 0)
                {
                    miningColor = new Color(255f / 255f, 205f / 255f, 0 / 255f);
                    if (i == 0 && j == 0)
                        break;
                }
                

            }
        }
        miningLight.color = miningColor;

    }

    void DoAttackControls()
    {
        if (!isAttacking)
        {
            attackObject.transform.localPosition = Vector3.zero + Vector3.up * 0.375f;
            attackObject.transform.localScale = new Vector3(1, 0.5f, 1);
            if (!Input.GetKey("joystick button 4") && !Input.GetKey("joystick button 5"))            //if (!Input.GetKey(KeyCode.Alpha1) && !Input.GetKey(KeyCode.Alpha2) && !Input.GetKey(KeyCode.LeftShift))
            {
                if (/*Input.GetMouseButtonDown(0)*/Input.GetAxis("Fire1")>0)
                {

                    attackObjRenderer.material.color = new Color(0.75f, 0, 0, 0.5f);
                    StartCoroutine(Attacks.PerformAttack(Attacks.AttackType.Melee, 0, attackObject, this.gameObject, attacksCooldown,this));
                    attackObjectControl.attackType = Attacks.AttackType.Melee;
                    attackObjectControl.attackElement = Attacks.AtkDefElement.Rock;
                    isAttacking = true;
                }
                if (/*Input.GetMouseButtonDown(1)*/Input.GetAxis("Fire2") > 0)
                {
                    attackObjRenderer.material.color = new Color(0, 0, 0.75f, 0.5f);
                    StartCoroutine(Attacks.PerformAttack(Attacks.AttackType.Melee, 1, attackObject, this.gameObject, attacksCooldown,this));
                    attackObjectControl.attackType = Attacks.AttackType.Melee;
                    attackObjectControl.attackElement = Attacks.AtkDefElement.Paper;
                    isAttacking = true;
                }
                if (/*Input.GetMouseButtonDown(2)*/Input.GetAxis("Fire3") > 0)
                {
                    attackObjRenderer.material.color = new Color(0.75f, 0.75f, 0, 0.5f);
                    StartCoroutine(Attacks.PerformAttack(Attacks.AttackType.Melee, 2, attackObject, this.gameObject, attacksCooldown, this));
                    attackObjectControl.attackType = Attacks.AttackType.Melee;
                    attackObjectControl.attackElement = Attacks.AtkDefElement.Scissors;
                    isAttacking = true;
                }
            }
            else if (!Input.GetKey("joystick button 4") && Input.GetKey("joystick button 5"))
            {
                if (Input.GetAxis("Fire1") > 0)
                {

                    attackObjRenderer.material.color = new Color(0.75f, 0, 0, 1f);
                    StartCoroutine(Attacks.PerformAttack(Attacks.AttackType.Ranged, 0, attackObject, this.gameObject, attacksCooldown, this));
                    attackObjectControl.attackType = Attacks.AttackType.Ranged;
                    attackObjectControl.attackElement = Attacks.AtkDefElement.Rock;
                    isAttacking = true;
                }
                if (Input.GetAxis("Fire2") > 0)
                {
                    attackObjRenderer.material.color = new Color(0, 0, 0.75f, 1f);
                    StartCoroutine(Attacks.PerformAttack(Attacks.AttackType.Ranged, 1, attackObject, this.gameObject, attacksCooldown, this));
                    attackObjectControl.attackType = Attacks.AttackType.Ranged;
                    attackObjectControl.attackElement = Attacks.AtkDefElement.Paper;
                    isAttacking = true;
                }
                if (Input.GetAxis("Fire3") > 0)
                {
                    attackObjRenderer.material.color = new Color(0.75f, 0.75f, 0, 1f);
                    StartCoroutine(Attacks.PerformAttack(Attacks.AttackType.Ranged, 2, attackObject, this.gameObject, attacksCooldown, this));
                    attackObjectControl.attackType = Attacks.AttackType.Ranged;
                    attackObjectControl.attackElement = Attacks.AtkDefElement.Scissors;
                    isAttacking = true;
                }
            }

            else if(Input.GetKey("joystick button 4") && !Input.GetKey("joystick button 5"))
            {
                if (Input.GetAxis("Fire1") > 0)                                    //Defense Mechanism
                {
                    attackElement = Attacks.AtkDefElement.Rock;
                    defenseMatRenderer.material.SetColor("_RimColor", Color.red);
                    var main = defenseParts.main;
                    main.startColor = Color.red;
                   // miningLight.color = new Color(255f / 255f, 0 / 255f, 15f / 255f); 
                }
                if (Input.GetAxis("Fire2") > 0)
                {
                    attackElement = Attacks.AtkDefElement.Paper;
                    defenseMatRenderer.material.SetColor("_RimColor", Color.blue);
                    var main = defenseParts.main;
                    main.startColor = Color.blue;
                   // miningLight.color = new Color(0 / 255f, 94f / 255f, 255f / 255f);
                }
                if (Input.GetAxis("Fire3") > 0)
                {
                    attackElement = Attacks.AtkDefElement.Scissors;
                    defenseMatRenderer.material.SetColor("_RimColor", Color.yellow);
                    var main = defenseParts.main;
                    main.startColor = Color.yellow;
                  //  miningLight.color = new Color(255f / 255f, 205f / 255f, 0 / 255f);
                }

            }

            else if(Input.GetKey("joystick button 4") && Input.GetKey("joystick button 5"))
            {
                    if (Input.GetAxis("Fire1") > 0)
                    {

                        attackObjRenderer.material.color = new Color(0.75f, 0, 0, 0.75f);
                        StartCoroutine(Attacks.PerformAttack(Attacks.AttackType.AoE, 0, attackObject, this.gameObject, attacksCooldown, this));
                        attackObjectControl.attackType = Attacks.AttackType.AoE;
                        attackObjectControl.attackElement = Attacks.AtkDefElement.Rock;
                        isAttacking = true;
                    }
                    if (Input.GetAxis("Fire2") > 0)
                    {
                        attackObjRenderer.material.color = new Color(0, 0, 0.75f, 0.75f);
                        StartCoroutine(Attacks.PerformAttack(Attacks.AttackType.AoE, 1, attackObject, this.gameObject, attacksCooldown, this));
                        attackObjectControl.attackType = Attacks.AttackType.AoE;
                        attackObjectControl.attackElement = Attacks.AtkDefElement.Paper;
                        isAttacking = true;
                    }
                    if (Input.GetAxis("Fire3") > 0)
                    {
                        attackObjRenderer.material.color = new Color(0.75f, 0.75f, 0, 0.75f);
                        StartCoroutine(Attacks.PerformAttack(Attacks.AttackType.AoE, 2, attackObject, this.gameObject, attacksCooldown, this));
                        attackObjectControl.attackType = Attacks.AttackType.AoE;
                        attackObjectControl.attackElement = Attacks.AtkDefElement.Scissors;
                        isAttacking = true;
                    }
            }

            
        }
        if (isAttacking)
            attackTimer += Time.deltaTime;
        if(!attackObject.activeSelf) //attackTimer>attacksCooldown)
        {
            attackTimer = 0;
            isAttacking = false;
        }


    }

    public void CalculateXPDamage(int XPMult, Attacks.AtkDefElement affectedElement,Attacks.AttackType atkType)
    {
        XP[(int)affectedElement] += XPGainAmt[(int)atkType, (int)affectedElement] * XPMult;
        for (int i = 0; i < 3; i++)
            Mathf.Clamp(XP[i], 0, XPLevelThreshold[i] + 27);

    }

    void DoAnimatorControls()
    {
        playerAnim.SetBool("isGrounded", isGrounded);
        playerAnim.SetBool("isJumping", isJumping);
        playerAnim.SetBool("isAttacking", isAttacking);
        if(isAttacking)
        {
            playerAnim.SetInteger("AttackType", (int)attackObjectControl.attackType);
            playerAnim.SetFloat("attackTime", attackTimer / (attacksCooldown * AttackTimers[(int)attackObjectControl.attackType, (int)attackObjectControl.attackElement]));
        }
        playerAnim.SetFloat("horizontal", Input.GetAxis("Horizontal"));
        playerAnim.SetFloat("vertical", Input.GetAxis("Vertical"));
        
    }

    void DoLockOn()
    {
        if (Mathf.Abs(Input.GetAxisRaw("HorizontalTurn")) ==1f || Mathf.Abs(Input.GetAxisRaw("VerticalTurn")) ==1f)
            lockonTarget = EnemyManager.Instance.enemyList[Random.Range(0, EnemyManager.Instance.enemyList.Count-1)].transform;
        transform.LookAt(lockonTarget);

    }

    IEnumerator KillMe()
    {
        float timer = 0;
        DeathAnim.timer = 0;
        DeathAnim.deathElement = attackElement;
        while (timer < 0.5f)
        {
            DeathAnim.isActive = true;
            timer += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        if (levels[(int)attackElement] != 0)
        {
            XP[(int)attackElement] -= 15f;
            if (XP[(int)attackElement] < 0)
            {
                levels[(int)attackElement]--;
                XPLevelThreshold[(int)attackElement] = 100 + levels[(int)attackElement] * 50;
                XP[(int)attackElement] = 3 * XPLevelThreshold[(int)attackElement] / 4;
            }
        }
        DeathAnim.isActive = false;
        isAlive = false;
        yield return new WaitForEndOfFrame();
    }


}

