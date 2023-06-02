using System.Security.Cryptography;
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
    public float blockSpeed = 5f;
    public float currentMoveSpeed = 5f;

    [Header("-=-Dashing-=-")]
    public float dashPower = 5f;
    public float dashCooldown = 5f;
    public int dashStamina = 30;

    [Header("-=-Stamina-=-")]
    public float stamina = 100;
    public int maxStamina = 100;
    public float staminaRegenFreq = 0.2f;
    public float staminaRegenDelay = 0.5f;

    [Header("-=-Sprinting-=-")]
    public float sprintStaminaDrainFreq = 0.5f;
    public float sprintStaminaCost = 1f;

    float sprint;
    float staminaRegenPeriod = 0f;
    float period = 0f;
    float lastCheck = 0f;


    bool moving = false;
    bool sprinting = false;
    bool dashing = false;
    bool dashAvailable = true;
    bool dashEnabled = true;
    bool staminaRegenEnabled = true;
    bool sprintEnabled = true;
    bool blocking = false;


    bool infiniteStamina = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        stamina = maxStamina;
    }

    void Update()
    {
        CalculateMovementDirection();
        Sprint();
        RegenStamina();
        StaminaDrain();
        DebugKeys();
        Blocking();
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

    void CalculateMovementDirection()
    {
        horizInput = Input.GetAxisRaw("Horizontal");
        vertInput = Input.GetAxisRaw("Vertical");
        direction = new Vector3(horizInput, 0f, vertInput).normalized;

        CheckMovePlayer(direction);
        CheckDash(direction);
    }

    void RegenStamina()
    {
        //Regenerate Stamina
        if (period >= staminaRegenFreq && staminaRegenEnabled)
        {
            period = 0f;
            if (stamina < maxStamina) stamina++;
        }
        period += Time.deltaTime;

        //Stamina Regeneration Delay
        if (!staminaRegenEnabled)
        {
            staminaRegenPeriod += Time.deltaTime;

            if (staminaRegenPeriod >= staminaRegenDelay)
            {
                staminaRegenEnabled = true;
            }
        }
    }

    void StaminaDrain()
    {
        if (sprinting && (Time.time - lastCheck) >= sprintStaminaDrainFreq)
        {
            stamina -= sprintStaminaCost;
            lastCheck = Time.time;
        }
    }

    void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftControl) && moving && stamina > 0f && sprintEnabled)
        {
            currentMoveSpeed = sprintSpeed;
            sprinting = true;
            PauseStaminaRegen();
        }
        else
        {
            currentMoveSpeed = walkSpeed;
            sprinting = false;
        }
    }

    void CheckMovePlayer(Vector3 direction)
    {
        if (direction.magnitude >= 0.01f)
        {
            moving = true;
            
        }
        else
        {
            moving = false;
        }
    }

    void CheckDash(Vector3 direction)
    {
        if (Input.GetKeyDown(KeyCode.Space) && direction.magnitude >= 0.01f && stamina >= dashStamina && dashAvailable && dashEnabled)
        {
            dashing = true;
            dashAvailable = false;
            Invoke(nameof(ResetDash), dashCooldown);
            stamina -= dashStamina;
            PauseStaminaRegen();
        }
    }

    void Blocking()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            blocking = true;
            sprintEnabled = false;
            dashEnabled = false;
            currentMoveSpeed = blockSpeed;
        }
        else
        {
            blocking = false;
            sprintEnabled = true;
            dashEnabled = true;
        }
    }

    void DebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            infiniteStamina = !infiniteStamina;
        }
        if (infiniteStamina)
        {
            stamina = 100f;
        }
    }

    void PauseStaminaRegen()
    {
        staminaRegenPeriod = 0f;
        staminaRegenEnabled = false;
    }

    void ResetDash()
    {
        dashAvailable = true;
    }

    void ResetStamina()
    {
        staminaRegenEnabled = true;
    }


}
