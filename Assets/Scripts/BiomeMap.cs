using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BiomeMap
{
    public static BiomeType[,] GenerateBiomeMap(float[,] heightMap, float[,] tempMap01, float[,] humidityMap, NoiseData noiseData, TerrainData terData, TemperatureData tempData, BiomesData biomeData)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        BiomeType[,] biomeMap = new BiomeType[width,height];


        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float elevation01 = heightMap[x, y];
                float temperatureC = TemperatureMap.ToTempC(tempMap01[x, y], tempData);
                float humidity01 = humidityMap[x, y];

                Biome bestBiome = biomeData.biomes[0];

                // Get applicable biome types
                foreach (Biome biome in biomeData.biomes)
                {

                    if(elevation01 > biome.fromHeight && biome.fromHeight >= bestBiome.fromHeight)
                    {
                        if(temperatureC > biome.fromTempC && biome.fromTempC >= bestBiome.fromTempC)
                        {
                            if(humidity01 > biome.fromHumidity && biome.fromHumidity >= bestBiome.fromHumidity)
                            {
                                bestBiome = biome;
                            }
                        }
                    }
                }

                biomeMap[x, y] = bestBiome.biomeType;
            }
        }

        return biomeMap;

    }

}
