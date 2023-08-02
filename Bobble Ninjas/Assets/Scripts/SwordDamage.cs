using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordDamage : MonoBehaviour
{
    public PlayerAttack attackScript;
    public float damage = 1;

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        attackScript = FindObjectOfType<PlayerAttack>();
    }

    void OnTriggerEnter(Collider other) 
    {
        if(attackScript.isSwinging) 
        {
            TakeDamage takeDmgScript = other.GetComponent<TakeDamage>();

            if(takeDmgScript != null)
            {
                takeDmgScript.Damaged(damage);
            }
        }
    }

}
