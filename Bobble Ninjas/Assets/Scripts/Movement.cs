using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Rigidbody rb;

    float horizInput;
    float vertInput;

    Vector3 direction;

    public float movementSpeed = 5f;
    public float dashPower = 5f;

    bool moving = false;
    bool dashing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        CheckMovePlayer();
        CheckDash();
    }

    void FixedUpdate()
    {
        MovePlayer();
        Dash();
    }

    void MovePlayer()
    {
        if (moving)
        {
            rb.AddRelativeForce(direction * movementSpeed * Time.deltaTime);
        }
    }

    void Dash()
    {
        if (dashing)
        {
            rb.AddRelativeForce(direction * dashPower * Time.deltaTime, ForceMode.Impulse);
            dashing = false;
        }
    }

    void CheckMovePlayer()
    {
        horizInput = Input.GetAxisRaw("Horizontal");
        vertInput = Input.GetAxisRaw("Vertical");
        direction = new Vector3(horizInput, 0f, vertInput).normalized;

        if(direction.magnitude >= 0.01f)
        {
            moving = true;
            
        }
        else
        {
            moving = false;
        }
    }

    void CheckDash()
    {
        horizInput = Input.GetAxisRaw("Horizontal");
        vertInput = Input.GetAxisRaw("Vertical");
        direction = new Vector3(horizInput, 0f, vertInput).normalized;

        if (Input.GetKeyDown(KeyCode.Space) && direction.magnitude >= 0.01f)
        {
            dashing = true;
        }
    }

}
