using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AssetImporters;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public NoiseData noiseData;
    public TerrainData terrainData;
    public TemperatureData temperatureData;
    public ErosionData erosionData;
    public BiomesData biomesData;

    public Material terrainMaterial;

    public MapDisplay mapDisplay;

    [SerializeField]
    public TerrainMaps terrainMaps;

    public enum DrawMode
    {
        ColorMap,
        HeightMap,
        HeightMapRGB,
        TemperatureMap,
        EquatorMap,
        HumidityMap,
        RawBiomeMap,
        BiomeMap,
        WaterMouintainMap
    };

    public DrawMode drawMode;

    public bool autoUpdate;
    public bool regenerateTerrainMaps;

    // Start is called before the first frame update
    private void Awake()
    {
        DrawMap();
    }

    private void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            if (autoUpdate)
            {
                DrawMap();
            }
        }
    }

    public void DrawMap()
    {
        if (regenerateTerrainMaps || terrainMaps == null)
        {
            terrainMaps = GenerateTerrainMaps(noiseData, terrainData, temperatureData, biomesData);
        }

        (float[,] heightMap, float[,] temperatureMap, float[,] temperatureMap01, float[,] humidityMap, BiomeType[,] biomeMap) = terrainMaps;

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

            case DrawMode.WaterMouintainMap:
                mapDisplay.DrawTexture(TextureGenerator.WaterMouintainMap(heightMap, terrainData));
                break;

            default:
                break;
        }
    }

    public TerrainMaps GenerateTerrainMaps(NoiseData noiseData, TerrainData terrainData, TemperatureData temperatureData, BiomesData biomeData)
    {
        int mapWidth = terrainData.tileCountX * MapMetrics.tileSize;
        int mapHeight = terrainData.tileCountY * MapMetrics.tileSize;

        // initialise maps
        TerrainMaps terrainMaps = new TerrainMaps(mapWidth, mapHeight);
        (
            float[,] heightMap,
            float[,] temperatureMap,
            float[,] temperatureMap01,
            float[,] humidityMap,
            BiomeType[,] biomeMap
        ) = terrainMaps;

        // generate height noise
        heightMap = HeightMap.GenerateHeightMap(noiseData, terrainData);

        // generate temperature map
        (temperatureMap, temperatureMap01) = TemperatureMap.GenerateTemperatureMaps(heightMap, terrainData, temperatureData);

        // generate humidity map
        humidityMap = HumidityMap.GenerateHumidityMap(heightMap, temperatureMap01, noiseData, terrainData, temperatureData);

        // hydraulic erosion
        heightMap = Erosion.ApplyHydraulicErosion(heightMap, humidityMap, temperatureMap01, erosionData, terrainData);

        // generate biomes
        biomeMap = BiomeMap.GenerateBiomeMap(heightMap, temperatureMap01, humidityMap, noiseData, terrainData, temperatureData, biomeData);

        // restructure;
        terrainMaps.heightMap = heightMap;
        terrainMaps.temperatureMap = temperatureMap;
        terrainMaps.temperatureMap01 = temperatureMap01;
        terrainMaps.humidityMap = humidityMap;
        terrainMaps.biomeMap = biomeMap;

        return terrainMaps;
    }

    private void OnValidate()
    {
        if (terrainData != null)
        {
            // unsub and resub so we don't start adding multiple subscriptions
            // every update camera controller probably does this better?
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
        if (erosionData != null)
        {
            erosionData.onValuesUpdated -= OnValuesUpdated;
            erosionData.onValuesUpdated += OnValuesUpdated;
        }
        if (biomesData != null)
        {
            biomesData.onValuesUpdated -= OnValuesUpdated;
            biomesData.onValuesUpdated += OnValuesUpdated;
        }
    }
}