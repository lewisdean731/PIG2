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

    public float heightMultiplier = 1;
    public AnimationCurve heightCurve;

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
