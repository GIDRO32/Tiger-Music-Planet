using UnityEngine;

public class CameraVerticalControl : MonoBehaviour
{
    private Vector3 startTouchPosition;
    private Vector3 currentPosition;
    private Vector3 cameraStartPosition;
    private bool isDragging = false;

    // Define limits for camera movement
    public float minY = -5f; // Minimum Y position for the camera
    public float maxY = 5f; // Maximum Y position for the camera

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
                cameraStartPosition = transform.position;
                isDragging = true;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                currentPosition = touch.position;
                float distanceMoved = (startTouchPosition.y - currentPosition.y) * 0.01f; // Invert control
                
                // Calculate new Y position and clamp it within the specified range
                float newYPosition = Mathf.Clamp(cameraStartPosition.y + distanceMoved, minY, maxY);
                
                // Apply the clamped position to the camera
                transform.position = new Vector3(transform.position.x, newYPosition, transform.position.z);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isDragging = false;
            }
        }

        // The same logic applies for mouse input (editor testing)
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPosition = Input.mousePosition;
            cameraStartPosition = transform.position;
            isDragging = true;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            currentPosition = Input.mousePosition;
            float distanceMoved = (startTouchPosition.y - currentPosition.y) * 0.01f; // Invert control
            
            // Calculate new Y position and clamp it within the specified range
            float newYPosition = Mathf.Clamp(cameraStartPosition.y + distanceMoved, minY, maxY);
            
            // Apply the clamped position to the camera
            transform.position = new Vector3(transform.position.x, newYPosition, transform.position.z);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }
}
