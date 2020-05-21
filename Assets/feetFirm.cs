using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class feetFirm : MonoBehaviour
{
    public Moveonterrain ikTarget;
    Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
        ikTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<Moveonterrain>();
    }
    // Start is called before the first frame update
    private void OnAnimatorIK(int layerIndex)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
        {
            anim.SetIKPosition(AvatarIKGoal.RightFoot, transform.position - Vector3.Cross(ikTarget.rightDir.normalized / 2, ikTarget.dir.normalized / 2));
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
            anim.SetIKPosition(AvatarIKGoal.LeftFoot, transform.position - Vector3.Cross(ikTarget.rightDir.normalized / 2, ikTarget.dir.normalized / 2));
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
        }

        //anim.SetLookAtPosition(ikTarget.camForward);
        //anim.SetLookAtWeight(0.5f);
    }   
}
