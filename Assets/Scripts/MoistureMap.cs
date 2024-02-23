using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MoistureMap
{
    public static float[,] GenerateMoistureMap(float[,] heightMap, float[,] tempMap, NoiseData noiseData, TerrainData terData, TemperatureData tempData)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        float[,] moistureMap = new float[width, height];

        // Create noise map

        // Apply terrain efffects

        // Apply temperature effects

        // Convert to real moisture values
        // ...

        return moistureMap;
    }
}
