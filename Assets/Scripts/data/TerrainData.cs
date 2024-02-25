using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TerrainData : UpdateableData
{
    [Range(1, 32)]
    public int tileCountX = 4;
    [Range(1, 32)]
    public int tileCountY = 4;

    public float scale = 1f;

    [Range(0, 1)]
    public float seaLevel = 0.18f;

    public float heightMultiplier = 1;
    public AnimationCurve heightCurve;

    public IslandInfo islandInfo;

    public float minHeight
    {
        get
        {
            return scale * heightMultiplier * heightCurve.Evaluate(0);
        }
    }
    public float maxHeight
    {
        get
        {
            return scale * heightMultiplier * heightCurve.Evaluate(1);
        }
    }
}

[System.Serializable]
public class IslandInfo
{
    public bool isIsland = true;

    [Range(0f, 1f)]
    public float size = 0.6f;
    [Range(0f, 1f)]
    public float roundness = 0.6f;
}
