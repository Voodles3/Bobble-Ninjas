using System.Runtime.CompilerServices;
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
    Vector3 rollDirection;

    public Transform playerBodyTransform;
    public GameObject playerBody;
    public GameObject bobbleninja;

    float horizInput;
    float vertInput;

    Animator animator;
    int isWalkingHash;
    int isRollingHash;

    [Header("-=-Movement-=-")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 5f;
    public float blockSpeed = 5f;
    public float currentMoveSpeed = 5f;

    [Header("-=-Dashing-=-")]
    public float dashPower = 5f;
    public float dashCooldown = 5f;
    public int dashStamina = 30;

    [Header("-=-Rolling-=-")]
    public float rollPower = 5f;
    public float rollDuration = 5f;
    public float rollCooldown = 5f;
    public int rollStamina = 20;

    [Header("-=-Stamina-=-")]
    public float stamina = 100;
    public int maxStamina = 100;
    public float staminaRegenFreq = 0.2f;
    public float staminaRegenDelay = 0.5f;

    [Header("-=-Sprinting-=-")]
    public float sprintStaminaDrainFreq = 0.5f;
    public float sprintStaminaCost = 1f;
    public float sprintAnimationSpeed = 1.5f;

    float sprint;
    float staminaRegenPeriod = 0f;
    float rollPeriod = 0f;
    float period = 0f;
    float lastCheck = 0f;


    bool moving = false;
    bool moveEnabled = true;
    bool sprinting = false;
    bool dashing = false;
    bool dashAvailable = true;
    bool dashEnabled = true;
    bool staminaRegenEnabled = true;
    bool sprintEnabled = true;
    bool blocking = false;
    bool rolling = false;
    bool rollAvailable = true;


    bool infiniteStamina = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        stamina = maxStamina;

        animator = bobbleninja.GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isRollingHash = Animator.StringToHash("isRolling");
    }

    void Update()
    {
        CalculateMovementDirection();
        Sprint();
        RegenStamina();
        DrainStaminaWhileSprinting();
        DebugKeys();
        Block();
    }

    void FixedUpdate()
    {
        MovePlayer();
        Dash();
        Roll();
    }

    void CalculateMovementDirection()
    {
        horizInput = Input.GetAxisRaw("Horizontal");
        vertInput = Input.GetAxisRaw("Vertical");
        direction = new Vector3(horizInput, 0f, vertInput).normalized;

        CheckMovePlayer(direction);
        CheckDash(direction);
        CheckRoll();
    }

    void CheckMovePlayer(Vector3 direction)
    {
        if (moveEnabled && direction.magnitude >= 0.01f)
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

    void CheckRoll()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && direction.magnitude >= 0.01f && stamina >= rollStamina && rollAvailable)
        {
            moveEnabled = false;
            sprintEnabled = false;
            dashEnabled = false;

            rolling = true;
            rollAvailable = false;

            rollDirection = direction;
            Invoke(nameof(ResetRoll), rollCooldown);

            stamina -= rollStamina;
            PauseStaminaRegen();
        }
    }

    void MovePlayer()
    {
        if (moving)
        {
            rb.AddRelativeForce(direction * currentMoveSpeed * Time.deltaTime);
            animator.SetBool(isWalkingHash, true);
        }
        else
        {
            animator.SetBool(isWalkingHash, false);
        }
    }

    void Dash()
    {
        if (dashing)
        {
            rb.velocity = Vector3.zero;
            rb.AddRelativeForce(direction * dashPower * Time.deltaTime, ForceMode.Impulse);
            dashing = false;
        }
    }

    void Roll()
    {
        if (rolling && rollPeriod < rollDuration)
        {
            rb.velocity = Vector3.zero;
            animator.SetBool(isRollingHash, true);
            playerBody.GetComponent<PlayerLookAtMouse>().enabled = false;

            playerBodyTransform.rotation = Quaternion.Slerp(playerBodyTransform.rotation, Quaternion.LookRotation(rollDirection), 0.2f);

            rb.AddRelativeForce(rollDirection * rollPower * Time.deltaTime, ForceMode.Force);

            rollPeriod += Time.deltaTime;

            Invoke(nameof(DoneRolling), (rollDuration));
        }
    }

    void DoneRolling()
    {
        sprintEnabled = true;
        moveEnabled = true;
        rollPeriod = 0f;
        rolling = false;
        animator.SetBool(isRollingHash, false);
        Invoke(nameof(EnableLookScript), 0.05f);
    }

    void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftControl) && moving && stamina > 0f && sprintEnabled)
        {
            currentMoveSpeed = sprintSpeed;
            sprinting = true;
            PauseStaminaRegen();
            animator.SetFloat("walkAnimSpeed", sprintAnimationSpeed);
        }
        else
        {
            currentMoveSpeed = walkSpeed;
            sprinting = false;
            animator.SetFloat("walkAnimSpeed", 1.0f);
        }
    }

    void Block()
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

    void DrainStaminaWhileSprinting()
    {
        if (sprinting && (Time.time - lastCheck) >= sprintStaminaDrainFreq)
        {
            stamina -= sprintStaminaCost;
            lastCheck = Time.time;
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

    void ResetRoll()
    {
        rollAvailable = true;
    }

    void EnableLookScript()
    {
        playerBody.GetComponent<PlayerLookAtMouse>().enabled = true;
    }


}
