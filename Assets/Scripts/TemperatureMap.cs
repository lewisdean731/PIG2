using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public static class TemperatureMap
{
    public static (float[,], float[,]) GenerateTemperatureMaps(float[,] heightMap, TerrainData terrData, TemperatureData tempData)
    {

        // Generate equator temperature map
        float[,] equatorMap = GenerateEquatorMap(heightMap, tempData);

        // Apply terrain effects to temperatures
        float[,] temperatureMap01 = ApplyTerrainTemperatureEffects(equatorMap, heightMap, tempData, terrData);

        // Convert to real temperatures
        float[,] temperatureMap = ToRealTemperatures(temperatureMap01, tempData);

        return (temperatureMap, temperatureMap01);
    }
    
    public static float[,] GenerateEquatorMap(float[,] heightMap, TemperatureData temperatureData)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        float[,] equatorMap = new float[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float distance = CalculateDistanceFromEquator(new Vector2(x,y), temperatureData.equatorPos, temperatureData.equatorRot);
                float normalizedDistance = Mathf.Clamp01(distance / width * temperatureData.equatorTransition); // Normalize the distance
                equatorMap[x, y] = 1 - normalizedDistance;
            }
        }

        // Now, temperatureMap contains values based on their distance from the equator
        return equatorMap;
    }

    static float CalculateDistanceFromEquator(Vector2 position, Vector2 equatorPos, float equatorRotation)
    {
        // Rotate the point around the line position to align with the line
        Vector2 rotatedPos = Quaternion.Euler(0, 0, equatorRotation) * (position - equatorPos);

        // Calculate the distance from the rotated point to the x-axis
        float distance = Mathf.Abs(rotatedPos.y);

        return distance;
    }


    public static float[,] ApplyTerrainTemperatureEffects(float[,] equatorMap, float[,] heightMap, TemperatureData tempData, TerrainData terrData)
    {

        int width = equatorMap.GetLength(0);
        int height = equatorMap.GetLength(1);

        float[,] temperatureMap = new float[width, height];


        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Higher elevation = lower temperatures
                float elevation01 = heightMap[x, y];
                float temperature01 = equatorMap[x, y];

                temperatureMap[x, y] = temperature01 - elevation01 / 2;
            }
        }

        return temperatureMap;
    }

    public static float[,] ToRealTemperatures(float[,] tempMap01, TemperatureData tempData)
    {
        int width = tempMap01.GetLength(0);
        int height = tempMap01.GetLength(1);

        float[,] temperatureMap = new float[width, height];


        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Scale 0-1 values into real temperatures between given min/max
                temperatureMap[x, y] = Mathf.Lerp(tempData.minTemperatureC, tempData.maxTemperatureC, tempMap01[x, y]);
            }
        }

        return temperatureMap;
    }

    public static float[,] To01Map(float[,] tempMap01, TemperatureData tempData)
    {
        int width = tempMap01.GetLength(0);
        int height = tempMap01.GetLength(1);

        float[,] temperatureMap01 = new float[width, height];


        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Scale temperature between 0-1
                temperatureMap01[x,y] = Mathf.InverseLerp(tempData.minTemperatureC, tempData.maxTemperatureC, tempMap01[x, y]);
            }
        }

        return temperatureMap01;
    }

}
