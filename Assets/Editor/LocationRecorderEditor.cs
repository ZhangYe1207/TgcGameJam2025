#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(LocationRecorder))]
public class LocationRecorderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LocationRecorder controller = (LocationRecorder)target;

        GUILayout.Space(10);
        GUI.backgroundColor = Color.cyan;

        if (GUILayout.Button("记录当前玩家location"))
        {
            controller.RecordPlayerLocation();
        }

        if (GUILayout.Button("记录当前Recorder location"))
        {
            controller.RecordRecorderLocation();
        }

        if (GUILayout.Button("根据LocationRecorder刷新Location Database"))
        {
            controller.RefreshLocationDatabase();
        }
    }
}
#endif
