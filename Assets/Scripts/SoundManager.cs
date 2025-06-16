using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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
    private Dictionary<string, AudioSource> musicSources = new Dictionary<string, AudioSource>(); // 多个音乐通道
    private AudioSource sfxSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        sfxSource = gameObject.AddComponent<AudioSource>();

        foreach (var entry in audioClips)
        {
            if (!clipDict.ContainsKey(entry.name))
                clipDict.Add(entry.name, entry.clip);
        }
    }

    // 播放短音效
    public void Play(string name)
    {
        if (clipDict.ContainsKey(name))
            sfxSource.PlayOneShot(clipDict[name]);
    }

    // 播放背景音乐，可多个同时
    public void PlayMusic(string name, float volume = 0.5f, bool loop = true)
    {
        if (!clipDict.ContainsKey(name)) return;

        // 如果已经在播放就跳过
        if (musicSources.ContainsKey(name) && musicSources[name].isPlaying)
            return;

        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clipDict[name];
        source.volume = volume;
        source.loop = loop;
        source.Play();

        musicSources[name] = source;
    }

    // 停止指定名称的音乐
    public void StopMusic(string name)
    {
        if (musicSources.ContainsKey(name))
        {
            musicSources[name].Stop();
            Destroy(musicSources[name]); // 销毁该 AudioSource 组件
            musicSources.Remove(name);
        }
    }

    // 淡出停止指定名称的音乐
    public void FadeOutMusic(string name, float duration)
    {
        if (musicSources.ContainsKey(name))
            StartCoroutine(FadeOutCoroutine(name, duration));
    }

    private IEnumerator FadeOutCoroutine(string name, float duration)
    {
        AudioSource source = musicSources[name];
        float startVol = source.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVol, 0f, time / duration);
            yield return null;
        }

        source.Stop();
        Destroy(source); 

        musicSources.Remove(name);
    }
}

