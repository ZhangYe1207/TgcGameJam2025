using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;                // 控制移动速度
    public bool isLocked = false;           // 控制是否禁用输入
    private Rigidbody rb;
    public Transform spriteChild;
    private bool isWalking = false;

    private bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;           // 防止倾倒
    }

    void FixedUpdate()
    {
        if (isLocked)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");  // 用Raw更直接
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 moveInput = new Vector3(moveX, 0, moveZ).normalized;

        // 翻转处理
        if (spriteChild != null && moveX != 0)
        {
            if (moveX > 0 && !facingRight)
                Flip();
            else if (moveX < 0 && facingRight)
                Flip();
        }

        // 直接设置速度，不使用AddForce
        rb.velocity = moveInput * speed;
        //SoundManager.Instance.PlayMusic("Car");


        if (moveInput.magnitude > 0.1f)
        {
            if (!isWalking)
            {
                SoundManager.Instance.PlayMusic("Car");
                isWalking = true;
            }
        }
        else
        {
            if (isWalking)
            {
                SoundManager.Instance.FadeOutMusic("Car", 0.5f);
                isWalking = false;
            }
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = spriteChild.localScale;
        scale.x *= -1;
        spriteChild.localScale = scale;
    }
}

