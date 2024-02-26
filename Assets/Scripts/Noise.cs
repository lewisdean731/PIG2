using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;

public enum BlendType
{
    Divide,
    Multiply,
    Add,
    Subtract,
    Lighten,
    Darken,
}
public static class Noise
{
    public static float[,] GenerateNoiseMap(
        int width,
        int height,
        NoiseData nData,
        bool useRidgeNoise = false)
    {

        float[,] noiseMap = new float[width, height];

        System.Random prng = new System.Random(convertStringSeedToInt(nData.seed));
        Vector2[] octaveOffsets = new Vector2[nData.octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < nData.octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + nData.offset.x;
            float offsetY = prng.Next(-100000, 100000) - nData.offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= nData.persistence;
        }


        if (nData.noiseScale <= 0)
        {
            nData.noiseScale = 0.0001f;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = width / 2;
        float halfHeight = height / 2;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                // each octave we will layer another level of noise on to the noiseMap,
                // subsequent octaves will impact the overall map less based on persistance
                // and lacunarity
                for (int i = 0; i < nData.octaves; i++)
                {
                    // sample points subtract halfWidth / halfHeight so when we change noise scale,
                    // it scales from the center of the noise map instead of the top-righht corner
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / nData.noiseScale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / nData.noiseScale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                    perlinValue += perlinValue * 2 - 1; // allows value to be in the range +-1 so noiseHeight may sometimes decrease

                    noiseHeight += perlinValue * amplitude;

                    if (useRidgeNoise)
                    {
                        noiseHeight = RidgeNoise(noiseHeight);
                    }

                    // noiseHeight *= 1.75f;

                    amplitude *= nData.persistence; // decreases each octave
                    frequency *= nData.lacunarity; // increases each octave
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
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
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

    public static float[,] SquareBump(float[,] noiseMap, TerrainData tData)
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
                distance = Mathf.Pow((float)(1 - (1-(nx*nx)) * (1-(ny*ny))), 1 - tData.islandInfo.roundness);

                float mix = 0;
                float mixDistance = 0;

                if(distance > tData.islandInfo.size)
                {
                    float i = Mathf.InverseLerp(tData.islandInfo.size, 1, distance);
                    mix = Mathf.Lerp(0, 1, i);
                    mixDistance = Mathf.Lerp(0, 1, i);
                }

                noiseMap[x, y] = Mathf.Lerp(noiseMap[x,y], 1 - mixDistance, mix);
            }
        }

        return noiseMap;
    }

    public static float RidgeNoise(float elevation)
    {
        return 2 * Mathf.Abs(0.5f - Mathf.Abs(elevation - 0.5f));
    }

    /// <summary>
    /// Method <c>BlendNoiseMap</c> Blend two noise maps using the given blend mode.
    /// </summary>
    public static float[,] BlendNoiseMaps(float[,] map1, float[,] map2, BlendType blendType, float modifier = 1)
    {
        float[,] blendedMap = new float[map1.GetLength(0), map1.GetLength(1)];

        for (int y = 0; y < map1.GetLength(1); y++)
        {
            for (int x = 0; x < map1.GetLength(0); x++)
            {
                switch (blendType)
                {
                    case BlendType.Add:
                        blendedMap[x, y] = Mathf.Clamp01(map1[x, y] + (map2[x, y] * modifier));
                        break;
                    case BlendType.Subtract:
                        blendedMap[x, y] = Mathf.Clamp01(map1[x, y] - (map2[x, y] * modifier));
                        break;
                    case BlendType.Multiply:
                        blendedMap[x, y] = map1[x,y] *= Mathf.Pow(map2[x, y], modifier);
                        break;
                    case BlendType.Divide:
                        blendedMap[x, y] = map1[x, y] /= Mathf.Pow(map2[x, y], modifier);
                        break;
                    case BlendType.Lighten:
                        blendedMap[x, y] = map1[x, y] > map2[x, y] ? map1[x, y] : map2[x, y];
                        break;
                    case BlendType.Darken:
                        blendedMap[x, y] = map1[x, y] < map2[x, y] ? map1[x, y] : map2[x, y];
                        break;
                    default:
                        blendedMap[x, y] = map1[x,y];
                        break;
                }
            }
        }

        return blendedMap;
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
