#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(NextLevelController))]
public class LevelControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NextLevelController controller = (NextLevelController)target;

        GUILayout.Space(10);
        GUI.backgroundColor = Color.cyan;

        if (GUILayout.Button("📍 记录 Follow 虚机快照"))
        {
            controller.RecordCurrentToFollow();
        }

        if (GUILayout.Button("记录 Overview 虚机快照"))
        {
            controller.RecordCurrentToOverview();
        }

        if (GUILayout.Button("记录主灯光位置"))
        {
            controller.RecordMainLight(controller.mainLight);
        }

        GUI.backgroundColor = Color.red;

        if (GUILayout.Button("清空所有关卡数据"))
        {
            if (EditorUtility.DisplayDialog("确认清空？", "是否清空所有 LevelData？此操作不可撤销。", "确认", "取消"))
            {
                controller.ClearAllLevels();
            }
        }

        GUI.backgroundColor = Color.yellow;

        if (GUILayout.Button(" 应用当前关卡（测试切换）"))
        {
            controller.ApplySettingsToCam(controller.followCam, controller.levels[controller.GetCurrentLevelIndex()].follow);
            controller.ActivateCamera(controller.followCam);
#if UNITY_EDITOR
            controller.ApplyLevelExtras(controller.GetCurrentLevelIndex());
#endif
        }

        GUI.backgroundColor = Color.white;
    }
}
#endif
