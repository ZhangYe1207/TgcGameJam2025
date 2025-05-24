using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public CardDatabase cardDatabase;
    public EventDatabase eventDatabase;
    
    private void Awake()
    {
        // 确保单例唯一
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            cardDatabase = Resources.Load<CardDatabase>("Databases/CardDatabase");
            eventDatabase = Resources.Load<EventDatabase>("Databases/EventDatabase");
        }
        else
        {
            Destroy(gameObject);
        }
    }
}