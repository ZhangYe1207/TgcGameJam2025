using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [System.Serializable]
    public class NamedClip
    {
        public string name;
        public AudioClip clip;
    }

    [Header("Audio Clips")]
    public NamedClip[] audioClips;

    private Dictionary<string, AudioClip> clipDict = new Dictionary<string, AudioClip>();
    private AudioSource audioSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();

        foreach (var entry in audioClips)
        {
            if (!clipDict.ContainsKey(entry.name))
                clipDict.Add(entry.name, entry.clip);
        }
    }

    public void Play(string name)
    {
        if (clipDict.ContainsKey(name))
        {
            audioSource.PlayOneShot(clipDict[name]);
        }
        else
        {
            Debug.LogWarning($"?? Sound '{name}' not found in SoundManager.");
        }
    }
}
