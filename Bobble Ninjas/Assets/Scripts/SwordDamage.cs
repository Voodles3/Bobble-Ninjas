using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordDamage : MonoBehaviour
{
    public PlayerAttack attackScript;
    public float damage = 1;

    void FixedUpdate()
    {
        attackScript = FindObjectOfType<PlayerAttack>();
    }

    void OnTriggerEnter(Collider other) 
    {
        if(attackScript.isSwinging) 
        {
            EnemyDamaged enemyDmgScript = other.GetComponent<EnemyDamaged>();

            if(enemyDmgScript != null)
            {
                enemyDmgScript.Damaged(damage);
            }
        }
    }

}
