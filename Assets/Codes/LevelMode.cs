using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelMode : MonoBehaviour
{
    public GameObject[] planets;
    private Vector3[] startPositions;
    public List<Collider2D> orbitColliders; // List of orbit colliders
    private GameObject draggedPlanet = null;
    private Vector3 offset;
    private Dictionary<GameObject, List<GameObject>> orbitToPlanetsMap = new Dictionary<GameObject, List<GameObject>>();
    [SerializeField] List<List<AudioClip>> MusicPatterns = new List<List<AudioClip>>(); // List of lists to hold music patterns for each orbit
    [SerializeField] private List<OrbitAudioPair> orbitAudioPairs = new List<OrbitAudioPair>();
    private Dictionary<GameObject, List<AudioClip>> orbitSoundOrder = new Dictionary<GameObject, List<AudioClip>>();
    private List<AudioClip> playedSounds = new List<AudioClip>();
    void Start()
    {
startPositions = new Vector3[planets.Length];
        for (int i = 0; i < planets.Length; i++)
        {
            startPositions[i] = planets[i].transform.position;
        }
        
        foreach (var orbitCollider in orbitColliders)
        {
            orbitToPlanetsMap[orbitCollider.gameObject] = new List<GameObject>();
        }
        foreach (OrbitAudioPair pair in orbitAudioPairs)
        {
            orbitSoundOrder[pair.orbit] = pair.audioClips;
        }
        MusicPatterns.Clear(); 
    }
void ConstructMusicPattern()
    {
        foreach (var orbitCollider in orbitColliders)
        {
            List<AudioClip> orbitClips = new List<AudioClip>();

            var planetsInOrbit = FindObjectsOfType<PlanetController>().Where(
                planet => orbitCollider.OverlapPoint(planet.transform.position)).ToList();

            foreach (var planet in planetsInOrbit)
            {
                int orbitIndex = orbitColliders.IndexOf(orbitCollider);
                if (orbitIndex >= 0 && orbitIndex < planet.orbitSounds.Length)
                {
                    AudioClip clip = planet.orbitSounds[orbitIndex];
                    orbitClips.Add(clip);
                }
            }

            MusicPatterns.Add(orbitClips);
        }
    }
    IEnumerator PlaySoundsFromPattern()
    {
        foreach (var orbitClips in MusicPatterns)
        {
            foreach (var clip in orbitClips)
            {
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = clip;
                audioSource.Play();
                Destroy(audioSource, clip.length); // Destroy the AudioSource once the clip has finished playing
                playedSounds.Add(clip);
                Debug.Log($"Playing AudioClip: {clip.name}");
            }
            yield return new WaitForSeconds(0.5f); // Wait for 0.5 seconds between each orbit's clips
        }
        MusicPatterns.Clear(); 
    }
    public void PlayAllSoundsSequentially()
    {
        ConstructMusicPattern();
        StartCoroutine(PlaySoundsFromPattern());
    }
public void CheckWinCondition()
{
    // Initialize a list to hold the correct sequence of sounds for all orbits
    List<AudioClip> correctSequence = new List<AudioClip>();

    // Populate the correctSequence list based on the defined order in orbitSoundOrder
    foreach (var orbitCollider in orbitColliders)
    {
        if (orbitSoundOrder.TryGetValue(orbitCollider.gameObject, out List<AudioClip> orbitSounds))
        {
            correctSequence.AddRange(orbitSounds);
        }
    }

    // Now compare the playedSounds with the correctSequence
    if (playedSounds.SequenceEqual(correctSequence))
    {
        Debug.Log("WIN!");
    }
    else
    {
        Debug.Log("Try again...");
    }

    // Clear playedSounds for the next attempt
    playedSounds.Clear();
}

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (Input.GetMouseButtonDown(0))
        {
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Planet"))
            {
                draggedPlanet = hit.collider.gameObject;
                offset = draggedPlanet.transform.position - mousePos;

                RotateAroundTarget rotateScript = draggedPlanet.GetComponent<RotateAroundTarget>();
                if (rotateScript != null)
                {
                    rotateScript.enabled = false;
                }
            }
        }

        if (draggedPlanet != null)
        {
            draggedPlanet.transform.position = new Vector3(mousePos.x + offset.x, mousePos.y + offset.y, draggedPlanet.transform.position.z);

            if (Input.GetMouseButtonUp(0))
            {
                bool isInsideOrbit = false;
                GameObject orbitTarget = null;

                foreach (Collider2D orbitCollider in orbitColliders)
                {
                    if (orbitCollider.OverlapPoint(draggedPlanet.transform.position))
                    {
                        isInsideOrbit = true;
                        orbitTarget = orbitCollider.gameObject;
                        break;
                    }
                }

                if (isInsideOrbit && orbitTarget != null)
                {
                    RotateAroundTarget rotateScript = draggedPlanet.GetComponent<RotateAroundTarget>();
                    if (rotateScript == null)
                    {
                        rotateScript = draggedPlanet.AddComponent<RotateAroundTarget>();
                    }
                    rotateScript.Init(orbitTarget, 20f); // Assuming 20 is the speed
                    rotateScript.enabled = true;
                }
                else
                {
                    int index = System.Array.IndexOf(planets, draggedPlanet);
                    draggedPlanet.transform.position = startPositions[index];
                }

                draggedPlanet = null;
            }
        }
    }
}

[System.Serializable]
public class OrbitAudioPair
{
    public GameObject orbit;
    public List<AudioClip> audioClips;
}

public class RotateAroundTarget : MonoBehaviour
{
    private GameObject target;
    private float speed;
    public Vector3 direction = Vector3.forward; // Change to Z-axis rotation

    public void Init(GameObject orbitTarget, float orbitSpeed)
    {
        target = orbitTarget;
        speed = orbitSpeed;
    }

    void Update()
    {
        transform.RotateAround(target.transform.position, direction, speed * Time.deltaTime);
    }
}
