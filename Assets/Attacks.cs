using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Attacks
{
    public enum AttackType { Melee, Ranged, AoE };
    public enum AtkDefElement { Rock, Paper, Scissors };
    public enum DefenseType { Regenerate, Damage, XPSteal };
    public static IEnumerator PerformAttack(AttackType attackType, int attackElement, GameObject attackObject, GameObject attackingObject, float cooldownTimer, Moveonterrain player = null)
    {
        float timer = 0;
        attackObject.SetActive(true);
        int playerX = -2799, playerY = -2799;
        float speedMult =  1, rangeMult = 1,difficultyMult = -1;
        Vector3 dir = attackObject.transform.forward;
        if (player != null)
        {
            playerX = (int)attackType;
            playerY = attackElement;
            cooldownTimer *= player.AttackTimers[playerX, playerY];
            speedMult = player.AttackSpeeds[playerX, playerY];
            rangeMult = player.AttackRanges[playerX, playerY];
            dir = -(Camera.main.transform.position - (player.transform.position + player.transform.up));
            if (Physics.SphereCast(player.transform.position + player.transform.up, 27, dir, out RaycastHit hit, float.MaxValue, 1 << LayerMask.NameToLayer("Enemy")))
            {
                dir = (hit.transform.position - (player.transform.position + player.transform.up));
                Debug.Log(hit.transform.name);
            }
        }

        else
        {
            difficultyMult = Moveonterrain.levels[(int)attackElement] + 1;
        }
        AudioManager.Instance.CreateAudio(attackObject.transform.position, AudioClipManager.attackClips[0, (int)attackType], attackObject.transform);
        while (timer< cooldownTimer)
        {
            switch(attackType)
            {
                case AttackType.Melee:
                    Vector3 scale = attackObject.transform.localScale;
                    scale.x = Mathf.Lerp(0f, 9f * rangeMult, timer / (cooldownTimer)) * speedMult * (difficultyMult >= 0 ? 6 * difficultyMult / 10 : 1);
                    scale.z = Mathf.Lerp(0f, 9f * rangeMult, timer / (cooldownTimer)) * speedMult * (difficultyMult >= 0 ? 6 * difficultyMult / 10 : 1);
                    attackObject.transform.localScale = scale; 
                    break;
                case AttackType.Ranged:
               //     attackObject.transform.Translate(dir.normalized * Time.deltaTime * 90 * speedMult * rangeMult,Space.World);    //pos.z = Mathf.Lerp(1, 80 * rangeMult, timer / Mathf.Pow(cooldownTimer,.2f)) * speedMult;
                    attackObject.transform.Translate(dir.normalized * 1.5f * /*(timer>cooldownTimer/2?-1: 1) * */ timer * ((speedMult / cooldownTimer) + (rangeMult) /
                Mathf.Pow(cooldownTimer, 2)) * (difficultyMult >= 0 ? 12 * difficultyMult / 10 : 1), Space.World);
                    
                    //attackObject.transform.localPosition = pos;
                    attackObject.transform.localScale = new Vector3(2.5f, 1.25f, 4f);
                    break;
                case AttackType.AoE:
                    attackObject.transform.localScale = Vector3.Lerp(new Vector3(0f, 0f, 0f), new Vector3(27, 13.5f, 27) * rangeMult, timer / (cooldownTimer / 1.5f)) * speedMult * (difficultyMult >= 0 ? 2 * difficultyMult / 10 : 1);
                    break;
            }
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        
        attackObject.transform.localPosition = Vector3.zero + Vector3.up * 0.375f;
        attackObject.transform.localScale = new Vector3(1, 0.5f, 1);
        attackObject.SetActive(false);
    }
}
