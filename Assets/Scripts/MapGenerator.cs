using System.Collections;
using System.Collections.Generic;
using UnityEditor.AssetImporters;
using UnityEngine;
using static UnityEditor.U2D.ScriptablePacker;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { HeightMap, ColorMap };
    public DrawMode drawMode;

    public NoiseData noiseData;
    public TerrainData terrainData;
    public TextureData textureData;

    public Material terrainMaterial;

    public MapDisplay mapDisplay;

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
        float[,] heightMap = HeightMap.GenerateHeightMap(noiseData, terrainData);

        switch (drawMode)
        {
            case DrawMode.HeightMap:
                mapDisplay.DrawTexture(TextureGenerator.FromHeightMap(heightMap));
                break;
            case DrawMode.ColorMap:
                Debug.Log("Draw ColorMap not implemented");
                // mapDisplay.DrawTexture(TextureGenerator.FromHeightMap(heightMap));
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
    }
}
