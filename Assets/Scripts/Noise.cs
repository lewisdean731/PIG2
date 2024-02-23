using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(
        int mapWidth,
        int mapHeight,
        NoiseData noiseData)
    {
        (string seed,
        Vector2 offset,
        float noiseScale, 
        int octaves, 
        float persistence, 
        float lacunarity, 
        float islandness,
        AnimationCurve redistributionCurve) = noiseData;

        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(convertStringSeedToInt(seed));
        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistence;
        }


        if (noiseScale <= 0)
        {
            noiseScale = 0.0001f;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2;
        float halfHeight = mapHeight / 2;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                // each octave we will layer another level of noise on to the noiseMap,
                // subsequent octaves will impact the overall map less based on persistance
                // and lacunarity
                for (int i = 0; i < octaves; i++)
                {
                    // sample points subtract halfWidth / halfHeight so when we change noise scale,
                    // it scales from the center of the noise map instead of the top-righht corner
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / noiseScale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / noiseScale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                    perlinValue += perlinValue * 2 - 1; // allows value to be in the range +-1 so noiseHeight may sometimes decrease

                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistence; // decreases each octave
                    frequency *= lacunarity; // increases each octave
                }

                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }

        // go through all noise map values and normalise them between max and min values
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }

    public static float[,] RedistributeHeights(float[,] noiseMap, AnimationCurve redistributionCurve)
    {
        for (int y = 0; y < noiseMap.GetLength(1); y++)
        {
            for (int x = 0; x < noiseMap.GetLength(0); x++)
            {
                noiseMap[x, y] = redistributionCurve.Evaluate(noiseMap[x, y]);
            }
        }

        return noiseMap;
    }

    public static float[,] SquareBump(float[,] noiseMap, float mix)
    {

        float width = noiseMap.GetLength(0);
        float height = noiseMap.GetLength(1);
        float distance;
        float nx;
        float ny;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // 0 when xy = half width/height
                // -1 when xy = 0 or 1 when max width/height
                nx = (float)((2 * x) / width) - 1;
                ny = (float)((2 * y) / height) - 1;

                // square bump
                distance = (float)(1 - (1-(nx*nx)) * (1-(ny*ny)));
                
                if(distance * 1.5f < 0.8)
                {
                    //distance *= 1.5f;
                }

                noiseMap[x, y] = Mathf.Lerp(noiseMap[x,y], 1 - distance, mix);
            }
        }

        return noiseMap;
    }

    public static int convertStringSeedToInt(string seed)
    {
        int seedValue = 0;
        byte[] bytes = Encoding.ASCII.GetBytes(seed);

        foreach (byte b in bytes)
        {
            seedValue += b;
        }

        return seedValue;
    }
}
