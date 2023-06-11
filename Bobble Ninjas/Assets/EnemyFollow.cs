using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;
    public float stoppingDistance = 2f;

    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer > stoppingDistance)
            {
                MoveTowardsTarget(player.position);
            }
            else
            {
                StopAgent();
            }
        }
    }

    private void MoveTowardsTarget(Vector3 targetPosition)
    {
        agent.SetDestination(targetPosition);
    }

    private void StopAgent()
    {
        agent.SetDestination(transform.position);
    }
}