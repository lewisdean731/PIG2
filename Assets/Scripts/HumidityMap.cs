using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HumidityMap
{
    public static float[,] GenerateHumidityMap(float[,] heightMap, float[,] tempMap01, NoiseData noiseData, TerrainData terData, TemperatureData tempData)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        float[,] humidityMap;

        // Generate noise
        NoiseData hNoise = noiseData.ShallowCopy();
        hNoise.noiseScale += 2;
        hNoise.octaves = 2;
        hNoise.lacunarity = 1;
        humidityMap = Noise.GenerateNoiseMap(width, height, hNoise);

        // Apply terrain effects
        humidityMap = ApplyTerrainEffects(humidityMap, heightMap);

        // Apply temperature effects
        humidityMap = ApplyTemperatureEffects(humidityMap, tempMap01);

        // ...

        return humidityMap;
    }

    public static float[,] ApplyTerrainEffects(float[,] humidityMap, float[,] temperatureMap01)
    {
        for (int y = 0; y < humidityMap.GetLength(1); y++)
        {
            for (int x = 0; x < humidityMap.GetLength(0); x++)
            {
                // Higher temperature = lower humidity
                float temperature01 = temperatureMap01[x, y];

                humidityMap[x, y] -= temperature01 / 2;
            }
        }

        return humidityMap;
    }

    public static float[,] ApplyTemperatureEffects(float[,] humidityMap, float[,] heightMap)
    {
        for (int y = 0; y < humidityMap.GetLength(1); y++)
        {
            for (int x = 0; x < humidityMap.GetLength(0); x++)
            {
                // Higher temperature = lower humidity
                float elevation01 = heightMap[x, y];

                humidityMap[x, y] -= elevation01 / 2;
            }
        }

        return humidityMap;
    }

}
