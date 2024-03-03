using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu]
public class ErosionData : UpdateableData
{
    public bool simulateInWater = false;

    [Range(1, 500)]
    public int cycles = 100;
    [Range(1, 500)]
    public int runsPerCycle = 100;
    [Range(1, 500)]
    public int stepsPerRun = 50;

    [Range(0, 0.2f)]
    public float percentLossPerStep = 0.03f;

    public ErosionData ShallowCopy()
    {
        return (ErosionData)this.MemberwiseClone();
    }
}
