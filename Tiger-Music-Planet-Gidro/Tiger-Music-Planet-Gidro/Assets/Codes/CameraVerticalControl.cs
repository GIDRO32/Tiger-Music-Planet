using UnityEngine;

public class CameraVerticalControl : MonoBehaviour
{
    public float minY = -5f; // Minimum Y position for the camera
    public float maxY = 5f; // Maximum Y position for the camera
    public float speed = 5f; // Speed of camera movement

    private bool moveUp = false;
    private bool moveDown = false;

    void Update()
    {
        if (moveUp)
        {
            MoveCameraUp();
        }
        if (moveDown)
        {
            MoveCameraDown();
        }
    }

    public void StartMovingUp()
    {
        moveUp = true;
    }

    public void StopMovingUp()
    {
        moveUp = false;
    }

    public void StartMovingDown()
    {
        moveDown = true;
    }

    public void StopMovingDown()
    {
        moveDown = false;
    }

    private void MoveCameraUp()
    {
        float newYPosition = Mathf.Clamp(transform.position.y + speed * Time.deltaTime, minY, maxY);
        transform.position = new Vector3(transform.position.x, newYPosition, transform.position.z);
    }

    private void MoveCameraDown()
    {
        float newYPosition = Mathf.Clamp(transform.position.y - speed * Time.deltaTime, minY, maxY);
        transform.position = new Vector3(transform.position.x, newYPosition, transform.position.z);
    }
}
