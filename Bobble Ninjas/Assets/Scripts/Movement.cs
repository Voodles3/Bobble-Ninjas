using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Rigidbody rb;

    float horizInput;
    float vertInput;

    public float movementSpeed = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        horizInput = Input.GetAxisRaw("Horizontal");
        vertInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizInput, 0f, vertInput).normalized;

        if(direction.magnitude >= 0.01f)
        {
            rb.AddRelativeForce(direction * movementSpeed * Time.deltaTime);
        }
    }

}
