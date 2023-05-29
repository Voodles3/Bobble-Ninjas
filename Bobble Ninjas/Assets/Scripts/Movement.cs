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


    [Header("-=-Movement-=-")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 5f;
    public float currentMoveSpeed = 5f;

    [Header("-=-Dashing-=-")]
    public float dashPower = 5f;
    public float dashCooldown = 5f;
    public int dashStamina = 30;

    [Header("-=-Stamina-=-")]
    public int stamina = 100;
    public int maxStamina = 100;
    public float staminaRegenFreq = 0.2f;
    public float staminaRegenDelay = 0.5f;
    float staminaRegenPeriod = 0f;
    bool canRegenStamina = true;
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
        //Regenerate Stamina
        if (period >= staminaRegenFreq && canRegenStamina)
        {
            period = 0f;
            if (stamina < maxStamina) stamina++;
        }
        period += Time.deltaTime;

        //Stamina Regeneration Delay
        if (!canRegenStamina)
        {
            staminaRegenPeriod += Time.deltaTime;

            if (staminaRegenPeriod >= staminaRegenDelay)
            {
                canRegenStamina = true;
            }
        }
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
            PauseStaminaRegen();
        }
    }

    void PauseStaminaRegen()
    {
        staminaRegenPeriod = 0f;
        canRegenStamina = false;
    }

    void ResetDash()
    {
        canDash = true;
    }

    void ResetStamina()
    {
        canRegenStamina = true;
    }

}
