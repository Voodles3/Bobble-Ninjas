using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;
    NavMeshAgent agent;

    public float minStopDist = 8f;
    public float maxStopDist = 12f;
    public float stoppingDistance = 10f;

    public float distancePeriod = 0.1f;

    bool hasSetStopDist = false;
    float period = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        DistanceClock();
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
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer > stoppingDistance)
            {
                MoveTowardsTarget(player.position);
                if (!hasSetStopDist)
                {
                    SetStoppingDistance();
                }
            }
            else
            {
                StopAgent();
                hasSetStopDist = false;
            }
        }
    }

    void SetStoppingDistance()
    {
        {
            stoppingDistance = Random.Range(minStopDist, maxStopDist);
            hasSetStopDist = true;
        }
    }

    void MoveTowardsTarget(Vector3 targetPosition)
    {
        agent.SetDestination(targetPosition);
    }

    void StopAgent()
    {
        agent.SetDestination(transform.position);
    }
}