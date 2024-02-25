using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu]
public class NoiseData : UpdateableData
{
    public string seed = "ukc";
    public Vector2 offset;

    public float noiseScale = 64;
    [Range(1, 30)]
    public int octaves = 4;
    [Range(0f, 1f)]
    public float persistence = 0.5f;
    public float lacunarity = 1.5f;

    public AnimationCurve redistributionCurve;
    public NoiseData ShallowCopy()
    {
        return (NoiseData)this.MemberwiseClone();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        base.OnValidate();
    }

#endif
}
