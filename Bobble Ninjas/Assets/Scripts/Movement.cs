using System.ComponentModel;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Rigidbody rb;
    Vector3 direction;


    float horizInput;
    float vertInput;

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 5f;
    public float currentMoveSpeed = 5f;

    [Header("Dashing")]
    public float dashPower = 5f;
    public float dashCooldown = 5f;
    public float dashStamina = 20f;

    public float stamina = 100f;
    public float maxStamina = 100f;
    public float staminaRegenPeriod = 0.2f;
    float period = 0f;

    bool moving = false;
    bool dashing = false;
    bool canDash = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        stamina = maxStamina;
    }

    void Update()
    {
        CheckMovePlayer();
        CheckDash();
        CheckSprint();
        RegenStamina();
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
            rb.AddRelativeForce(direction * currentMoveSpeed * Time.deltaTime);
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

    void RegenStamina()
    {
        if (period > staminaRegenPeriod)
        {
            if (stamina < maxStamina) stamina++;
            period = 0;
        }
        period += Time.deltaTime;
    }

    void CheckSprint()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            currentMoveSpeed = sprintSpeed;
        }
        else
        {
            currentMoveSpeed = walkSpeed;
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

        if (Input.GetKeyDown(KeyCode.Space) && direction.magnitude >= 0.01f && canDash && stamina >= dashStamina)
        {
            dashing = true;
            canDash = false;
            Invoke(nameof(ResetDash), dashCooldown);
            stamina -= dashStamina;
        }
    }

    void ResetDash()
    {
        canDash = true;
    }

}
