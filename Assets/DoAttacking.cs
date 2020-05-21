using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoAttacking : MonoBehaviour
{
    public Attacks.AttackType attackType;
    public Attacks.AtkDefElement attackElement;
    public Transform source;
    public Material[] attackMats;
    Material innerMat;
    Renderer attackRend;

    private void Start()
    {
        attackRend = GetComponent<Renderer>();
        attackMats = new Material[3];
        attackMats[0] = Resources.Load<Material>("Materials/Rock");
        attackMats[1] = Resources.Load<Material>("Materials/Paper");
        attackMats[2] = Resources.Load<Material>("Materials/Scissor");
        innerMat = Resources.Load<Material>("Materials/InnerMat");
        attackRend.materials = new Material[2] { attackMats[0], innerMat };
    }
    private void Update()
    {
        if(isActiveAndEnabled)
        {
            attackRend.material = attackMats[(int)attackElement];
            attackRend.materials[1].SetVector("_TintColor", RPSEnemy.RPSColors[(int)attackElement]/1.6f * 1.5f);

        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            var enemyScripts = other.transform.GetComponents<RPSEnemy>();
            RPSEnemy enemyScript = null;
            if (enemyScripts.Length>0)
                enemyScript = enemyScripts[Random.Range(0, enemyScripts.Length)];
            else Debug.Log("POWA");
            if (enemyScript)
            {
                //     Debug.Log(enemyScript.attackElement);
                if (source && source.CompareTag("Player"))
                {
                    var playerScript = source.GetComponent<RPSEnemy>();
                    if (enemyScript.attackElement == (Attacks.AtkDefElement)((int)(attackElement + 1) % 3))
                        enemyScript.DoDefense(1, playerScript,attackElement,attackType);         //  Debug.Log(source + " Made Bad Attack On " + other);
                    else if (attackElement == (Attacks.AtkDefElement)((int)(enemyScript.attackElement + 1) % 3))
                    {
                        enemyScript.DoDefense(0, playerScript, attackElement, attackType);        //Debug.Log(source + " Made Good Attack On " + other);
                        enemyScript.DoDamage(playerScript.dmgValue, attackType, attackElement, playerScript.gameObject.GetComponent<Moveonterrain>());
                    }
                    else if (enemyScript.attackElement == attackElement)
                        enemyScript.DoDamage(playerScript.dmgValue, attackType,attackElement, playerScript.gameObject.GetComponent<Moveonterrain>());     //Debug.Log(source + " Made Neutral Attack On " + other);

                    ParticlesManager.Instance.CreateParticle(attackType, attackElement, other.transform.position);
                    AudioManager.Instance.CreateAudio(other.transform.position, AudioClipManager.attackClips[1, (int)attackType], other.transform);
                }
            }
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            var playerScript = other.GetComponent<RPSEnemy>();
            var enemyScript = source.GetComponent<RPSEnemy>();
            if (playerScript.attackElement == (Attacks.AtkDefElement)((int)(attackElement + 1) % 3))
                enemyScript.DoDefense(0, playerScript, attackElement, attackType);         //  Debug.Log(source + " Made Bad Attack On " + other);
            else if (attackElement == (Attacks.AtkDefElement)((int)(playerScript.attackElement + 1) % 3))
            {
                enemyScript.DoDefense(1, playerScript, attackElement, attackType);
                playerScript.DoDamage(enemyScript.dmgValue, attackType, attackElement);
                //Debug.Log(source + " Made Good Attack On " + other);
            }
            else if (playerScript.attackElement == attackElement)
            {
                
                playerScript.DoDamage(enemyScript.dmgValue, attackType, attackElement);     //Debug.Log(source + " Made Neutral Attack On " + other);
            }
            ParticlesManager.Instance.CreateParticle(attackType, attackElement, other.transform.position);
            AudioManager.Instance.CreateAudio(other.transform.position, AudioClipManager.attackClips[1, (int)attackType], other.transform);

        }

        if(other.gameObject.layer == LayerMask.NameToLayer("LevelUp"))
        {
            if (source && source.CompareTag("Player"))
            {
                var towerScript = other.GetComponentInParent<Tower>();
                var playerScript = source.GetComponent<Moveonterrain>();
                if(playerScript.XP[(int)attackElement]>= playerScript.XPLevelThreshold[(int)attackElement])
                if (towerScript.attackElement == (Attacks.AtkDefElement)((int)(attackElement + 1) % 3))
                    towerScript.PerformLevelUp(playerScript, -1);        //  Debug.Log(source + " Made Bad Attack On " + other);
                else if (attackElement == (Attacks.AtkDefElement)((int)(towerScript.attackElement + 1) % 3))
                    towerScript.PerformLevelUp(playerScript, 1);        //Debug.Log(source + " Made Good Attack On " + other);
                
                else if (towerScript.attackElement == attackElement)
                    towerScript.PerformLevelUp(playerScript, 0);     //Debug.Log(source + " Made Neutral Attack On " + other);
            }
        }

    }
}
