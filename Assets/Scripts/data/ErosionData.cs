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

    [Range(0, 1f)]
    public float capacityFactor = 0.1f; // Controls how much sediment water can carry

    [Range(0, 1f)]
    public float erosionSpeed = 0.05f; // Controls how easily terrain is eroded

    [Range(0, 1f)]
    public float depositSpeed = 0.05f; // Controls how easily sediment is deposited

    [Range(0, 1f)]
    public float evaporationRate = 0.01f; // Controls how quickly water evaporates

    [Range(0, 1f)]
    public float transportSpeed = 0.1f; // Controls how much sediment is transported downstream

    [Range(0, 5f)]
    public float humidityWaterMultiplier =
        1.0f; // Multiplier for water based on humidity

    public ErosionData ShallowCopy()
    { return (ErosionData)this.MemberwiseClone(); }
}