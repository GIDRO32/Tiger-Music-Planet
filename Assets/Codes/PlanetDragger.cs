using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlanetDragger : MonoBehaviour
{
    public List<Collider2D> orbitColliders; // List of all orbit colliders
    private Vector3 offset;
    private bool isDragging = false;
    public Canvas planetWatcherCanvas; // Assign this in the Inspector

    void Update()
    {
        if (isDragging)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePos.x + offset.x, mousePos.y + offset.y, transform.position.z);
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            TransferToCanvas(this.gameObject, planetWatcherCanvas);
            ReleasePlanet();
        }
    }

    void OnMouseDown()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - mousePos;
        isDragging = true;
    }

    void ReleasePlanet()
    {
        isDragging = false;
        bool isInsideOrbit = orbitColliders.Exists(orbitCollider => orbitCollider.OverlapPoint(transform.position));

        if (isInsideOrbit)
        {
            // If released inside an orbit, transfer the planet to the PlanetWatcher canvas
            if (planetWatcherCanvas != null)
            {
                // Assuming planets are UI elements; adjust if using SpriteRenderer
                transform.SetParent(planetWatcherCanvas.transform, false);
                
                // Optionally adjust the position or scale if needed
                // Adjust to ensure it's visible and correctly positioned within the new canvas
            }
        }
        else
        {
            // Destroy the clone if not released inside any orbit collider
            Destroy(gameObject);
        }
    }
    void TransferToCanvas(GameObject planet, Canvas targetCanvas)
{
    if (targetCanvas.renderMode == RenderMode.WorldSpace)
    {
        // For World Space canvases, directly setting the position might work fine
        planet.transform.SetParent(targetCanvas.transform, false);
    }
    else
    {
        // For Screen Space canvases, you might need to convert the position
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(planet.transform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(targetCanvas.GetComponent<RectTransform>(), screenPoint, targetCanvas.worldCamera, out Vector2 localPoint);
        
        RectTransform planetRect = planet.GetComponent<RectTransform>();
        if (planetRect != null)
        {
            planet.transform.SetParent(targetCanvas.transform, false);
            planetRect.anchoredPosition = localPoint;
        }
    }
}

}
