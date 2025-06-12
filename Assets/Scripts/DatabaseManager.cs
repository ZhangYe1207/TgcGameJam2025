using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    public CardDatabase cardDatabase;
    public EventDatabase eventDatabase;
    public ProjectDatabase projectDatabase;
    public LocationDatabase locationDatabase;
    public NPCDatabase npcDatabase;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            cardDatabase = Resources.Load<CardDatabase>("Databases/CardDatabase");
            eventDatabase = Resources.Load<EventDatabase>("Databases/EventDatabase");
            projectDatabase = Resources.Load<ProjectDatabase>("Databases/ProjectDatabase");
            locationDatabase = Resources.Load<LocationDatabase>("Databases/LocationDatabase");
            npcDatabase = Resources.Load<NPCDatabase>("Databases/NPCDatabase");
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
