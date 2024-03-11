using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class PlanetController : MonoBehaviour
{
    public AudioClip[] orbitSounds; // Array of audio clips for each orbit
    private AudioSource audioSource;
    public AudioMixer Sound_Volume;
    public bool isBeingDragged = false; // Flag to check if the planet is being dragged

    void Start()
    {
        // Get or add an AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

void Update()
{
    if (Input.GetMouseButtonDown(0))
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            CloneAndDrag();
        }
    }
}

public void CloneAndDrag()
    {
        // Clone this GameObject.
        GameObject planetClone = Instantiate(gameObject, transform.position, Quaternion.identity, transform.parent);

        // If the planet has a specific component that should only be on the clone (e.g., PlanetDragger),
        // add it here. If it's already attached and should be removed from the original, adjust accordingly.
        // planetClone.AddComponent<PlanetDragger>();
        
        // Optionally, make adjustments to the clone (e.g., deactivate or modify components that should only be active on the original).
        // Destroy(planetClone.GetComponent<PlanetController>());

        // If you use any component to indicate that this is a clone for dragging purposes,
        // you can initialize or set its state here.
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isBeingDragged && collision.CompareTag("Orbit")) // Check if not being dragged
        {
            Debug.Log("On the orbit");
            int orbitIndex = GetOrbitIndex(collision.gameObject);
            if (orbitIndex != -1 && orbitIndex < orbitSounds.Length)
            {
                // Play the corresponding sound
                audioSource.clip = orbitSounds[orbitIndex];
                // audioSource.Play();
            }
        }
    }

    private int GetOrbitIndex(GameObject orbitObject)
    {
        int index;
        if (int.TryParse(orbitObject.name.Replace("Orbit", ""), out index))
        {
            return index - 1; // Assuming orbit names are "Orbit1", "Orbit2", etc.
        }
        return -1;
    }

    // This would be called by the drag logic to set the dragging state.
    public void SetDragging(bool isDragging)
    {
        isBeingDragged = isDragging;
    }
}