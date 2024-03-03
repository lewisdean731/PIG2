using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Erosion
{
    public static float[,] ApplyHydraulicErosion(float[,] heightMap, float[,] humidityMap, float[,] temperatureMap, ErosionData eData, TerrainData tData)
    {
        int currentCycle = 0;
        for (int i = 0; i < eData.cycles; i++)
        {
#if UNITY_EDITOR
            EditorUtility.DisplayProgressBar("Applying Hydraulic Erosion", "Processing Cycle " + currentCycle + "/" + eData.cycles + "...", (float)currentCycle / (float)eData.cycles);
#endif
            for (int j = 0; j < eData.runsPerCycle; j++)
            {
                for (int k = 0; k < eData.runsPerCycle; k++)
                {
                    heightMap = ApplyHydraulicErosionRun(heightMap, humidityMap, temperatureMap, eData, tData);
                }
            }
            currentCycle++;
        }
#if UNITY_EDITOR
        EditorUtility.ClearProgressBar();
#endif
        return heightMap;
    }

    static float[,] ApplyHydraulicErosionRun(float[,] heightMap, float[,] humidityMap, float[,] temperatureMap, ErosionData eData, TerrainData tData)
    {
        Vector2 currentPos = GetRandomPointOnMap(heightMap, tData.seaLevel);
        Vector2 prevPos = currentPos;
        int cpx = (int)currentPos.x;
        int cpy = (int)currentPos.y;
        float size = 0.00f;
        float sizeThreshold = 0.06f;
        float slopeThreshold = 0.005f;
        float erodePercent = 0.03f;

        for (int i = 0; i < eData.stepsPerRun; i++)
        {
            // work out where to go next
            Vector2 nextPos = GetLowestNeighbour(heightMap, currentPos, prevPos);
            int npx = (int)nextPos.x;
            int npy = (int)nextPos.y;

            float slope = heightMap[cpx, cpy] - heightMap[npx, npy];
            // do erosion for current point
            // pick up material (erode) if below size / slope threshold
            if (size < sizeThreshold)
            {
                size += slope * erodePercent;
                heightMap[cpx, cpy] -= slope * erodePercent;
            }
            // drop off material (deposit) if above size / slope threshold
            else
            {
                size -= size * erodePercent;
                heightMap[cpx, cpy] += size * erodePercent;
            }

            if (heightMap[npx, npy] < tData.seaLevel) break;

            prevPos = currentPos;
            currentPos = nextPos;
        }

        return heightMap;
    }


    public static Vector2 GetRandomPointOnMap(float[,] map, float minValue = 0, float maxValue = 1)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        while (true)
        {
        int x = Random.Range(0, width);
        int y = Random.Range(0, height);

        if (map[x,y] >= minValue && map[x,y] <= maxValue) return new Vector2(x,y);
        }
    }

    public static Vector2 GetLowestNeighbour(float[,] map, Vector2 c, Vector2 p)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        int x = (int)c.x;
        int y = (int)c.y;
        float tl = x > 0 && y < height ? map[x - 1, y + 1] : float.MaxValue;
        float tm = x >= 0 && y < height ? map[x, y + 1] : float.MaxValue;
        float tr = x < width && y < height ? map[x + 1, y + 1] : float.MaxValue;
        float mr = x > 0 && y < height ? map[x + 1, y] : float.MaxValue;
        float br = x > 0 && y < height ? map[x + 1, y - 1] : float.MaxValue;
        float bm = x > 0 && y < height ? map[x, y - 1] : float.MaxValue;
        float bl = x > 0 && y < height ? map[x - 1, y - 1] : float.MaxValue;
        float ml = x > 0 && y < height ? map[x - 1, y] : float.MaxValue;

        float lowestValue = tl;
        Vector2 lowestPoint = new Vector2(x - 1, y + 1);

        if (tm < lowestValue && c != p)
        {
            lowestValue = tm;
            lowestPoint = new Vector2(x, y + 1);
        }
        if (tr < lowestValue && c != p)
        {
            lowestValue = tr;
            lowestPoint = new Vector2(x + 1, y + 1);
        }
        if (mr < lowestValue && c != p)
        {
            lowestValue = mr;
            lowestPoint = new Vector2(x + 1, y);
        }
        if (br < lowestValue && c != p)
        {
            lowestValue = br;
            lowestPoint = new Vector2(x + 1, y - 1);
        }
        if (bm < lowestValue && c != p)
        {
            lowestValue = bm;
            lowestPoint = new Vector2(x, y - 1);
        }
        if (bl < lowestValue && c != p)
        {
            lowestValue = bl;
            lowestPoint = new Vector2(x - 1, y - 1);
        }
        if (ml < lowestValue && c != p)
        {
            lowestPoint = new Vector2(x - 1, y);
        }

        return lowestPoint;

    }
}
