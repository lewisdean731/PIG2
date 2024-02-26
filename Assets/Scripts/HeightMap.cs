using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public static class HeightMap
{
    public static float[,] GenerateHeightMap(NoiseData noiseData, TerrainData terrainData)
    {
        int mapWidth = terrainData.tileCountX * MapMetrics.tileSize;
        int mapHeight = terrainData.tileCountY * MapMetrics.tileSize;

        // generate height noise
        float[,] heightMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, noiseData);

        // add mountain ridges
        (float[,] ridgeMap, float[,] ridgeZoneMap) = GenerateRidgeMap(mapWidth, mapHeight, noiseData);
        heightMap = Noise.BlendNoiseMaps(heightMap, ridgeMap, BlendType.Add);

        // redistribute height if height curve provided
        if (noiseData.redistributionCurve != null)
        {
            heightMap = Noise.RedistributeHeights(heightMap, noiseData.redistributionCurve);
        }

        // use square bump to raise center, lower borders (e.g. make an island)
        if (terrainData.islandInfo.isIsland)
        {
            heightMap = Noise.SquareBump(heightMap, terrainData);
        }

        return heightMap;
    }

    public static (float[,], float[,]) GenerateRidgeMap(int width, int height, NoiseData noiseData)
    {

        // generate mountain ridge map
        NoiseData ridgeNoiseData = noiseData.ShallowCopy();
        ridgeNoiseData.noiseScale *= 8;
        ridgeNoiseData.lacunarity = 2.5f;
        ridgeNoiseData.persistence = 1;
        float[,] ridgeMap = Noise.GenerateNoiseMap(width, height, ridgeNoiseData, true);

        // generate ridge zone map
        NoiseData ridgeZoneNoiseData = noiseData.ShallowCopy();
        ridgeZoneNoiseData.noiseScale *= 4;
        ridgeZoneNoiseData.octaves = 2;
        ridgeZoneNoiseData.lacunarity = 1.5f;
        ridgeNoiseData.persistence = 0;
        float[,] ridgeZoneMap = Noise.GenerateNoiseMap(width, height, ridgeZoneNoiseData);

        // scale rides by zone map
        ridgeMap = Noise.BlendNoiseMaps(ridgeMap, ridgeZoneMap, BlendType.Multiply);


        return (ridgeMap, ridgeZoneMap);
    }
}
