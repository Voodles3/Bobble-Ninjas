using UnityEngine;
using System.Collections;
using UnityEngine.AI;

[DefaultExecutionOrder(1)]
public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;
    public NavMeshAgent agent;

    [Header("Animations")]
    Animator animator;
    int isWalkingHash;
    int attack1Hash;

    [Header("Enemy Movement")]
    public float enemySpeed = 6f;
    public float enemyBackingSpeed = 6f;
    public float enemyAttackSpeed = 10f;

    [Header("Stopping Distances")]
    public float stoppingDistance = 10f;
    public float[] stopDistances = {10f, 12f, 14f, 16f};
    bool canSetStopDist = true;
    public float attackDistance = 4f;
    public float cancelAttackDistance = 14f;

    [Header("Distance To Player")]
    public float distanceToPlayer;

    [Header("AI State Update Frequency")]
    public float AIStateUpdatePeriod = 0.1f;
    float period = 0f;

    [Header("Enemy AI States")]
    public aiState currentState;
    public enum aiState{Chasing, Idling, Circling, Backing, Attacking, Stunned, Unaware};
    
    [Header("Other")]
    private EnemyLook lookScript;
    private EnemyHandler enemyHandlerScript;
    public EnemyDamaged enemyDamagedScript;

    public bool canAttack = true;
    public bool canSetAiState = true;
    public bool isCurrentlyTargetingPlayer;
    public bool lastCheckForTarget;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        EnemyHandler.Instance.EnemyAiScriptInstances.Add(this);
    }

    void Start()
    {
        //References
        animator = GetComponentInChildren<Animator>();
        playerTransform = GameObject.Find("Player").transform;
        lookScript = GetComponent<EnemyLook>();
        enemyHandlerScript = GameObject.Find("Enemy Handler").GetComponent<EnemyHandler>();
        enemyDamagedScript = GetComponent<EnemyDamaged>();

        //Animations
        isWalkingHash = Animator.StringToHash("isWalking");
        attack1Hash = Animator.StringToHash("attack1");

        //Miscellaneous
        agent.updateRotation = false;
        //Set Initial AI State
        currentState = aiState.Chasing;
    }


    void Update()
    {
        AIStateUpdateClock();
        HandleAIState();
        CheckStayingInIdle();
        //CheckTargetStatus();
        DebugKeys();
    }

    void DebugKeys()
    {
        if (Input.GetKey(KeyCode.G))
        {
            currentState = aiState.Unaware;
        }
        else
        {
            lookScript.enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            canSetAiState = !canSetAiState; 
        }
    }

    void CheckStayingInIdle()
    {
        if (Mathf.Abs(distanceToPlayer - stoppingDistance) >= 5)
        {
            CancelInvoke(nameof(StartAttackCycle));
        }
    }

    void CheckTargetStatus()
    {
        if (isCurrentlyTargetingPlayer != lastCheckForTarget)
        {
            lastCheckForTarget = isCurrentlyTargetingPlayer;

            if (isCurrentlyTargetingPlayer == true)
            {
                enemyHandlerScript.activeEnemyCount++;
            }
            else
            {
                enemyHandlerScript.activeEnemyCount--;
            }

            Debug.Log("Changed!");
        }
    }

    void AIStateUpdateClock()
    {
        //Call SetAIState() every AIStateUpdatePeriod seconds
        if (period >= AIStateUpdatePeriod)
        {
            SetAIState();
            period = 0f;
        }
        period += Time.deltaTime;
    }

    void SetAIState()
    {
        if (playerTransform != null)
        {
            //Find the distance between the enemy and the player
            distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if(canSetAiState)
            {
                //Set enemy AI state based on enemy's distance from the player
                if (distanceToPlayer > stoppingDistance + 2)
                {
                    currentState = aiState.Chasing;
                }
                else if (Mathf.Abs(distanceToPlayer - stoppingDistance) < 2)
                {
                    currentState = aiState.Idling;
                }
                else if (distanceToPlayer < stoppingDistance - 2)
                {
                    currentState = aiState.Backing;
                }
            }
        }
    }

    void HandleAIState()
    {
        //Execute a method based on current enemy AI state
        switch (currentState)
        {
            case aiState.Chasing:
                Chasing();
                break;

            case aiState.Idling:
                Idling();
                break;

            case aiState.Backing:
                Backing();
                break;

            case aiState.Attacking:
                Attacking();
                break;

            case aiState.Stunned:
                //Call Stunned method
                break;

            case aiState.Unaware:
                Unaware();
                break;
        }
    }

    void AttackCycle()
    {
        if(currentState == aiState.Idling && canAttack)//if enemy is close enough to player and can attack
        {
            canAttack = false;

            currentState = aiState.Attacking;
        }
    }

    void Unaware()
    {
        isCurrentlyTargetingPlayer = false;

        //Set movement information
        agent.isStopped = true;

        //Animations
        animator.SetBool(isWalkingHash, true);
        animator.SetBool(attack1Hash, false);

        lookScript.enabled = false;
    }

    void Chasing()
    {
        isCurrentlyTargetingPlayer = true;

        //Set movement information
        agent.isStopped = false;
        agent.speed = enemySpeed;

        //Move towards player
        enemyHandlerScript.MoveTowardsCircularPositionAroundTarget();
        
        //Set new stopping distance
        if (canSetStopDist) SetStoppingDistance();

        //Animations
        animator.SetBool(isWalkingHash, true);
        animator.SetBool(attack1Hash, false);
    }

    void Attacking()
    {
        isCurrentlyTargetingPlayer = true;

        canSetAiState = false;

        //Set movement information
        agent.isStopped = false;
        agent.speed = enemyAttackSpeed;

        //Move towards player
        MoveTowardsTarget(playerTransform.position);

        //Set new stopping distance
        if (canSetStopDist) SetStoppingDistance();

        if(distanceToPlayer > cancelAttackDistance)
        {
            currentState = aiState.Chasing;
            canSetAiState = true;
        }

        //Animations
        animator.SetBool(isWalkingHash, true);

        if (distanceToPlayer <= attackDistance)
        {
            Attack();
        }
    }

    void Attack()
    {
        animator.SetBool(attack1Hash, true);
        agent.isStopped = true;
        canSetAiState = true;
    }

    void Idling()
    {
        isCurrentlyTargetingPlayer = true;

        //Set movement information
        agent.isStopped = true;

        canAttack = true;
        BeginAttack();

        //Animations
        animator.SetBool(isWalkingHash, false);
        animator.SetBool(attack1Hash, false);
    }

    void BeginAttack()
    {
        Invoke(nameof(StartAttackCycle), 1.5f);
    }

    void StartAttackCycle()
    {
        AttackCycle();
    }

    void Backing()
    {
        isCurrentlyTargetingPlayer = true;

        //Set movement information
        agent.isStopped = false;
        agent.speed = enemyBackingSpeed;

        //Move away from player
        Vector3 moveBackwards = Vector3.MoveTowards(transform.position, playerTransform.position, -agent.speed);
        MoveTowardsTarget(moveBackwards);

        canSetStopDist = true;

        //resets attack cooldown, i put it in backing because when enemy is done with an attack he will have to back up
        canAttack = true;

        //Animations
        animator.SetBool(isWalkingHash, true);
        animator.SetBool(attack1Hash, false);
    }

    public void MoveTowardsTarget(Vector3 targetPosition)
    {
        agent.SetDestination(targetPosition);
    }

    void SetStoppingDistance()
    {
        canSetStopDist = false;

        //Set stopping distance
        stoppingDistance = 10;
        //stoppingDistance = stopDistances[Random.Range(0, stopDistances.Length)];
    }
}