using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelRoster : MonoBehaviour
{
    public GameObject[] Stars;
    public string[] tags;
    private int[] State;

    void Start()
    {
        // Initialize the State array with the same length as the tags array
        State = new int[tags.Length];

        // Initially deactivate all stars and update their states
        for (int i = 0; i < tags.Length; i++)
        {
            Stars[i].SetActive(false);
            State[i] = PlayerPrefs.GetInt(tags[i], 0); // Default state is 0 if not set
        }

        UpdateStars();
    }

    void UpdateStars()
    {
        // Update stars based on the state values
        for (int i = 0; i < tags.Length; i++)
        {
            Stars[i].SetActive(State[i] == 1);
        }
    }

    public void ResetData()
    {
        for (int i = 0; i < tags.Length; i++)
        {
            State[i] = 0;
            PlayerPrefs.SetInt(tags[i], State[i]);
        }

        // After resetting the data, update the stars
        UpdateStars();

        // Save the reset state to PlayerPrefs
        PlayerPrefs.Save();
    }
}
