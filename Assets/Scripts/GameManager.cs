using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public CardDatabase cardDatabase;
    
    private void Awake()
    {
        // 确保单例唯一
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            cardDatabase = Resources.Load<CardDatabase>("Databases/CardDatabase");
        }
        else
        {
            Destroy(gameObject);
        }
    }
}