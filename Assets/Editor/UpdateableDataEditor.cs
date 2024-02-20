using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UpdateableData), true)]
public class UpdateableDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        UpdateableData data = target as UpdateableData;

        if (GUILayout.Button("Update"))
        {
            data.notifyOfUpdatedValues();
            EditorUtility.SetDirty(target); // notifies something has changed
        }

    }
}