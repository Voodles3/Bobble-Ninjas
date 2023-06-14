using UnityEngine;
using UnityEngine.AI;
public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    NavMeshAgent agent;

    [Header("Animations")]
    Animator animator;
    int isWalkingHash;

    [Header("Enemy Movement")]
    public float enemySpeed = 6f;
    public float enemyBackupSpeed = 6f;

    [Header("Stopping Distances")]
    public float stoppingDistance = 10f;
    public float[] stopDistances = {10f, 12f, 14f, 16f};
    bool canSetStopDist = true;

    [Header("Distance To Player")]
    public float distanceToPlayer;

    [Header("AI State Update Frequency")]
    public float AIStateUpdatePeriod = 0.1f;
    float period = 0f;

    [Header("Enemy AI States")]
    public aiState currentState;
    public enum aiState{Chasing, Idling, Circling, Backing, Attacking, Stunned};

    void Start()
    {
        //References
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        player = GameObject.Find("Player").transform;

        //Animations
        isWalkingHash = Animator.StringToHash("isWalking");

        //Miscellaneous
        agent.updateRotation = false;

        //Set Initial AI State
        currentState = aiState.Idling;
    }

    void Update()
    {
        AIStateUpdateClock();
        HandleAIState();
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
        if (player != null)
        {
            //Find the distance between the enemy and the player
            distanceToPlayer = Vector3.Distance(transform.position, player.position);

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
                //Call Attacking method
                break;

            case aiState.Stunned:
                //Call Stunned method
                break;
        }
    }

    void Chasing()
    {
        //Set movement information
        agent.isStopped = false;
        agent.speed = enemySpeed;

        //Move towards player
        MoveTowardsTarget(player.position);

        //Set new stopping distance
        if (canSetStopDist) SetStoppingDistance();

        //Animations
        animator.SetBool(isWalkingHash, true);
    }

    void Idling()
    {
        //Set movement information
        agent.isStopped = true;

        //Animations
        animator.SetBool(isWalkingHash, false);
    }

    void Backing()
    {
        //Set movement information
        agent.isStopped = false;
        agent.speed = enemyBackupSpeed;

        //Move away from player
        Vector3 moveBackwards = Vector3.MoveTowards(transform.position, player.position, -agent.speed);
        MoveTowardsTarget(moveBackwards);

        canSetStopDist = true;

        //Animations
        animator.SetBool(isWalkingHash, true);
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