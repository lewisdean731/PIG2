using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMaps
{
    public float[,] heightMap;
    public float[,] temperatureMap;
    public float[,] temperatureMap01;
    public float[,] humidityMap;
    public BiomeType[,] biomeMap;

    public TerrainMaps(int mapWidth, int mapHeight)
    {
        this.heightMap = new float[mapWidth, mapHeight];
        this.temperatureMap = new float[mapWidth, mapHeight];
        this.temperatureMap01 = new float[mapWidth, mapHeight];
        this.humidityMap = new float[mapWidth, mapHeight];
        this.biomeMap = new BiomeType[mapWidth, mapHeight];
    }

    public void Deconstruct(
        out float[,] heightMap,
        out float[,] temperatureMap,
        out float[,] temperatureMap01,
        out float[,] humidityMap,
        out BiomeType[,] biomeMap)
    {
        heightMap = this.heightMap;
        temperatureMap = this.temperatureMap;
        temperatureMap01 = this.temperatureMap01;
        humidityMap = this.humidityMap;
        biomeMap = this.biomeMap;
    }
}
