using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable 
{

    void DoDamage(float damage, Attacks.AttackType atkType);
    void DoDefense(int defenseMode, Transform source);

}
