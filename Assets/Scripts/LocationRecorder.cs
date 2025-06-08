using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class Location
{
    public Vector3 position;
    public int level;
}

public class LocationRecorder : MonoBehaviour
{
    public GameObject playerGO;
    public GameObject locationPrefab;

#if UNITY_EDITOR
    public void RecordPlayerLocation()
    {


        Location cur = new Location();
        cur.position = playerGO.transform.position;
        cur.level = GameManager.Instance.currentLevel;
        DatabaseManager.Instance.locationDatabase.AddLocation(cur);
        MarkLocation(cur);
        Debug.Log("记录当前玩家位置成功");
    }

    public void RecordRecorderLocation()
    {
        Location cur = new Location();
        cur.position = gameObject.transform.position;
        cur.level = GameManager.Instance.currentLevel;
        DatabaseManager.Instance.locationDatabase.AddLocation(cur);
        MarkLocation(cur);
        Debug.Log("记录当前位置成功");
    }

    private void MarkLocation(Location cur) {
        var locGO = Instantiate(locationPrefab, gameObject.transform);
        locGO.transform.position = cur.position;
    }

#endif
}
