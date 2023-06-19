using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackScript : MonoBehaviour
{
    Animator animator;
    int isAttackHash;
    Movement movementScript;


    void Start()
    {
        animator = GetComponent<Animator>();
        movementScript = GetComponent<Movement>();
        isAttackHash = Animator.StringToHash("isAttack");
    }

    
    void Update()
    {
        Attack();
    }

    void Attack()
    {
        if(Input.GetKey(KeyCode.Mouse0)&&!movementScript.blocking)
        {
           animator.SetBool(isAttackHash, true); 
        }
        else
        {
            animator.SetBool(isAttackHash, false);
        }
        
    }
}
