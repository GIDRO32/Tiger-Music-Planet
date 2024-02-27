using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelMode : MonoBehaviour
{
    
    public GameObject Interface;
    public GameObject PlayButton;
    public GameObject StartPanel;
    public GameObject CompletePanel;
    public GameObject[] planets;
    private Vector3[] startPositions;
    public GameObject[] buttonIndicators;
    public List<Collider2D> orbitColliders; // List of orbit colliders
    private GameObject draggedPlanet = null;
    private Vector3 offset;
    private Dictionary<GameObject, List<GameObject>> orbitToPlanetsMap = new Dictionary<GameObject, List<GameObject>>();
    // [SerializeField] private List<GameObjectGroup> sequenceOfGroups = new List<GameObjectGroup>();
    [SerializeField] private List<List<AudioClip>> MusicPatterns = new List<List<AudioClip>>(); // List of lists to hold music patterns for each orbit
    [SerializeField] private List<OrbitAudioPair> orbitAudioPairs = new List<OrbitAudioPair>();
    private Dictionary<GameObject, List<AudioClip>> orbitSoundOrder = new Dictionary<GameObject, List<AudioClip>>();
    public float levelTimer = 70.0f; // Total time for the level in seconds
    public Slider timerSlider; // Assign in the inspector
    public GameObject gameOverPanel; // Assign in the inspector
    private bool extraTimeUsed = false; // To check if the extra time button has been used
    public Button extraTimeButton; // Assign in the inspector
    public Text timerCounter;
    public AudioSource sounds;
    public AudioClip StageClear;
    public AudioClip TimeOut;
    public AudioClip AddSomeTime;
    public AudioClip resetOrbits;
    private bool gameOverSoundPlayed = false;
    private int LevelState;
    public string LevelTag;
    public float patternPlayDelay;
    public Button playAllSoundsButton;

    void Start()
    {
        LevelState = PlayerPrefs.GetInt(LevelTag, LevelState);
        Time.timeScale = 1f;
        timerSlider.maxValue = levelTimer; 
        timerSlider.value = levelTimer;
        Interface.SetActive(false);
        CompletePanel.SetActive(false);
        StartPanel.SetActive(true);
        PlayButton.SetActive(true);
        gameOverPanel.SetActive(false);
        startPositions = new Vector3[planets.Length];
        for (int i = 0; i < planets.Length; i++)
        {
            startPositions[i] = planets[i].transform.position;
        }
        for(int j = 0; j < buttonIndicators.Length; j++)
        {
            buttonIndicators[j].SetActive(false);
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
    // Play the sounds first
    foreach (var orbitClips in MusicPatterns)
    {
        foreach (var clip in orbitClips)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.Play();
            // yield return new WaitForSeconds(clip.length); // Wait for the clip to finish playing
            Destroy(audioSource, clip.length);
            Debug.Log($"Playing AudioClip: {clip.name}");
        }
        yield return new WaitForSeconds(0.5f); // Additional wait time between orbits
    }

    bool allOrbitsCorrect = true; // Assume all orbits are correct until proven otherwise

    // Adjusting for orbitSoundOrder being a Dictionary<GameObject, List<AudioClip>>
    var orbitGameObjects = orbitSoundOrder.Keys.ToList();

    if (MusicPatterns.Count != orbitSoundOrder.Count)
    {
        allOrbitsCorrect = false; // Immediate fail if the number of populated orbits doesn't match the required number
    }
    else
    {
        for (int orbitIndex = 0; orbitIndex < orbitGameObjects.Count; orbitIndex++)
        {
            var orbitGameObject = orbitGameObjects[orbitIndex];
            if (orbitIndex < MusicPatterns.Count) // Ensure we don't go out of bounds
            {
                var playerOrbitSounds = new HashSet<AudioClip>(MusicPatterns[orbitIndex]);
                var correctOrbitSounds = new HashSet<AudioClip>(orbitSoundOrder[orbitGameObject]);

                if (!playerOrbitSounds.SetEquals(correctOrbitSounds))
                {
                    allOrbitsCorrect = false;
                    break; // Exit the loop early if any orbit doesn't match
                }
            }
            else
            {
                // If there are fewer player inputs than required orbits, it's an automatic fail
                allOrbitsCorrect = false;
                break;
            }
        }
    }

    // Clearing MusicPatterns after checking
    MusicPatterns.Clear();

    // Check if all orbits match after verification
    if (allOrbitsCorrect)
    {
        LevelState = 1;
        PlayerPrefs.SetInt(LevelTag, LevelState);
        sounds.PlayOneShot(StageClear);
        Debug.Log("WIN!");
        CompletePanel.SetActive(true);
        Time.timeScale = 0f;
    }
    else
    {
        Debug.Log("Try again...");
    }

    yield return null; // This yield return is optional here but maintains structure consistency
}




IEnumerator MoveCameraToPositionY(float targetY, float duration)
{
    float time = 0;
    Vector3 startPosition = Camera.main.transform.position;
    Vector3 endPosition = new Vector3(startPosition.x, targetY, startPosition.z);

    while (time < duration)
    {
        time += Time.deltaTime;
        float t = time / duration;
        Camera.main.transform.position = Vector3.Lerp(startPosition, endPosition, t);
        yield return null;
    }

    Camera.main.transform.position = endPosition; // Ensure the camera is exactly in the right position
}
public void PlayAllSoundsSequentially()
    {
        if (playAllSoundsButton.interactable)
        {
            StartCoroutine(MoveCameraAndPlaySounds());
            StartCoroutine(DisablePlayAllSoundsButtonTemporarily());
        }
    }

    IEnumerator DisablePlayAllSoundsButtonTemporarily()
    {
        playAllSoundsButton.interactable = false; // Disable the button
        yield return new WaitForSeconds(10); // Wait for 10 seconds
        playAllSoundsButton.interactable = true; // Re-enable the button
    }

IEnumerator MoveCameraAndPlaySounds()
{
    // Move the camera to Y position 0 over 1 second (adjust duration as needed)
    yield return StartCoroutine(MoveCameraToPositionY(0f, 0.1f));

    // After the camera has reached its position, construct the music pattern and play sounds
    ConstructMusicPattern();
    StartCoroutine(PlaySoundsFromPattern());
}
public void ShowPattern()
{
    StartCoroutine(PlaySoundsFromOrbitAudioPairs());
    PlayButton.SetActive(false);
}
public void StartLevel()
{
    sounds.PlayOneShot(AddSomeTime);
    StartPanel.SetActive(false);
    Interface.SetActive(true);
}
IEnumerator PlaySoundsFromOrbitAudioPairs()
{
    List<AudioSource> activeAudioSources = new List<AudioSource>();
    int indicatorIndex = 0;

    foreach (OrbitAudioPair pair in orbitAudioPairs)
    {
        if (indicatorIndex < buttonIndicators.Length)
        {
            buttonIndicators[indicatorIndex].SetActive(true); // Activate indicator for the current pair
        }

        foreach (AudioClip clip in pair.audioClips)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.Play();
            activeAudioSources.Add(audioSource); // Keep track of this AudioSource to destroy it later
        }

        yield return new WaitForSeconds(patternPlayDelay); // Wait for the pattern delay before proceeding to the next pair

        if (indicatorIndex < buttonIndicators.Length)
        {
            buttonIndicators[indicatorIndex].SetActive(false); // Deactivate the current indicator
            indicatorIndex++; // Move to the next indicator
        }
    }

    // Ensure all audio sources are destroyed after their clips have finished playing
    foreach (var source in activeAudioSources)
    {
        if (source != null) // Check if the source hasn't already been destroyed
        {
            Destroy(source, source.clip.length);
        }
    }

    // Ensure that all indicators are turned off at the end
    foreach (var indicator in buttonIndicators)
    {
        indicator.SetActive(false);
    }
}



public void DestroyAllPlanetsOnOrbits()
{
    sounds.PlayOneShot(resetOrbits);
    foreach (var planet in FindObjectsOfType<PlanetController>())
    {
        // Check if the planet is within any orbit collider
        if (orbitColliders.Any(orbitCollider => orbitCollider.OverlapPoint(planet.transform.position)))
        {
            Destroy(planet.gameObject);
        }
    }
}


    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        timerCounter.text = levelTimer.ToString("F1");

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
        if (levelTimer > 0)
        {
            levelTimer -= Time.deltaTime;
            timerSlider.value = levelTimer;
        }
        else
        {
           if (!gameOverSoundPlayed)
            {
            Time.timeScale = 0f;
            gameOverPanel.SetActive(true);
            extraTimeButton.interactable = false;
            sounds.volume = 0.5f; // Adjust the volume as needed
            sounds.PlayOneShot(TimeOut);
            gameOverSoundPlayed = true; // Prevent further plays
            }
        }
    }
    public void AddExtraTime()
    {
        if (!extraTimeUsed)
        {
            sounds.PlayOneShot(AddSomeTime);
            levelTimer += 30.0f; // Add 30 seconds to the timer
            extraTimeUsed = true; // Prevent further use of the button in this level
            extraTimeButton.interactable = false; // Disable the button
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
