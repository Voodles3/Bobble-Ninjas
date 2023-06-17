using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSwinging : MonoBehaviour
{
    public bool isSwinging = false;

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
