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

        // generate noise
        float[,] heightMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, noiseData);

        // redistribute height if height curve provided
        if(noiseData.redistributionCurve != null)
        {
            heightMap = Noise.RedistributeHeights(heightMap, noiseData.redistributionCurve);
        }

        // use square bump to raise center, lower borders (e.g. make an island)
        heightMap = Noise.SquareBump(heightMap, noiseData.islandness);



        return heightMap;
    }
}
