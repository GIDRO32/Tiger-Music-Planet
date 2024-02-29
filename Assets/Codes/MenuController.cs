using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject MenuButtons;
    public GameObject[] Panels;
    public AudioSource musicSource;
    public AudioSource SFX;
    public AudioClip[] Effects;
    // Start is called before the first frame update
    
    void Start()
    {
        MenuButtons.SetActive(true);
        for(int i = 0; i < Panels.Length; i++)
        {
            Panels[i].SetActive(false);
        }
        var endlessModePanel = GameObject.Find("SetTempo");
    if (endlessModePanel != null) {
        Destroy(endlessModePanel);
    }
    }
public void OpenPanel(int index)
{
    SFX.PlayOneShot(Effects[1]);
    MenuButtons.SetActive(false);
    Panels[index].SetActive(true);
}
public void SoundTest()
{
    SFX.PlayOneShot(Effects[0]);
}

public void ClosePanel(int index)
{
    SFX.PlayOneShot(Effects[1]);
    MenuButtons.SetActive(true);
    Panels[index].SetActive(false);
}
}