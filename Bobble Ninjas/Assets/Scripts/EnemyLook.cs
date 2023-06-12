using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLook : MonoBehaviour
{
    public Vector3 direction;
    public Vector3 playerPosition;
    public GameObject player;
    float period = 0f;

    void Start()
    {
        player = GameObject.Find("Player");
    }

    void Update()
    {
        playerPosition = player.transform.position;
        CallAim();
    }

    void CallAim()
    {
        if (period >= 0.01f)
        {
            Aim();
            period = 0f;
        }
        period += Time.deltaTime;
    }

    void Aim()
    {
        // Calculate the direction
        direction = playerPosition - transform.position;

        direction.y = 0;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.2f);
    }
}
