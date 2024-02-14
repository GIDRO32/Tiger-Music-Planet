using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    public GameObject Interface;
    public GameObject[] Panels;
    public AudioSource musicSource;
    public AudioSource SFX;
    public AudioClip[] Effects;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0f;
        Interface.SetActive(false);
        Panels[0].SetActive(true);
        for(int i = 1; i < Panels.Length; i++)
        {
            Panels[i].SetActive(false);
        }
        musicSource.Pause();
    }
public void OpenPanel(int index)
{
    Time.timeScale = 0f;
    Interface.SetActive(false);
    Panels[index].SetActive(true);
    if (musicSource.isPlaying)
    {
        musicSource.Pause(); // Pause the music when a panel is opened
    }
}
public void SoundTest()
{
    SFX.PlayOneShot(Effects[0]);
}

public void ClosePanel(int index)
{
    Time.timeScale = 1f;
    Interface.SetActive(true);
    Panels[index].SetActive(false);
    musicSource.UnPause(); // Resume the music when a panel is closed
}
}