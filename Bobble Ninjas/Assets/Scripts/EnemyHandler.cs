using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[DefaultExecutionOrder(0)]
public class EnemyHandler : MonoBehaviour
{
    private static EnemyHandler instance;
    public static EnemyHandler Instance
    {
        get {return instance;}
        private set
        {
            instance = value;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }

        Destroy(gameObject);
    }

    public int activeEnemyCount = 0;
    public bool canShuffle = false;

    public Transform playerTransform;
    EnemyAI enemyAIScript;


    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        enemyAIScript = GameObject.FindWithTag("Enemy").GetComponent<EnemyAI>();
    }

    void Update()
    {
        Checks();
    }

    void Checks()
    {
        //Check for deaths
        for (int i = 0; i < EnemyAiScriptInstances.Count; i++)
        {
            if (EnemyAiScriptInstances[i].enemyDamagedScript.isDead)
            {
                EnemyAiScriptInstances.Remove(EnemyAiScriptInstances[i]);
            }
        }

        for (int i = 0; i < EnemyAiScriptInstances.Count; i++)
        {
            if (EnemyAiScriptInstances[i].agent.remainingDistance >= 5 && EnemyAiScriptInstances[i].currentState == EnemyAI.aiState.Chasing)
            {
                if (!EnemiesReadyToShuffle.Contains(EnemyAiScriptInstances[i])) { EnemiesReadyToShuffle.Add(EnemyAiScriptInstances[i]); }
            }
            else
            {
                EnemiesReadyToShuffle.Remove(EnemyAiScriptInstances[i]);
            }

            if (EnemiesReadyToShuffle.Count == EnemyAiScriptInstances.Count && canShuffle)
            {
                canShuffle = false;
                Shuffle(EnemyAiScriptInstances);
                Debug.Log("Shuffling.");
            }
            else if (EnemiesReadyToShuffle.Count <= EnemyAiScriptInstances.Count / 2)
            {
                canShuffle = true;
            }
        }

        /*if (enemyAIScript.currentState == EnemyAI.aiState.Chasing && !haveShuffled)
        {
            haveShuffled = true;
            Shuffle(EnemyAiScriptInstances);
            Debug.Log("Shuffling!");
        }
        else if (enemyAIScript.currentState != EnemyAI.aiState.Chasing)
        {
            haveShuffled = false;
        }*/

    }

    void Shuffle<T>(List<T> inputList)
    {
        for (int i = 0; i < inputList.Count - 1; i++)
        {
            T temp = inputList[i];
            int rand = Random.Range(i, inputList.Count);
            inputList[i] = inputList[rand];
            inputList[rand] = temp;
        }
    }

    public List<EnemyAI> EnemyAiScriptInstances = new List<EnemyAI>();
    public List<EnemyAI> EnemiesReadyToShuffle = new List<EnemyAI>();
    public float radiusAroundTarget;

    public void MoveTowardsCircularPositionAroundTarget()
    {
        radiusAroundTarget = enemyAIScript.stoppingDistance;


        if (EnemyAiScriptInstances.Count <= 1) 
        {
            enemyAIScript.MoveTowardsTarget(playerTransform.position);
            enemyAIScript.canSetAiState = true;
        }
        else 
        {
            //List<EnemyAI> SortedEnemies = Enemies.OrderBy(x => x.distanceToPlayer).ToList();

            for (int i = 0; i < EnemyAiScriptInstances.Count; i++)
            {
                EnemyAiScriptInstances[i].MoveTowardsTarget(new Vector3
                (   playerTransform.position.x + radiusAroundTarget * Mathf.Cos(2 * Mathf.PI * i / EnemyAiScriptInstances.Count),
                    playerTransform.position.y,
                    playerTransform.position.z + radiusAroundTarget * Mathf.Sin(2 * Mathf.PI * i / EnemyAiScriptInstances.Count)));
            }

            for (int i = 0; i < EnemyAiScriptInstances.Count; i++)
            {
                if (EnemyAiScriptInstances[i].agent.remainingDistance <= 2)
                {
                    EnemyAiScriptInstances[i].canSetAiState = true;
                }
                else
                {
                    EnemyAiScriptInstances[i].canSetAiState = false;
                }
            }
        }
    }
}
