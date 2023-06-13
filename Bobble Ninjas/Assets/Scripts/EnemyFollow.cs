using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;
    NavMeshAgent agent;

    Animator animator;
    int isWalkingHash;

    public float stoppingDistance = 10f;
    public float distanceToPlayer;

    float[] stopDistances = {10f, 12f, 14f, 16f};

    public float distancePeriod = 0.1f;
    public float slowdownFactor = 0.5f;

    bool canSetStopDist = true;
    float period = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        animator = GetComponentInChildren<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");

        player = GameObject.Find("Player").transform;
    }

    void Update()
    {
        DistanceClock();
        //CheckDistance();
    }

    void DistanceClock()
    {
        if (period >= distancePeriod)
        {
            CheckDistance();
            period = 0f;
        }
        period += Time.deltaTime;
    }

    void CheckDistance()
    {
        if (player != null)
        {
            distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer > stoppingDistance + 1)
            {
                agent.isStopped = false;
                agent.speed = 6;
                MoveTowardsTarget(player.position);

                animator.SetBool(isWalkingHash, true);

                if (canSetStopDist)
                {
                    SetStoppingDistance();
                }
            }
            else if (Mathf.Abs(distanceToPlayer - stoppingDistance) < 1f)
            {
                agent.isStopped = true;

                animator.SetBool(isWalkingHash, false);
            }
            else if (distanceToPlayer < stoppingDistance - 1)
            {
                agent.isStopped = false;

                animator.SetBool(isWalkingHash, true);

                Vector3 backwardsMove = Vector3.MoveTowards(transform.position, player.position, -agent.speed);
                agent.SetDestination(backwardsMove);
                canSetStopDist = true;
            }
            
        }
    }

    void SetStoppingDistance()
    {
        {
            canSetStopDist = false;
            //stoppingDistance = stopDistances[Random.Range(0, stopDistances.Length)];
            stoppingDistance = 8;
        }
    }

    void MoveTowardsTarget(Vector3 targetPosition)
    {
        agent.SetDestination(targetPosition);
    }

}