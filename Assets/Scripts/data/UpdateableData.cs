using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateableData : ScriptableObject
{
    public event System.Action onValuesUpdated;
    public bool autoUpdate;

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (autoUpdate)
        {
            UnityEditor.EditorApplication.update += notifyOfUpdatedValues;
        }
    }

    public void notifyOfUpdatedValues()
    {
        UnityEditor.EditorApplication.update -= notifyOfUpdatedValues;
        if (onValuesUpdated != null)
        {
            onValuesUpdated();
        }
    }

#endif
}
