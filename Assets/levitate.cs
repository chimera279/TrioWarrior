using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levitate : RPSEnemy
{
    Rigidbody rb;
    public GameObject target,attackObject;
    DoAttacking attackObjectControl;
    Renderer attackObjRenderer;
    float levitateTimer, tempTimer, gravForce,  attackTimer;
    public float playerRange, velMagCap, levitationForce, maxGravForce, attractionForce, repulsionForce, attackTime;
    public int attackTimerRandomMult;
    public bool inAttackRange, isAttacking;
    float velocityCap;
    // Start is called before the first frame update
    void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        levitateTimer = Random.value;
        velocityCap = 90;
        rb = GetComponent<Rigidbody>();
        attackElement = Attacks.AtkDefElement.Paper;
        attackType = (Attacks.AttackType)Random.Range(0, 3);
        defenseType = (DefenseType)(Random.Range(0, 3));
        attackTimer = 0;
        this.RefreshAttackTimers();
        attackTime = Random.Range(attackTimeMin, attackTimeMax);
     //   attackObject = transform.Find("AttackObject").gameObject;
        attackObjectControl = attackObject.GetComponent<DoAttacking>();
        attackObjRenderer = attackObject.GetComponent<Renderer>();
        sightRange = Random.Range(200f, 350f);
        isAlive = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (tempTimer > levitateTimer)
        {
            tempTimer = 0;
            levitateTimer = Random.value;
            if (transform.position.y < 2600)
            {
                rb.AddForce(Vector3.up * levitationForce * Time.deltaTime, ForceMode.Acceleration);
                rb.AddForce(Vector3.up * levitationForce * Time.deltaTime, ForceMode.Acceleration);
            }
            else
            {
                rb.AddForce(-Vector3.up * levitationForce * Time.deltaTime, ForceMode.Acceleration);
                rb.AddForce(-Vector3.up * levitationForce * Time.deltaTime, ForceMode.Acceleration);

            }

            rb.AddForce(Random.onUnitSphere * levitationForce * Time.deltaTime, ForceMode.Acceleration);
            rb.AddForce(Random.onUnitSphere * levitationForce * Time.deltaTime, ForceMode.Acceleration);
                var jumpVel = rb.velocity;
            {
                jumpVel.y = 20 * velMagCap/3;
                jumpVel.z = 20 * velMagCap / 3;
                jumpVel.x = 20 * velMagCap / 3;
                gravForce = 0;
                rb.velocity = jumpVel;
            }
        }
       // rb.AddForce(transform.up *  4f);
        //else if (Random.Range(0, 4) == 0)
        //    rb.AddForce(transform.up / 1f, ForceMode.Impulse);
        if(Vector3.SqrMagnitude(transform.position-target.transform.position)<Mathf.Pow(sightRange * transform.localScale.x,2))
        if (Vector3.SqrMagnitude(transform.position - target.transform.position) > Mathf.Pow(playerRange/2, 2))
            rb.AddForce((target.transform.position - transform.position) * Time.deltaTime * attractionForce, ForceMode.Impulse);
        else
                rb.AddForce(Random.onUnitSphere * levitationForce * Time.deltaTime, ForceMode.Acceleration);
        // rb.AddForce(-(target.transform.position - transform.position) * Time.deltaTime * repulsionForce, ForceMode.Impulse);

        //if (Input.GetKeyDown(KeyCode.C)){
        //    var jumpVel = rb.velocity;
        //    jumpVel.y = 20;
        //    gravForce = 0;
        //    rb.velocity = jumpVel;
        //}
        if (gravForce < maxGravForce) gravForce += Time.deltaTime * 2;
        if (gravForce > maxGravForce) gravForce = 0;
        var tempVel = rb.velocity;
        tempVel.y -= gravForce;
        tempVel.x -= gravForce;
        tempVel.z -= gravForce;
        if (tempVel.y > velocityCap) tempVel.y = velocityCap;
        if (tempVel.z > velocityCap) tempVel.z = velocityCap;
        if (tempVel.x > velocityCap) tempVel.x = velocityCap;
        if (tempVel.magnitude > velMagCap) 
            tempVel = tempVel.normalized * velMagCap;
        rb.velocity = tempVel;
    }

    private void Update()
    {
        tempTimer += Time.deltaTime;
        if (Vector3.SqrMagnitude(transform.position - target.transform.position) > Mathf.Pow(playerRange, 2))
            inAttackRange = false;
        else inAttackRange = true;

        if(inAttackRange)
        {
            attackTimer += Time.deltaTime;
            if(attackTimer> attackTime * 2 && !attackObject.activeSelf)
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
                attackObjRenderer.material.color = new Color(0, 0, 0.75f, 0.75f);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 centroid = new Vector3();
        foreach (var c in collision.contacts)
            centroid += c.normal;
        rb.AddForce(centroid * 60000 * Time.deltaTime, ForceMode.Acceleration);

    }

    

    private void OnCollisionExit(Collision collision)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

   

    public override void RefreshStats()     // Paper
    {
        base.RefreshStats();
        levitationForce = Random.Range(7000,10000) * 4 * (1 + (Moveonterrain.levels[(int)attackElement] + 1) / 10);
        maxGravForce = Random.Range(1f, 1.5f) * 4 * (1 - (Moveonterrain.levels[(int)attackElement] + 1) / 10);
        velMagCap = Random.Range(5, 10) * 2 * (1 + (Moveonterrain.levels[(int)attackElement] + 1) / 10);
        attractionForce = Random.Range(0f, 3f) * (2 + Moveonterrain.levels[(int)attackElement] / 3) * (1 + (Moveonterrain.levels[(int)attackElement] + 1) / 10);
        repulsionForce = Random.Range(0f, 1.5f) * (2 - Moveonterrain.levels[(int)attackElement]/3) * (1 + (Moveonterrain.levels[(int)attackElement] + 1) / 10);
        playerRange = Random.Range(15f, 25f) * transform.localScale.x * 1 * (1 + (Moveonterrain.levels[(int)attackElement] + 1) / 10);
        attackTimerRandomMult = Random.Range(0, 10-Moveonterrain.levels[(int)attackElement]);
        maxHp = Random.Range(75f, 125f) * (CompareTag("Special") ? Random.Range(2, 3) : 1) * 1 * (1 + (Moveonterrain.levels[(int)attackElement] + 1) / 10);
        dmgValue = Random.Range(1f, 2f) * (CompareTag("Special") ? Random.Range(2, 3) : 1) * 1 * (1 + (Moveonterrain.levels[(int)attackElement] + 1) / 10);
        hp = maxHp;
    }
}
