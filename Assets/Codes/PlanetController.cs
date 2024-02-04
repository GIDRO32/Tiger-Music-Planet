using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetController : MonoBehaviour
{
    public AudioClip[] orbitSounds; // Array of audio clips for each orbit
    private AudioSource audioSource;
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isBeingDragged && collision.CompareTag("Orbit")) // Check if not being dragged
        {
            Debug.Log("OOOOHHHH!!!");
            int orbitIndex = GetOrbitIndex(collision.gameObject);
            if (orbitIndex != -1 && orbitIndex < orbitSounds.Length)
            {
                // Play the corresponding sound
                audioSource.clip = orbitSounds[orbitIndex];
                audioSource.Play();
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
