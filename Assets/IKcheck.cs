using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKcheck : MonoBehaviour
{
    public GameObject lh, rh, lf, rf;
    public Animator anim;
    private void OnAnimatorIK(int layerIndex)
    {
        anim = this.GetComponent<Animator>();
        anim.SetIKPosition(AvatarIKGoal.LeftFoot, lf.transform.position);
        anim.SetIKPosition(AvatarIKGoal.RightFoot, rf.transform.position);
        anim.SetIKPosition(AvatarIKGoal.LeftHand, lh.transform.position);
        anim.SetIKPosition(AvatarIKGoal.RightHand, rh.transform.position);

        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0.5f);
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0.5f);
        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0.5f);
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0.5f);

        anim.SetIKRotation(AvatarIKGoal.LeftFoot, lf.transform.rotation);
        anim.SetIKRotation(AvatarIKGoal.RightFoot, rf.transform.rotation);
        anim.SetIKRotation(AvatarIKGoal.LeftHand, lh.transform.rotation);
        anim.SetIKRotation(AvatarIKGoal.RightHand, rh.transform.rotation);

        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0.5f);
        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0.5f);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0.5f);
        anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 0.5f);
    }
}