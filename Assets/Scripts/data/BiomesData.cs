using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BiomesData : UpdateableData
{
    public Biome[] biomes;
}

public enum BiomeType { 
    // Aquatic
    Aquatic_Icy,
    Aquatic_DeepOcean,
    Aquatic_ShallowOcean,
    Aquatic_Coast,
    Aquatic_Beach,
    // Desert
    Desert_Hot,
    Desert_Scrub,
    Desert_Savannah,
    // Grassland
    Grassland_Steppe,
    Grassland_Plain,
    Grassland_Shrubland,
    // Forest
    Forest_Temperate,
    Forest_Boreal,
    Forest_Rainforest,
    // Tundra
    Tundra_Taiga,
    Tundra_Arctic,
    // Mountain
    Mountain_Foothills,
    Mountain_High,
    Mountain_Peak
}

public enum GradientEvaluator { Humidity, Temperature }

[System.Serializable]
public class Biome
{
    public BiomeType biomeType;
    public Gradient humidityGradient;
    public GradientEvaluator gradientEvaluator;
    // public Color color;
    [Range(0f, 1f)]
    public float fromHeight;
    [Range(-40, 50)]
    public int fromTempC;
    [Range(0f, 1f)]
    public float fromHumidity;
}