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

    [Range(0f, 1f)]
    public float islandness = 0.5f;

    public AnimationCurve redistributionCurve;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        base.OnValidate();
    }

    public void Deconstruct(
        out string seed,
        out Vector2 offset,
        out float noiseScale,
        out int octaves,
        out float persistence,
        out float lacunarity,
        out float islandness,
        out AnimationCurve redistributionCurve)
    {
        seed = this.seed;
        offset = this.offset;
        noiseScale = this.noiseScale;
        octaves = this.octaves;
        persistence = this.persistence;
        lacunarity = this.lacunarity;
        islandness = this.islandness;
        redistributionCurve = this.redistributionCurve;
    }

#endif
}
