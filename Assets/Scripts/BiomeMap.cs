using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BiomeMap
{
    class BiomeElegibility
    {
        public Biome biome;
        public float heightScore;
        public float tempScore;
        public float humidityScore;

        public BiomeElegibility (Biome biome, float heightScore, float tempScore, float humidityScore)
        {
            this.biome = biome;
            this.heightScore = heightScore;
            this.tempScore = tempScore;
            this.humidityScore = humidityScore;
        }
    }

    public static BiomeType[,] GenerateBiomeMap(float[,] heightMap, float[,] tempMap01, float[,] humidityMap, NoiseData noiseData, TerrainData terData, TemperatureData tempData, BiomesData biomeData)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        BiomeType[,] biomeMap = new BiomeType[width,height];

        float heightWeighting = 0.5f;
        float tempWeighting = 0.1f;
        float humidityWeighting = 0.4f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float elevation01 = heightMap[x, y];
                float temperatureC = TemperatureMap.ToTempC(tempMap01[x, y], tempData);
                float humidity01 = humidityMap[x, y];

                List<BiomeElegibility> elegibleBiomes = new List<BiomeElegibility>();

                // Get elegible biomes
                foreach (Biome biome in biomeData.biomes)
                {
                    if(elevation01 > biome.fromHeight)
                    {
                        if(temperatureC > biome.fromTempC)
                        {
                            if(humidity01 > biome.fromHumidity)
                            {
                                elegibleBiomes.Add(new BiomeElegibility(
                                    biome,
                                    biome.fromHeight - elevation01,
                                    biome.fromTempC - temperatureC,
                                    biome.fromHumidity - humidity01
                                ));
                            }
                        }
                    }
                }

                if (elegibleBiomes.Count == 0)
                {
                    elegibleBiomes.Add(new BiomeElegibility(
                        biomeData.biomes[0],
                        99,
                        99,
                        99
                    ));
                }

                // Determine best biome
                BiomeElegibility bestBiome = elegibleBiomes[0];

                foreach (BiomeElegibility be in elegibleBiomes)
                {
                    float heightScoreDiff = (be.heightScore - bestBiome.heightScore) * heightWeighting;
                    float tempScoreDiff = (be.tempScore - bestBiome.tempScore) * tempWeighting;
                    float humiditytScoreDiff = (be.humidityScore - bestBiome.humidityScore) * humidityWeighting;

                    if(heightScoreDiff + tempScoreDiff + humiditytScoreDiff > 0)
                    {
                        bestBiome = be;
                    }
                }

                biomeMap[x, y] = bestBiome.biome.biomeType;
            }
        }

        return biomeMap;

    }

}
