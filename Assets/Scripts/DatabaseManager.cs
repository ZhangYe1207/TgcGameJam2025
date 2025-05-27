using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    public CardDatabase cardDatabase;
    public EventDatabase eventDatabase;
    public ProjectDatabase projectDatabase;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            cardDatabase = Resources.Load<CardDatabase>("Databases/CardDatabase");
            eventDatabase = Resources.Load<EventDatabase>("Databases/EventDatabase");
            projectDatabase = Resources.Load<ProjectDatabase>("Databases/ProjectDatabase");
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
