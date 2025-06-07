#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;

[System.Serializable]
public class LevelData
{
    [Header("Camera Settings")]
    public Vector3 cameraPosition;
    public Vector3 cameraRotation;

    [Header("Spot Light Settings")]
    public Vector3 spotLightPosition;
    public Vector3 spotLightRotation;
}

public class NextLevelController : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public Light spotLight;

    [Header("Level Configurations")]
    public LevelData[] levels;

    [Header("Transition Settings")]
    [SerializeField] private float transitionDuration = 1.0f;

    private int currentLevelIndex = 0;
    private bool isTransitioning = false;

    void Start()
    {
        if (levels != null && levels.Length > 0)
        {
            ApplyLevelInstant(0);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !isTransitioning)
        {
            currentLevelIndex++;
            if (currentLevelIndex < levels.Length)
            {
                StartCoroutine(TransitionToLevel(currentLevelIndex));
            }
            else
            {
                Debug.Log("No more levels.");
            }
        }
    }

    void ApplyLevelInstant(int index)
    {
        mainCamera.transform.position = levels[index].cameraPosition;
        mainCamera.transform.rotation = Quaternion.Euler(levels[index].cameraRotation);
        spotLight.transform.position = levels[index].spotLightPosition;
        spotLight.transform.rotation = Quaternion.Euler(levels[index].spotLightRotation);
    }

    IEnumerator TransitionToLevel(int index)
    {
        isTransitioning = true;

        Vector3 startCamPos = mainCamera.transform.position;
        Quaternion startCamRot = mainCamera.transform.rotation;
        Vector3 startLightPos = spotLight.transform.position;
        Quaternion startLightRot = spotLight.transform.rotation;

        Vector3 targetCamPos = levels[index].cameraPosition;
        Quaternion targetCamRot = Quaternion.Euler(levels[index].cameraRotation);
        Vector3 targetLightPos = levels[index].spotLightPosition;
        Quaternion targetLightRot = Quaternion.Euler(levels[index].spotLightRotation);

        float timer = 0f;

        while (timer < transitionDuration)
        {
            float t = timer / transitionDuration;
            mainCamera.transform.position = Vector3.Lerp(startCamPos, targetCamPos, t);
            mainCamera.transform.rotation = Quaternion.Slerp(startCamRot, targetCamRot, t);
            spotLight.transform.position = Vector3.Lerp(startLightPos, targetLightPos, t);
            spotLight.transform.rotation = Quaternion.Slerp(startLightRot, targetLightRot, t);

            timer += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = targetCamPos;
        mainCamera.transform.rotation = targetCamRot;
        spotLight.transform.position = targetLightPos;
        spotLight.transform.rotation = targetLightRot;

        isTransitioning = false;
    }

#if UNITY_EDITOR
    public void AddLevelFromScene()
    {
        if (mainCamera == null || spotLight == null)
        {
            Debug.LogWarning("Main Camera or Spot Light is not assigned.");
            return;
        }

        LevelData newLevel = new LevelData
        {
            cameraPosition = mainCamera.transform.position,
            cameraRotation = mainCamera.transform.eulerAngles,
            spotLightPosition = spotLight.transform.position,
            spotLightRotation = spotLight.transform.eulerAngles
        };

        int len = levels != null ? levels.Length : 0;
        LevelData[] newLevels = new LevelData[len + 1];
        if (len > 0) levels.CopyTo(newLevels, 0);
        newLevels[len] = newLevel;
        levels = newLevels;

        Debug.Log($"✅ Captured new LevelData [{len}] from scene.");
        EditorUtility.SetDirty(this);
    }

    public void ClearAllLevels()
    {
        levels = new LevelData[0];
        Debug.Log("🗑️ Cleared all LevelData.");
        EditorUtility.SetDirty(this);
    }
#endif
}
