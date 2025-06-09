using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    public float speed = 15f;
    public float maxSpeed = 30f;
    public float decelerationForce = 10f;
    public bool isLocked = false;

    private Rigidbody rb;
    public Transform spriteChild;

    private bool facingRight = false; // 默认朝右

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (isLocked)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // 自动左右翻转逻辑
        if (spriteChild != null)
        {
            if (moveX > 0.1f && !facingRight)
            {
                Flip();
            }
            else if (moveX < -0.1f && facingRight)
            {
                Flip();
            }
        }

        // 移动逻辑
        if (Mathf.Abs(moveX) < 0.1f && Mathf.Abs(moveZ) < 0.1f)
        {
            if (rb.velocity.magnitude > 0.1f)
                rb.AddForce(-rb.velocity.normalized * decelerationForce, ForceMode.Force);
        }
        else
        {
            Vector3 move = new Vector3(moveX, 0, moveZ) * speed;
            rb.AddForce(move, ForceMode.Force);
        }

        if (rb.velocity.magnitude > maxSpeed)
            rb.velocity = rb.velocity.normalized * maxSpeed;
    }

    void Flip()
    {
        // 切换方向状态
        facingRight = !facingRight;

        // 将 X 轴缩放乘以 -1 实现镜像翻转
        Vector3 scale = spriteChild.localScale;
        scale.x *= -1;
        spriteChild.localScale = scale;
    }
}
