using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioClip selectedSong;
    public int selectedBPM = 120; // Default value
    public float rotationSpeedMultiplier = 20f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make this object persist across scene changes
        }
    }
}
