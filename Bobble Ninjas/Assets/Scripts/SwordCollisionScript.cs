using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollisionScript : MonoBehaviour
{
    public SetSwinging swingScript;
    

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other) {
        if(swingScript.isSwinging) {
            Debug.Log("Collided");
        }
    }
}
