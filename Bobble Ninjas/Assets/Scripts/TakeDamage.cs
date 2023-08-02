using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamage : MonoBehaviour
{
    public float health = 5f;
    Animator animator;
    int isDeadHash;
    

    void Start() 
    {
        animator = GetComponentInChildren<Animator>();
        isDeadHash = Animator.StringToHash("isDead");
    }
    public void Damaged(float amount)
    {
        health -= amount;
        if(health<=0)
        {
            //animator.SetBool(isDeadHash, true);
            Destroy(gameObject);
        }
    }
}
