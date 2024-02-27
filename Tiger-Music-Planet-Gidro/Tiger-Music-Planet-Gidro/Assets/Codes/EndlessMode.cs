using UnityEngine;
using System.Collections;

public class EndlessMode : MonoBehaviour
{
    public GameObject planetPrefab; // Prefab for spawning planets
    public Transform[] startPositions; // Array of start positions
    public GameObject orbitCenter; // GameObject planets will orbit around
    public Collider2D clickZone; // Collider representing the click zone
    public GameObject destroyZone; // GameObject representing the destroy zone
    public Sprite[] planetSkins; // Assign this array in the Unity Editor
    public int bpm = 120; // Beats per minute, controls spawn rate

    private float spawnInterval; // Time between spawns, based on bpm
    private float bpmDoublingInterval = 90f; // 1.5 minutes in seconds

void Start()
{
    if (AudioManager.Instance != null)
    {
        // Use the selected song from AudioManager
        GetComponent<AudioSource>().clip = AudioManager.Instance.selectedSong;
        GetComponent<AudioSource>().Play();

        // Adjust spawn rate based on selected BPM
        SetBPM(AudioManager.Instance.selectedBPM/2);
    }

    Time.timeScale = 1f;
    CalculateSpawnInterval();
    StartCoroutine(SpawnPlanets());
    StartCoroutine(DoubleBPM());
}


    void CalculateSpawnInterval()
    {
        // Convert bpm to spawn interval in seconds
        spawnInterval = 60f / bpm;
    }

    IEnumerator SpawnPlanets()
    {
        Debug.Log("SpawnPlanets coroutine started.");
        while (true)
        {
            SpawnPlanet();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

void SpawnPlanet()
{
    // Randomly select one of the start positions
    Transform startPosition = startPositions[Random.Range(0, startPositions.Length)];
    GameObject planet = Instantiate(planetPrefab, startPosition.position, Quaternion.identity);

    // Initialize the planet (e.g., set its orbit target and skin)
    Planet planetScript = planet.GetComponent<Planet>();
    if (planetScript != null)
    {
        planetScript.Initialize(orbitCenter, clickZone, destroyZone);

        // If planetSkins array is not empty, assign a random skin
        if (planetSkins.Length > 0)
        {
            Sprite randomSkin = planetSkins[Random.Range(0, planetSkins.Length)];
            planetScript.SetSkin(randomSkin);
        }
    }
}
IEnumerator DoubleBPM()
    {
        while (true)
        {
            yield return new WaitForSeconds(bpmDoublingInterval);

            // Double the effective BPM and adjust spawn interval accordingly
            spawnInterval /= 2;
        }
    }
    void SetBPM(int bpm)
    {
        spawnInterval = 60f / bpm;
    }
}
