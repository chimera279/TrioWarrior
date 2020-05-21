using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateAMan : MonoBehaviour
{
    float horizontal, vertical;
    Animator anim;
    bool isGrounded;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = transform.forward.x;
        vertical = transform.forward.z;
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("horizontal", horizontal);
        anim.SetFloat("vertical", vertical);
    }

    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}
