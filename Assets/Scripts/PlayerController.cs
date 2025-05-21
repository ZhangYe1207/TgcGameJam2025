using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 15f;
    public float maxSpeed = 30f;
    public float decelerationForce = 10f; // Force applied when no input
    public bool isLocked = false;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // 如果玩家被锁定，则不移动
        if (isLocked) return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        
        // Check if there's any input
        if (Mathf.Abs(moveX) < 0.1f && Mathf.Abs(moveZ) < 0.1f)
        {
            // Apply deceleration force in the opposite direction of current velocity
            if (rb.velocity.magnitude > 0.1f)
            {
                rb.AddForce(-rb.velocity.normalized * decelerationForce, ForceMode.Force);
            }
        }
        else
        {
            Vector3 move = new Vector3(moveX, 0, moveZ) * speed;
            rb.AddForce(move, ForceMode.Force);
        }

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }
}
