using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamaged : MonoBehaviour
{
    public float health = 5f;
    Animator animator;
    int isDeadHash;
    private EnemyAI aiScript;
    private EnemyLook lookScript;
    private Collider enemyCollider;
    

    void Start() 
    {
        enemyCollider = GetComponent<CapsuleCollider>();
        animator = GetComponentInChildren<Animator>();
        aiScript = GetComponent<EnemyAI>();
        lookScript = GetComponent<EnemyLook>();
        isDeadHash = Animator.StringToHash("isDead");
    }
    public void Damaged(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            animator.SetBool(isDeadHash, true);
            aiScript.enabled = false;
            lookScript.enabled = false;
            enemyCollider.enabled = false;
        }
    }
}
