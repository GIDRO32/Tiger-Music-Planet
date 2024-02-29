using UnityEngine;
using UnityEngine.UI; // Required for working with UI elements

public class PlayerHealth : MonoBehaviour
{
    public GameObject Interface;
    public Slider healthSlider; // Assign in the inspector
    private int health = 50; // Starting health
    private int comboCounter = 0; // Combo counter
    private int maxHealth = 100;
    public Text healthCounter;
    public GameObject comboStreak;
    public GameObject GameOver;
    private int judgement_index = 0;
    public GameObject[] judgements; 
    public Text comboValue;
    public Text coinCounter;
    private int balance;
    private int coins = 0;
    private bool IsLost = false;
    private bool balanceUpdated = false;
    public AudioSource SFX;
    public AudioClip[] Effects;

    void Start()
    {
        balance = PlayerPrefs.GetInt("Total", balance);
        comboStreak.SetActive(false);
        GameOver.SetActive(false);
    }

    public void IncreaseHealth()
    {
        if (health < maxHealth)
        {
            // Add only 1 health point if the player has 99 health
            health += (health == 99) ? 1 : 2;
        }
        else
        {
            comboStreak.SetActive(true);
            comboCounter+=10;
        }
        if(health == 100 && comboCounter < 500)
        {
            coins+=100;
            Debug.Log(coins);
        }
        else if(health == 100 && comboCounter >= 500)
        {
            coins += 500;
            Debug.Log(coins);
        }
        SFX.PlayOneShot(Effects[0]);
    }

    public void DecreaseHealth()
    {
        if (health > 0)
        {
            health -= 5;
            health = Mathf.Max(health, 0); // Ensure health doesn't drop below 0
        }
        SFX.PlayOneShot(Effects[1]);
        comboStreak.SetActive(false);
        comboCounter = 0;
    }
    void UpdateJudgementsVisibility()
{
    for (int i = 0; i < judgements.Length; i++)
    {
        if (i == judgement_index)
        {
            judgements[i].SetActive(true); // Show the correct judgement
        }
        else
        {
            judgements[i].SetActive(false); // Hide all other judgements
        }
    }

    // If comboCounter is 0, ensure all judgements are hidden, and comboStreak is inactive
    if (comboCounter == 0)
    {
        foreach (var judgement in judgements)
        {
            judgement.SetActive(false);
        }
        comboStreak.SetActive(false);
    }
    else
    {
        comboStreak.SetActive(true); // Show combo streak UI when comboCounter is not 0
    }
}
    void Update()
    {
        coinCounter.text = coins.ToString();
        if(!IsLost)
        {
        comboValue.text = "Combo:\n" + comboCounter.ToString();
        healthCounter.text = health.ToString();
        healthSlider.value = health;
        if (comboCounter >= 0 && comboCounter < 100) judgement_index = 0;
        else if (comboCounter >= 100 && comboCounter < 200) judgement_index = 1;
        else if (comboCounter >= 200 && comboCounter < 300) judgement_index = 2;
        else if (comboCounter >= 300 && comboCounter < 400) judgement_index = 3;
        else if (comboCounter >= 400 && comboCounter < 500) judgement_index = 4;
        else judgement_index = 5; // Assuming you have at least 6 judgment levels
        }

    if(health == 0 && !balanceUpdated)
    {
        balance += coins;
        PlayerPrefs.SetInt("Total", balance);
        PlayerPrefs.Save(); // Ensure the changes are saved immediately
        balanceUpdated = true; // Prevent further updates
        Time.timeScale = 0f;
        GameOverState();
    }

    // Update the visibility of judgements
    UpdateJudgementsVisibility();
    }
    void GameOverState()
    {
        IsLost = true;
        Interface.SetActive(false);
        GameOver.SetActive(true);
        SFX.PlayOneShot(Effects[2]);
    }
}
