using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AssetImporters;
using UnityEngine;
using static UnityEditor.U2D.ScriptablePacker;

public class MapGenerator : MonoBehaviour
{

    public NoiseData noiseData;
    public TerrainData terrainData;
    public TemperatureData temperatureData;
    public BiomesData biomesData;

    public Material terrainMaterial;

    public MapDisplay mapDisplay;

    [SerializeField]

    public float[] debugMapValues;
    public enum DrawMode { ColorMap, HeightMap, HeightMapRGB, TemperatureMap, EquatorMap, HumidityMap, RawBiomeMap, BiomeMap };
    public DrawMode drawMode;
    
    public bool autoUpdate;

    // Start is called before the first frame update
    void Awake()
    {
        DrawMap();
    }

    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            DrawMap();
        }
    }

    public void DrawMap()
    {
        (float[,] heightMap, float[,] temperatureMap, float[,] temperatureMap01, float[,] humidityMap, BiomeType[,] biomeMap) = HeightMap.GenerateTerrainMaps(noiseData, terrainData, temperatureData, biomesData);

        switch (drawMode)
        {
            case DrawMode.ColorMap:
                Debug.Log("Draw ColorMap not implemented");
                // mapDisplay.DrawTexture(TextureGenerator.FromHeightMap(heightMap));
                break;
            case DrawMode.HeightMap:
                mapDisplay.DrawTexture(TextureGenerator.FromHeightMap(heightMap));
                break;
            case DrawMode.HeightMapRGB:
                mapDisplay.DrawTexture(TextureGenerator.FromEquatorMap(heightMap, temperatureData));
                break;
            case DrawMode.TemperatureMap:
                mapDisplay.DrawTexture(TextureGenerator.FromTemperatureMap(temperatureMap, temperatureData));
                break;
            case DrawMode.EquatorMap:
                Debug.Log("Draw EquatorMap not implemented");
                // mapDisplay.DrawTexture(TextureGenerator.FromEquatorMap(heightMap, temperatureData));
                break;
            case DrawMode.HumidityMap:
                mapDisplay.DrawTexture(TextureGenerator.FromHumiditiyMap(humidityMap));
                break;
            case DrawMode.RawBiomeMap:
                mapDisplay.DrawTexture(TextureGenerator.FromRawBiomeMap(biomeMap, biomesData));
                break;
            case DrawMode.BiomeMap:
                mapDisplay.DrawTexture(TextureGenerator.FromBiomeMap(biomeMap, biomesData, humidityMap, temperatureMap01));
                break;
            default:
                break;
        }
    }

    void OnValidate()
    {
        if (terrainData != null)
        {
            // unsub and resub so we don't start adding multiple subscriptions every update
            // camera controller probably does this better?
            terrainData.onValuesUpdated -= OnValuesUpdated;
            terrainData.onValuesUpdated += OnValuesUpdated;
        }
        if (noiseData != null)
        {
            noiseData.onValuesUpdated -= OnValuesUpdated;
            noiseData.onValuesUpdated += OnValuesUpdated;
        }
        if (temperatureData != null)
        {
            temperatureData.onValuesUpdated -= OnValuesUpdated;
            temperatureData.onValuesUpdated += OnValuesUpdated;
        }
        if (biomesData != null)
        {
            biomesData.onValuesUpdated -= OnValuesUpdated;
            biomesData.onValuesUpdated += OnValuesUpdated;
        }
    }
}
