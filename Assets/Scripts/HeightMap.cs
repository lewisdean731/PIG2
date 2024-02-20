using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public static class HeightMap
{
    public static float[,] GenerateHeightMap(NoiseData noiseData)
    {

        // generate noise
        float[,] heightMap = Noise.GenerateNoiseMap(
            MapMetrics.tileSize,
            MapMetrics.tileSize,
            noiseData.seed, noiseData.noiseScale,
            noiseData.octaves, noiseData.persistence,
            noiseData.lacunarity,
            new Vector2(0, 0) + noiseData.offset);


        return heightMap;
    }
}
