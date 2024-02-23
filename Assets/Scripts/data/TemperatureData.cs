using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TemperatureData : UpdateableData
{

    [Range(-30, 40)]
    public float minTemperatureC = -10f;
    [Range(-30, 40)]
    public float maxTemperatureC = 35f;

    public Vector2 equatorPos = new Vector2(MapMetrics.tileSize / 2, MapMetrics.tileSize / 2);
    [Range(0, 360)]
    public float equatorRot = 90f;

    [Range(0, 5)]
    public float equatorTransition = 1;

    public Gradient debugTempVisualisation;
}
