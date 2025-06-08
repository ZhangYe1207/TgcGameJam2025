using UnityEngine;
using Cinemachine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ThisVirtualCameraSettings
{
    public Vector3 followOffset;
    public float fov;
    public Vector3 position;
    public Vector3 rotation;
}

[System.Serializable]
public class LevelData
{
    public VirtualCameraSettings overview = new VirtualCameraSettings();
    public VirtualCameraSettings follow = new VirtualCameraSettings();
    public Vector3 mainLightPosition;
    public Vector3 mainLightRotation;
    public GameObject airWallRoot;
}

public class NextLevelController : MonoBehaviour
{
    [Header("References")]
    public CinemachineVirtualCamera overviewCam;
    public CinemachineVirtualCamera followCam;
    public Transform player;

    [Header("Scene References")]
    public Transform mainLight;

    [Header("Level Settings")]
    public CinemachineLevelData[] levels;

    [Header("Transition Settings")]
    public float transitionDelay = 1.5f;

    private int currentLevelIndex = 0;

    void Start()
    {
        if (levels.Length > 0)
        {
            ApplySettingsToCam(followCam, levels[currentLevelIndex].follow);
            ActivateCamera(followCam);
            ApplyLevelExtras(currentLevelIndex);

        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            currentLevelIndex++;
            if (currentLevelIndex < levels.Length)
            {
                StartCoroutine(SwitchWithOverview(currentLevelIndex));
            }
            else
            {
                Debug.Log("✅ 已到最后一关");
            }
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            ApplySettingsToCam(overviewCam, levels[currentLevelIndex].overview);
            ActivateCamera(overviewCam);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            ApplySettingsToCam(followCam, levels[currentLevelIndex].follow);
            ActivateCamera(followCam);
        }
    }

    IEnumerator SwitchWithOverview(int index)
    {
        ApplySettingsToCam(overviewCam, levels[index].overview);
        ActivateCamera(overviewCam);
        ApplyLevelExtras(index);

        yield return new WaitForSeconds(transitionDelay);

        ApplySettingsToCam(followCam, levels[index].follow);
        ActivateCamera(followCam);
    }

    public void ApplySettingsToCam(CinemachineVirtualCamera cam, VirtualCameraSettings settings)
    {
        var transposer = cam.GetCinemachineComponent<CinemachineTransposer>();
        if (transposer != null)
        {
            transposer.m_FollowOffset = settings.followOffset;
        }
        cam.m_Lens.FieldOfView = settings.fov;
        cam.transform.position = settings.position;
        cam.transform.rotation = Quaternion.Euler(settings.rotation);
    }

    public void ActivateCamera(CinemachineVirtualCamera target)
    {
        overviewCam.gameObject.SetActive(target == overviewCam);
        followCam.gameObject.SetActive(target == followCam);
    }






    public void ApplyLevelExtras(int index)
    {
        if (mainLight != null)
        {
            mainLight.position = levels[index].mainLightPosition;
            mainLight.rotation = Quaternion.Euler(levels[index].mainLightRotation);

            Light lightComponent = mainLight.GetComponent<Light>();
            if (lightComponent != null)
            {
                lightComponent.intensity = levels[index].mainLightIntensity;
                lightComponent.range = levels[index].mainLightRange;
            }
        }


        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i].airWallRoot != null)
            {
                levels[i].airWallRoot.SetActive(i == index);
            }
        }
    }





    public int GetCurrentLevelIndex() => currentLevelIndex;







    /// <summary>
    /// ////////
    /// </summary>

#if UNITY_EDITOR
    public void RecordCurrentToFollow()
    {
        EnsureLevelExists();
        var data = levels[currentLevelIndex].follow;
        var transposer = followCam.GetCinemachineComponent<CinemachineTransposer>();
        if (transposer != null)
        {
            data.followOffset = transposer.m_FollowOffset;
        }
        data.fov = followCam.m_Lens.FieldOfView;
        data.position = followCam.transform.position;
        data.rotation = followCam.transform.eulerAngles;
        EditorUtility.SetDirty(this);
        Debug.Log($"✅ 记录 Follow 虚机快照（关卡 {currentLevelIndex}）成功");
    }

    public void RecordCurrentToOverview()
    {
        EnsureLevelExists();
        var data = levels[currentLevelIndex].overview;
        var transposer = overviewCam.GetCinemachineComponent<CinemachineTransposer>();
        if (transposer != null)
        {
            data.followOffset = transposer.m_FollowOffset;
        }
        data.fov = overviewCam.m_Lens.FieldOfView;
        data.position = overviewCam.transform.position;
        data.rotation = overviewCam.transform.eulerAngles;
        EditorUtility.SetDirty(this);
        Debug.Log($"记录 Overview 虚机快照（关卡 {currentLevelIndex}）成功");
    }

  


    public void RecordMainLight(Transform light)
    {
        EnsureLevelExists();

        levels[currentLevelIndex].mainLightPosition = light.position;
        levels[currentLevelIndex].mainLightRotation = light.eulerAngles;

        Light lightComponent = light.GetComponent<Light>();
        if (lightComponent != null)
        {
            levels[currentLevelIndex].mainLightIntensity = lightComponent.intensity;
            levels[currentLevelIndex].mainLightRange = lightComponent.range;
        }

        Debug.Log($" 记录主灯光位置 + 强度 + 范围成功（关卡 {currentLevelIndex}）");
        EditorUtility.SetDirty(this);
    }


    public void ClearAllLevels()
    {
        levels = new CinemachineLevelData[0];
        EditorUtility.SetDirty(this);
        Debug.Log("清空所有关卡数据");
    }

    private void EnsureLevelExists()
    {
        if (levels == null || levels.Length == 0)
        {
            levels = new CinemachineLevelData[1] { new CinemachineLevelData() };
            currentLevelIndex = 0;
        }
        else if (currentLevelIndex >= levels.Length)
        {
            var newLevels = new CinemachineLevelData[currentLevelIndex + 1];
            levels.CopyTo(newLevels, 0);
            for (int i = levels.Length; i < newLevels.Length; i++)
            {
                newLevels[i] = new CinemachineLevelData();
            }
            levels = newLevels;
        }
    }
#endif
}
