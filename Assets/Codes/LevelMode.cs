using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelMode : MonoBehaviour
{
    public GameObject PlayButton;
    public GameObject[] planets;
    private Vector3[] startPositions;
    public List<Collider2D> orbitColliders; // List of orbit colliders
    private GameObject draggedPlanet = null;
    private Vector3 offset;
    private Dictionary<GameObject, List<GameObject>> orbitToPlanetsMap = new Dictionary<GameObject, List<GameObject>>();
    // [SerializeField] private List<GameObjectGroup> sequenceOfGroups = new List<GameObjectGroup>();
    [SerializeField] private List<List<AudioClip>> MusicPatterns = new List<List<AudioClip>>(); // List of lists to hold music patterns for each orbit
    [SerializeField] private List<OrbitAudioPair> orbitAudioPairs = new List<OrbitAudioPair>();
    private Dictionary<GameObject, List<AudioClip>> orbitSoundOrder = new Dictionary<GameObject, List<AudioClip>>();
    void Start()
    {
        PlayButton.SetActive(true);
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
    bool isCorrectOrder = true; // Assume true until proven otherwise

    // Temporary list to store the sequence of played sounds for comparison
    List<AudioClip> playedSounds = new List<AudioClip>();

    foreach (var orbitClips in MusicPatterns)
    {
        foreach (var clip in orbitClips)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.Play();
            playedSounds.Add(clip); // Add the clip to the playedSounds list for later comparison
            Destroy(audioSource, clip.length);
            Debug.Log($"Playing AudioClip: {clip.name}");
        }
        yield return new WaitForSeconds(0.5f);
    }
List<AudioClip> correctOrderSounds = orbitSoundOrder.SelectMany(pair => pair.Value).ToList();
if (playedSounds.Count == correctOrderSounds.Count)
{
    for (int i = 0; i < playedSounds.Count; i++)
    {
        if (playedSounds[i] != correctOrderSounds[i])
        {
            isCorrectOrder = false;
            break;
        }
    }
}
else
{
    isCorrectOrder = false;
}

    // Clearing MusicPatterns after playing
    MusicPatterns.Clear();

    // Check if the order matches after all sounds have been played
    if (isCorrectOrder)
    {
        Debug.Log("WIN!");
    }
    else
    {
        Debug.Log("Try again...");
    }
}
    public void PlayAllSoundsSequentially()
    {
        ConstructMusicPattern();
        StartCoroutine(PlaySoundsFromPattern());
    }
public void ShowPattern()
{
    StartCoroutine(PlaySoundsFromOrbitAudioPairs());
}

IEnumerator PlaySoundsFromOrbitAudioPairs()
{
    foreach (OrbitAudioPair pair in orbitAudioPairs)
    {
        foreach (AudioClip clip in pair.audioClips)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.Play();
            Debug.Log($"Playing correct AudioClip: {clip.name} for orbit: {pair.orbit.name}");
            yield return new WaitForSeconds(clip.length + 0.5f); // Wait for the clip to finish plus a delay
            Destroy(audioSource); // Clean up the AudioSource once the clip has finished
        }
    }
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

[System.Serializable]
public class GameObjectGroup
{
    public List<GameObject> groupItems = new List<GameObject>();
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
