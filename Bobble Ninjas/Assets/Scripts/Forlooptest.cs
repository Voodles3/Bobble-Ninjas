using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forlooptest : MonoBehaviour
{
    void Start()
    {
        int n = 5, sum = 0;

			for (int i=1; i<=n; i++)
			{
				sum += i;
				Debug.Log(i);			}

			Debug.Log($"Sum of first {n} natural numbers = {sum}");
    }
}
