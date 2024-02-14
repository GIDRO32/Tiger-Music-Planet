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

void Start()
{
    if (AudioManager.Instance != null)
    {
        // Use the selected song from AudioManager
        GetComponent<AudioSource>().clip = AudioManager.Instance.selectedSong;
        GetComponent<AudioSource>().Play();

        // Adjust spawn rate based on selected BPM
        bpm = AudioManager.Instance.selectedBPM;
    }

    CalculateSpawnInterval();
    StartCoroutine(SpawnPlanets());
}


    void CalculateSpawnInterval()
    {
        // Convert bpm to spawn interval in seconds
        spawnInterval = 60f / bpm;
    }

    IEnumerator SpawnPlanets()
    {
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

}
