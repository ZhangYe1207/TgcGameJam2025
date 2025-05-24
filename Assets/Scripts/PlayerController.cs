using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float speed = 15f;
    public float maxSpeed = 30f;
    public float acceleration = 40f;
    public float deceleration = 60f;
    public bool isLocked = false;

    private Rigidbody rb;
    private Vector3 inputDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
    }

    void FixedUpdate()
    {
        if (isLocked) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        inputDirection = new Vector3(moveX, 0, moveZ).normalized;

        Vector3 currentVelocity = rb.velocity;
        Vector3 targetVelocity = inputDirection * speed;

        Vector3 velocityChange;

        if (inputDirection != Vector3.zero)
        {
            // 加速
            velocityChange = Vector3.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime) - currentVelocity;
        }
        else
        {
            // 减速（强制将速度逐渐归零）
            velocityChange = Vector3.MoveTowards(currentVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime) - currentVelocity;
        }

        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        // 限制最大速度
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
            rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);
        }
    }
}

