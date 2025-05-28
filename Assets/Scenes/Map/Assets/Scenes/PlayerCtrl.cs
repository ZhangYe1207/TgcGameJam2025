using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 获取输入
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(h, 0f, v).normalized;

        // 移动
        if (move.magnitude > 0)
        {
            transform.position += move * moveSpeed * Time.deltaTime;
            transform.forward = move;
        }

        // 设置动画参数
        animator.SetFloat("Speed", move.magnitude);
        Debug.Log("Speed = " + move.magnitude);
    }
}