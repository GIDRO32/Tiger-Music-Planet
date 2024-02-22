using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SongSelector : MonoBehaviour
{
    public List<Song> Songs = new List<Song>(); // List of songs
    public AudioSource audioSource; // AudioSource to play the sound when reaching bounds
    public AudioClip boundarySound; // Sound to play when reaching list bounds
    public Text songDisplayText; // Text to display the current song name
    private int currentIndex = 0; // Current index in the song list
    private int currentBPM = 100;
    public float speedMultiplier = 20f; // Current speed multiplier for planet rotation
    public float speedChangeAmount = 10f; // Amount to change speed by with each button press
    public Text speedDisplayText;
    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
void Start()
{
    // Use the new method to get the current skin's songs
    if (Profile.Instance != null)
    {
        Songs = Profile.Instance.GetCurrentSkinSongs();
    }
    else
    {
        Debug.LogWarning("Profile instance is null.");
    }

    UpdateSongDisplay();
    UpdateSpeedDisplay();
}
public void IncreaseSpeed()
{
    speedMultiplier += speedChangeAmount; // Update local multiplier
    AudioManager.Instance.rotationSpeedMultiplier = speedMultiplier; // Sync with AudioManager
    UpdateSpeedDisplay();
}
public void DecreaseSpeed()
{
    speedMultiplier = Mathf.Max(20f, speedMultiplier - speedChangeAmount); // Update local multiplier
    AudioManager.Instance.rotationSpeedMultiplier = speedMultiplier; // Sync with AudioManager
    UpdateSpeedDisplay();
}
void UpdateSpeedDisplay()
{
    if (speedDisplayText != null)
    {
        speedDisplayText.text = $"Speed: {speedMultiplier}"; // Format the speed to one decimal place
    }
}
private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Endless") // Check if the loaded scene is EndlessMode
        {
            Destroy(gameObject); // Destroy SongSelector if moving to EndlessMode
        }
        // Alternatively, you can destroy it in any scene other than its origin scene.
    }

    public void NextSong()
    {
        if (currentIndex < Songs.Count - 1)
        {
            currentBPM += 10;
            currentIndex++;
            UpdateSongDisplay();
        }
        else
        {
            // Play a sound to indicate no more next songs
            audioSource.PlayOneShot(boundarySound);
        }
    }

    public void PreviousSong()
    {
        if (currentIndex > 0)
        {
            currentBPM -= 10;
            currentIndex--;
            UpdateSongDisplay();
        }
        else
        {
            // Play a sound to indicate no more previous songs
            audioSource.PlayOneShot(boundarySound);
        }
    }
    public void ClearSongs()
{
    Songs.Clear();
    currentIndex = 0; // Reset the index to the beginning
    songDisplayText.text = ""; // Clear the song display text
}
public void PopulateSongs()
{
    if (Profile.Instance != null)
    {
        Songs = Profile.Instance.GetCurrentSkinSongs();
        UpdateSongDisplay(); // Ensure the display is updated with the first song
    }
}


public void UpdateSongDisplay()
{
    if (Songs.Count > 0 && currentIndex >= 0 && currentIndex < Songs.Count)
    {
        Song currentSong = Songs[currentIndex];
        songDisplayText.text = $"Song: {currentSong.SongName}\nComposer: {currentSong.Composer}\nBPM: {currentBPM}";

        // Update AudioManager with the selected song and BPM
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.selectedSong = currentSong.Clip;
            AudioManager.Instance.selectedBPM = currentBPM;
        }
    }
}

}
