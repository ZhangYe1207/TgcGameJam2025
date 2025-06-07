using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NextLevelController))]
public class NextLevelControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NextLevelController controller = (NextLevelController)target;

        GUILayout.Space(10);
        GUI.backgroundColor = Color.cyan;

        if (GUILayout.Button("➕ 记录当前相机与灯光为新关卡"))
        {
            controller.AddLevelFromScene();
        }

        GUI.backgroundColor = Color.red;

        if (GUILayout.Button("🗑️ 清空所有关卡数据"))
        {
            if (EditorUtility.DisplayDialog("确认清空？", "是否清空所有 LevelData？此操作不可撤销。", "确认", "取消"))
            {
                controller.ClearAllLevels();
            }
        }

        GUI.backgroundColor = Color.white;
    }
}
