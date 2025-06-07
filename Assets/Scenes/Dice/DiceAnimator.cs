using UnityEngine;
using System.Collections;

public class DiceAnimator : MonoBehaviour
{
    public Sprite faceTrue;   // ✔️
    public Sprite faceFalse;  // ❌
    
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Play(bool result)
    {
        StartCoroutine(PlayAnimationThenSetResult(result));
    }

    private IEnumerator PlayAnimationThenSetResult(bool result)
    {
        animator.enabled = true; // 启用 Animator 以播放动画
        animator.Play("DiceSpin", -1, 0f); // 播放动画 Clip，从头开始

        yield return new WaitForSeconds(0.7f); // 等待动画播放完一轮（依据你的动画长度）

        animator.enabled = false; // 停止 Animator，冻结当前帧

        // 切换为最终结果 Sprite
        spriteRenderer.sprite = result ? faceTrue : faceFalse;
    }
}
