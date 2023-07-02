using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollisionScript : MonoBehaviour
{
    public bool swingScript;
    public float damage = 1;
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        swingScript = FindObjectOfType<SetSwinging>().isSwinging;
    }

    void OnTriggerEnter(Collider other) 
    {
        //Debug.Log("Can collide");
        if(swingScript) {
            //Debug.Log("Collided");
            DamageScript damageScript = other.GetComponent<DamageScript>();
            if(damageScript!=null)
            {
                damageScript.Damaged(damage);
            }
        }
    }

}
