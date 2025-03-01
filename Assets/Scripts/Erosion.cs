using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR

using UnityEditor;

#endif

public static class Erosion
{
    public static float[,] ApplyHydraulicErosion(
        float[,] heightMap,
        float[,] humidityMap,
        float[,] temperatureMap,
        ErosionData eData,
        TerrainData tData)
    {
        int currentCycle = 0;
        for (int i = 0; i < eData.cycles; i++)
        {
#if UNITY_EDITOR
            EditorUtility.DisplayProgressBar(
                "Applying Hydraulic Erosion",
                "Processing Cycle " + currentCycle + "/" + eData.cycles + "...",
                (float)currentCycle / (float)eData.cycles);
#endif
            for (int j = 0; j < eData.runsPerCycle; j++)
            {
                heightMap =
                    ApplyHydraulicErosionRun(heightMap, humidityMap, eData, tData);
            }
            currentCycle++;
        }
#if UNITY_EDITOR
        EditorUtility.ClearProgressBar();
#endif
        return heightMap;
    }

    private static float[,] ApplyHydraulicErosionRun(
    float[,] heightMap, float[,] humidityMap, ErosionData eData,
    TerrainData tData)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Vector2 currentPos = GetRandomPointOnMap(heightMap, tData.seaLevel);
        Vector2 prevPos = currentPos;
        int cpx = Mathf.FloorToInt(currentPos.x);
        int cpy = Mathf.FloorToInt(currentPos.y);

        // Get humidity at the starting point
        float humidity = humidityMap[cpx, cpy];

        // Use humidity to influence initial water volume
        float water = 1f + humidity * eData.humidityWaterMultiplier;

        float sediment = 0f; // Initial sediment volume

        for (int i = 0; i < eData.stepsPerRun; i++)
        {
            cpx = Mathf.FloorToInt(currentPos.x);
            cpy = Mathf.FloorToInt(currentPos.y);

            // Ensure the current position is within bounds
            if (cpx < 0 || cpx >= width || cpy < 0 || cpy >= height)
            {
                break; // Stop if out of bounds
            }

            // Find neighboring cells and their height differences
            List<NeighborData> neighbors = GetNeighborsWithHeightDifferences(
                heightMap, currentPos, prevPos);

            // Calculate total height difference for normalization
            float totalHeightDifference = 0;
            foreach (var neighbor in neighbors)
            {
                totalHeightDifference += neighbor.HeightDifference;
            }

            // Distribute water and sediment to neighbors based on height difference
            foreach (var neighbor in neighbors)
            {
                int npx = neighbor.X;
                int npy = neighbor.Y;

                // Calculate the proportion of water and sediment to transfer
                float proportion = totalHeightDifference > 0
                                       ? neighbor.HeightDifference / totalHeightDifference
                                       : 1f / neighbors.Count; // Distribute evenly if
                                                               // flat

                // Calculate sediment capacity based on flow (proportional water volume)
                float sedimentCapacity = Mathf.Max(0, neighbor.HeightDifference) *
                                         water * proportion * eData.capacityFactor;

                // Erosion/Deposition logic
                if (sediment > sedimentCapacity)
                {
                    // Deposition: Deposit sediment where capacity is lower
                    float depositAmount =
                        (sediment - sedimentCapacity) * eData.depositSpeed;
                    depositAmount = Mathf.Min(depositAmount, heightMap[npx, npy]);
                    heightMap[npx, npy] += depositAmount;
                    sediment -= depositAmount;
                }
                else
                {
                    // Erosion: Pick up sediment where capacity is higher
                    float erodeAmount =
                        Mathf.Min((sedimentCapacity - sediment) * eData.erosionSpeed,
                                  heightMap[cpx, cpy]);
                    heightMap[cpx, cpy] -= erodeAmount;
                    sediment += erodeAmount;
                }

                // Transport sediment downstream (proportional to water volume)
                float sedimentTransferAmount =
                    Mathf.Min(sediment, eData.transportSpeed * water * proportion);
                sediment -= sedimentTransferAmount;
                // The sediment is effectively moved to the neighbor in the next iteration
            }

            // Evaporation: Reduce water volume
            water *= (1 - eData.evaporationRate);

            // Stop if water volume is too low
            if (water <= 0)
            {
                break;
            }

            // Choose the next position randomly from the neighbors, weighted by
            // height difference
            if (neighbors.Count != 0)
            {
                // Keep the same position if no neighbours to move to
                currentPos = ChooseNextPosition(neighbors, totalHeightDifference);
            }

            cpx = Mathf.FloorToInt(currentPos.x);
            cpy = Mathf.FloorToInt(currentPos.y);

            prevPos = new Vector2(cpx, cpy);

            // Optionally stop if we hit sea level
            if (heightMap[cpx, cpy] < tData.seaLevel && !eData.simulateInWater)
            {
                break;
            }
        }

        return heightMap;
    }

    // Helper class to store neighbor data
    public class NeighborData
    {
        public int X;
        public int Y;
        public float HeightDifference;
    }

    // Function to get neighbors with height differences
    private static List<NeighborData> GetNeighborsWithHeightDifferences(
        float[,] map, Vector2 c, Vector2 p)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        int x = Mathf.FloorToInt(c.x);
        int y = Mathf.FloorToInt(c.y);

        List<NeighborData> neighbors = new List<NeighborData>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue; // Skip the current cell

                int nx = x + i;
                int ny = y + j;

                // Check bounds
                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                {
                    // Skip the previous position to avoid going back directly
                    if (nx == Mathf.FloorToInt(p.x) && ny == Mathf.FloorToInt(p.y))
                        continue;

                    float heightDifference = map[x, y] - map[nx, ny];
                    if (heightDifference > 0)
                    { // Only consider neighbors lower than the
                      // current cell
                        neighbors.Add(new NeighborData
                        {
                            X = nx,
                            Y = ny,
                            HeightDifference = heightDifference
                        });
                    }
                }
            }
        }

        return neighbors;
    }

    // Function to choose the next position based on height differences
    private static Vector2 ChooseNextPosition(List<NeighborData> neighbors,
                                     float totalHeightDifference)
    {
        if (neighbors.Count == 0)
        {
            return Vector2.negativeInfinity; // No valid neighbors
        }

        float randomValue = Random.value;
        float cumulativeProbability = 0;

        for (int i = 0; i < neighbors.Count; i++)
        {
            float probability = totalHeightDifference > 0
                                    ? neighbors[i].HeightDifference / totalHeightDifference
                                    : 1f / neighbors.Count;
            cumulativeProbability += probability;
            if (randomValue <= cumulativeProbability)
            {
                return new Vector2(neighbors[i].X, neighbors[i].Y);
            }
        }

        // Fallback: return the last neighbor if something goes wrong
        return new Vector2(neighbors[neighbors.Count - 1].X,
                           neighbors[neighbors.Count - 1].Y);
    }

    public static Vector2 GetRandomPointOnMap(float[,] map, float minValue = 0)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        while (true)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            if (map[x, y] >= minValue)
            {
                return new Vector2(x, y);
            }
        }
    }

    public static Vector2 GetLowestNeighbour(float[,] map, Vector2 c, Vector2 p)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        int x = Mathf.FloorToInt(c.x);
        int y = Mathf.FloorToInt(c.y);

        float lowestValue = float.MaxValue;
        Vector2 lowestPoint = c; // Default to current position

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue; // Skip the current cell

                int nx = x + i;
                int ny = y + j;

                // Check bounds
                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                {
                    // Skip the previous position to avoid going back
                    if (nx == Mathf.FloorToInt(p.x) && ny == Mathf.FloorToInt(p.y))
                        continue;

                    if (map[nx, ny] < lowestValue)
                    {
                        lowestValue = map[nx, ny];
                        lowestPoint = new Vector2(nx, ny);
                    }
                }
            }
        }

        return lowestPoint;
    }
}