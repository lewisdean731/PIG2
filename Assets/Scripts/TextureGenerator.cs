using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class TextureGenerator
{
    public static Texture2D FromColourMap(Color[] colourMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }
    public static Texture2D FromHeightMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        return FromColourMap(colourMap, width, height);
    }

    public static Texture2D WaterMouintainMap(float[,] heightMap, TerrainData tData)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (heightMap[x, y] == 0 || heightMap[x, y] == 1)
                {
                    colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
                } else
               if (heightMap[x, y] <= tData.seaLevel)
                {
                    float depth = Mathf.InverseLerp(0, tData.seaLevel, heightMap[x, y]);
                    colourMap[y * width + x] = Color.Lerp(Color.blue, Color.cyan, depth);
                } else
                if (heightMap[x, y] >= 0.7f)
                {
                    float altitude = Mathf.InverseLerp(0.7f, 1, heightMap[x, y]);
                    colourMap[y * width + x] = Color.Lerp(Color.yellow, Color.red, altitude);
                }
                else
                {
                    float altitude = Mathf.InverseLerp(tData.seaLevel, 1, heightMap[x, y]);
                    colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, altitude);
                }
            }
        }

        return FromColourMap(colourMap, width, height);
    }

    public static Texture2D FromEquatorMap(float[,] eMap, TemperatureData tData)
    {
        int width = eMap.GetLength(0);
        int height = eMap.GetLength(1);

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colourMap[y * width + x] = tData.debugTempVisualisation.Evaluate(eMap[x, y]);
            }
        }

        return FromColourMap(colourMap, width, height);
    }
    public static Texture2D FromTemperatureMap(float[,] tMap, TemperatureData tData)
    {
        int width = tMap.GetLength(0);
        int height = tMap.GetLength(1);

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Scale temperature between 0-1
                float scaledTemp = Mathf.InverseLerp(tData.minTemperatureC, tData.maxTemperatureC, tMap[x, y]);
                colourMap[y * width +  x] = tData.debugTempVisualisation.Evaluate(scaledTemp);
            }
        }

        return FromColourMap(colourMap, width, height);
    }

    public static Texture2D FromHumiditiyMap(float[,] hMap)
    {
        int width = hMap.GetLength(0);
        int height = hMap.GetLength(1);

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colourMap[y * width + x] = Color.Lerp(Color.white, Color.blue, hMap[x,y]);
            }
        }

        return FromColourMap(colourMap, width, height);
    }

    public static Texture2D FromBiomeMap(BiomeType[,] bMap, BiomesData bData, float[,] hMap, float[,] tMap01)
    {
        int width = bMap.GetLength(0);
        int height = bMap.GetLength(1);

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                BiomeType biomeType = bMap[x,y];

                Biome biome = null;

                foreach(Biome b in bData.biomes)
                {
                    if(b.biomeType == biomeType)
                    {
                        biome = b;
                    }
                }

                if(biome == null)
                {
                    throw new UnassignedReferenceException("Biome not found");
                }

                // get colour at humidity / temp
                float htValue = biome.gradientEvaluator == GradientEvaluator.Humidity ? hMap[x, y] : tMap01[x, y];
                colourMap[y * width + x] = biome.humidityGradient.Evaluate(htValue);
            }
        }

        return FromColourMap(colourMap, width, height);
    }

    public static Texture2D FromRawBiomeMap(BiomeType[,] bMap, BiomesData bData)
    {
        int width = bMap.GetLength(0);
        int height = bMap.GetLength(1);

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                BiomeType biomeType = bMap[x, y];

                Biome biome = null;

                foreach (Biome b in bData.biomes)
                {
                    if (b.biomeType == biomeType)
                    {
                        biome = b;
                    }
                }

                if (biome == null)
                {
                    throw new UnassignedReferenceException("Biome not found");
                }

                // get colour at 50% humidity
                colourMap[y * width + x] = biome.humidityGradient.Evaluate(0.5f);
            }
        }

        return FromColourMap(colourMap, width, height);
    }
}

