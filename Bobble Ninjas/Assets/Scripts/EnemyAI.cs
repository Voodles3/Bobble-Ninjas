using UnityEngine;
using System.Collections;
using UnityEngine.AI;
public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;
    NavMeshAgent agent;

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
    public enum aiState{Chasing, Idling, Circling, Backing, Attacking, Stunned};
    
    [Header("Other")]
    public bool canAttack = true;
    public bool canSetAiState = true;

    void Start()
    {
        //References
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        playerTransform = GameObject.Find("Player").transform;

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
    }

    void CheckStayingInIdle()
    {
        if (currentState != aiState.Idling)
        {
            CancelInvoke(nameof(StartAttackCycle));
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
                if (distanceToPlayer > stoppingDistance + 1)
                {
                    currentState = aiState.Chasing;
                }
                else if (Mathf.Abs(distanceToPlayer - stoppingDistance) < 1f)
                {
                    currentState = aiState.Idling;
                }
                else if (distanceToPlayer < stoppingDistance - 1)
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
        }
    }

    void AttackCycle()
    {
        if(currentState == aiState.Idling && canAttack)//if enemy is close enough to player and can attack
        {
            canAttack = false;

            if (currentState == aiState.Idling)
            {
                currentState = aiState.Attacking;
            }

            Debug.Log("Attack!");
        }
        
    }

    void Chasing()
    {
        //Set movement information
        agent.isStopped = false;
        agent.speed = enemySpeed;

        //Move towards player
        MoveTowardsTarget(playerTransform.position);

        //Set new stopping distance
        if (canSetStopDist) SetStoppingDistance();

        //Animations
        animator.SetBool(isWalkingHash, true);
        animator.SetBool(attack1Hash, false);
    }

    void Attacking()
    {
        canSetAiState = false;

        //Set movement information
        agent.isStopped = false;
        agent.speed = enemyAttackSpeed;

        //Move towards player
        MoveTowardsTarget(playerTransform.position);

        //Set new stopping distance
        //the enemy has to be able to move in close to player without going in idle state
        if (canSetStopDist) SetStoppingDistance();

        if(distanceToPlayer > cancelAttackDistance)
        {
            currentState = aiState.Chasing;
            canSetAiState = true;
        }

        //Animations
        animator.SetBool(isWalkingHash, true);

        if(distanceToPlayer <= attackDistance)
        {
            animator.SetBool(attack1Hash, true);
            agent.isStopped = true;
            //currentState = aiState.Idling;
            canSetAiState = true;
        }
        else
        {
            animator.SetBool(attack1Hash, false);
            agent.isStopped = false;
        }

    }

    void Idling()
    {
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

    void MoveTowardsTarget(Vector3 targetPosition)
    {
        //Move towards target
        agent.SetDestination(targetPosition);
    }

    void SetStoppingDistance()
    {
        canSetStopDist = false;

        //Set stopping distance
        stoppingDistance = 8;
        //stoppingDistance = stopDistances[Random.Range(0, stopDistances.Length)];
    }
}