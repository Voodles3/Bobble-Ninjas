using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiTest : MonoBehaviour
{

    bool addOrSubtract = true;
    double sum = 3;
    double addend = 0;
    public int maxIterations = 100;
    int first = 2, second = 3, third = 4;
    int multiplier;

    void Start()
    {
        for (int iterations = 0; iterations < maxIterations; addOrSubtract = !addOrSubtract, iterations++)
        {
            multiplier = first * second * third;
            double totalAddend = 4/multiplier;

            addend = Math.Round(totalAddend, 5, MidpointRounding.AwayFromZero);

            if (addOrSubtract)
            {
                sum += addend;
            }
            else
            {
                sum -= addend;
            }

            first += 2;
            second += 2;
            third += 2;
            Debug.Log($"Multiplier: {multiplier}");
            Debug.Log($"Addend: {addend}");
        }
        Debug.Log($"Sum: {sum}");
    }
}
