using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PostProcessingTestBuilder : EditorWindow
{
    [MenuItem("Tools/Build PostProcessing Test Scene")]
    public static void BuildScene()
    {
        // 创建 RenderTexture
        RenderTexture rt = new RenderTexture(1920, 1080, 24);
        rt.name = "TestRenderTexture";

        // 创建 Camera
        GameObject camGO = new GameObject("TestCamera");
        Camera cam = camGO.AddComponent<Camera>();
        camGO.tag = "MainCamera";
        camGO.transform.position = new Vector3(0, 1, -5);
        camGO.transform.rotation = Quaternion.identity;
        cam.targetTexture = rt;

        var cameraData = camGO.AddComponent<UniversalAdditionalCameraData>();
        cameraData.renderPostProcessing = true;

        // 创建一个发光球体
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = "GlowBall";
        sphere.transform.position = new Vector3(0, 1, 0);

        // 创建材质并启用 Emission
        Material glowMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        glowMat.color = Color.white;
        glowMat.EnableKeyword("_EMISSION");
        glowMat.SetColor("_EmissionColor", Color.white * 5f);
        sphere.GetComponent<Renderer>().material = glowMat;

        // 创建 Volume + Profile
        GameObject volumeGO = new GameObject("Global Volume");
        volumeGO.transform.position = Vector3.zero;
        Volume volume = volumeGO.AddComponent<Volume>();
        volume.isGlobal = true;
        volume.priority = 1;
        VolumeProfile profile = ScriptableObject.CreateInstance<VolumeProfile>();
        volume.sharedProfile = profile;

        // 添加 Bloom 效果
        var bloom = profile.Add<Bloom>();
        bloom.intensity.Override(10f);
        bloom.threshold.Override(1f);

        // 创建一个 Canvas + RawImage 显示 RenderTexture
        GameObject canvasGO = new GameObject("Canvas");
        canvasGO.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject rawImgGO = new GameObject("RawImage");
        rawImgGO.transform.parent = canvasGO.transform;
        RawImage rawImg = rawImgGO.AddComponent<RawImage>();
        rawImg.texture = rt;
        rawImg.rectTransform.anchorMin = Vector2.zero;
        rawImg.rectTransform.anchorMax = Vector2.one;
        rawImg.rectTransform.offsetMin = Vector2.zero;
        rawImg.rectTransform.offsetMax = Vector2.zero;

        Debug.Log("✅ Post-processing test scene created!");
    }
}
