using UnityEngine;

public class QuadSpriteAnimator : MonoBehaviour
{
    public Texture[] frames;           // 帧数组
    public float frameRate = 10f;      // 每秒帧率

    private Renderer rend;
    private int index = 0;
    private float timer = 0f;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (frames.Length > 0)
            rend.material.mainTexture = frames[0];
    }

    void Update()
    {
        if (frames.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= 1f / frameRate)
        {
            index = (index + 1) % frames.Length;
            rend.material.mainTexture = frames[index];
            timer = 0f;
        }
    }
}
