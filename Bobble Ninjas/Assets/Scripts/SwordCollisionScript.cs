using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollisionScript : MonoBehaviour
{
    public SetSwinging swingScript;
    public float damage = 1;

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        swingScript = FindObjectOfType<SetSwinging>();
    }

    void OnTriggerEnter(Collider other) 
    {
        //Debug.Log("Can collide");
        if(swingScript.isSwinging) 
        {
            //Debug.Log("Collided");
            DamageScript damageScript = other.GetComponent<DamageScript>();
            if(damageScript != null)
            {
                damageScript.Damaged(damage);
            }
        }
    }

}
