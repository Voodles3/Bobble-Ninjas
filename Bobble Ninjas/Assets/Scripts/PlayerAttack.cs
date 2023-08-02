using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    Animator animator;
    int isAttackHash;

    public bool isSwinging = false;


    void Start()
    {
        animator = GetComponent<Animator>();
        isAttackHash = Animator.StringToHash("isAttack");
    }
    
    void Update()
    {
        Attack();
    }

    void Attack()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0)&&!Input.GetKey(KeyCode.Mouse1))
        {
           animator.SetBool(isAttackHash, true); 
        }
        else
        {
            animator.SetBool(isAttackHash, false);
        }
        
    }

    //These are triggered as Animation events in the bobbleninjaplayer Animation "DiagonalSwingTRBL"
    void AllowSwinging()
    {
        //Debug.Log("Is Swinging");
        isSwinging = true;
    }

    void DisallowSwinging()
    {
        //Debug.Log("Is not Swinging");
        isSwinging = false;
    }

}
