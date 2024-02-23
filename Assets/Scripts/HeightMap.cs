using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public static class HeightMap
{
    public static (float[,], float[,]) GenerateHeightMap(NoiseData noiseData, TerrainData terrainData, TemperatureData temperatureData)
    {
        int mapWidth = terrainData.tileCountX * MapMetrics.tileSize;
        int mapHeight = terrainData.tileCountY * MapMetrics.tileSize;

        // generate height noise
        float[,] heightMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, noiseData);

        // redistribute height if height curve provided
        if(noiseData.redistributionCurve != null)
        {
            heightMap = Noise.RedistributeHeights(heightMap, noiseData.redistributionCurve);
        }

        // use square bump to raise center, lower borders (e.g. make an island)
        heightMap = Noise.SquareBump(heightMap, noiseData.islandness);

        // generate moisture noise
        NoiseData moistureNoiseData = noiseData.ShallowCopy();
        /*moistureNoiseData.noiseScale *= 2;
        moistureNoiseData.octaves /= 2;
        float[,] moistureMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, moistureNoiseData);*/

        // generate temperature map
        float[,] temperatureMap = TemperatureMap.GenerateTemperatureMap(heightMap, terrainData, temperatureData);

        return (heightMap, temperatureMap);
    }
}
