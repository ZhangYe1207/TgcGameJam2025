using UnityEngine;

public class TopDownCameraFollow : MonoBehaviour
{
    public Transform target; // Assign your player transform here in the Inspector
    public float smoothSpeed = 0.125f; // Adjust for smoother or quicker camera movement
    public Vector3 offset; // The desired offset from the player (e.g., new Vector3(0, 10, -5) for a typical top-down view)
    public float rotationAngleX = 45f; // Angle for the top-down perspective (e.g., 45-60 degrees)
    public float currentZoom = 10f; // Initial zoom level (Y offset)
    public float zoomSpeed = 4f;
    public float minZoom = 5f;
    public float maxZoom = 15f;

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("Camera target not set!");
            return;
        }

        // Handle Zoom (optional - using mouse scroll wheel)
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // Update offset based on zoom
        Vector3 dynamicOffset = new Vector3(offset.x, currentZoom, offset.z);

        // Desired position for the camera
        Vector3 desiredPosition = target.position + dynamicOffset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Ensure the camera is always looking down at the player from the specified angle
        transform.rotation = Quaternion.Euler(rotationAngleX, 0f, 0f);
    }
}