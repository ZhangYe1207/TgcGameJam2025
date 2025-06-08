using UnityEngine;

[System.Serializable]
public class VirtualCameraSettings
{
    public Vector3 followOffset = new Vector3(0, 5, -10);
    public float fov = 60f;
    public Vector3 position = Vector3.zero;
    public Vector3 rotation = Vector3.zero;
}

[System.Serializable]
public class CinemachineLevelData
{
    [Header("Virtual Camera Settings")]
    public VirtualCameraSettings overview = new VirtualCameraSettings();
    public VirtualCameraSettings follow = new VirtualCameraSettings();

    [Header("Main Light Settings")]
    public Vector3 mainLightPosition = Vector3.zero;
    public Vector3 mainLightRotation = Vector3.zero;
    public float mainLightIntensity = 1f;
    public float mainLightRange = 10f;


    [Header("Air Wall Reference")]
    public GameObject airWallRoot;
}
