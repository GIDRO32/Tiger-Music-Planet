using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class Profile : MonoBehaviour
{
    #region SingleTon:Profile
    public static Profile Instance;
    void Awake()
    {
        if(Instance == null)
        Instance = this;
        else
        Destroy(gameObject);
    }
    #endregion
[System.Serializable] public class Skin
{
    public Sprite Image;
    public List<Song> Songs = new List<Song>(); // Updated to use Song class
}


    public List<Skin> SkinList;
    [SerializeField] GameObject SkinUITemplate;
    [SerializeField] Transform SkinScrolling;
    GameObject g;
    public int newSelectedIndex ,previousSelectedIndex;

    [SerializeField] Color ActiveSkinColor;
    [SerializeField] Color DefaultSkinColor;
    [SerializeField] Image CurrentSkin;

    void Start()
    {
        GetAvailableSkins();
        LoadPurchasedSkins();
        newSelectedIndex = previousSelectedIndex = 0;
    }
void GetAvailableSkins()
{
    for(int i = 0; i < Shopping.Instance.ShopItemList.Count; i++)
    {
        if(Shopping.Instance.ShopItemList[i].isAvailable)
        {
            // Ensure songs are also passed here
            AddSkin(Shopping.Instance.ShopItemList[i].Image, Shopping.Instance.ShopItemList[i].Songs);
        }
    }
    SelectSkin(newSelectedIndex); // This will now print the correct list of songs for the initially selected skin
}
public List<Song> GetCurrentSkinSongs()
    {
        if (SkinList.Count > newSelectedIndex)
        {
            return SkinList[newSelectedIndex].Songs;
        }
        return new List<Song>(); // Return an empty list if no current skin
    }

    public void AddSkin(Sprite picture, List<Song> songs)
    {
        if (SkinList == null)
        SkinList = new List<Skin>();

        Skin av = new Skin() { Image = picture, Songs = songs }; // Assign both image and songs
        SkinList.Add(av);
        g = Instantiate(SkinUITemplate, SkinScrolling);
        g.transform.GetChild(0).GetComponent<Image>().sprite = av.Image;
        g.transform.GetComponent<Button>().AddEventListener(SkinList.Count - 1, OnSkinClick);
        PlayerPrefs.SetInt("ItemAvailable_" + Shopping.Instance.ShopItemList.Count, 1);
        PlayerPrefs.Save();
    }
void LoadPurchasedSkins()
{
    SkinList.Clear(); // Clear existing skins
    ClearSkinUI(); // Clear existing skin UI elements

    // Reinitialize skins with songs
    for (int i = 0; i < Shopping.Instance.ShopItemList.Count; i++)
    {
        if (PlayerPrefs.GetInt("ItemAvailable_" + i, 0) == 1 || i == 0) // Include default skin
        {
            AddSkin(Shopping.Instance.ShopItemList[i].Image, Shopping.Instance.ShopItemList[i].Songs);
        }
    }
    SelectSkin(newSelectedIndex); // Ensure this reflects the updated list
}
    public void UnlockSkin(int skinIndex)
    {
        PlayerPrefs.SetInt("ItemAvailable_" + skinIndex, 1);
        PlayerPrefs.Save();
    }
    void OnSkinClick(int SkinIndex)
    {
        SelectSkin(SkinIndex);
    }
void SelectSkin(int SkinIndex)
{
    if (SkinIndex < 0 || SkinIndex >= SkinList.Count) return; // Guard clause

    previousSelectedIndex = newSelectedIndex;
    newSelectedIndex = SkinIndex;

    if (SkinScrolling.childCount > newSelectedIndex)
    {
        SkinScrolling.GetChild(newSelectedIndex).GetComponent<Image>().color = ActiveSkinColor;
    }
    if (SkinScrolling.childCount > previousSelectedIndex)
    {
        SkinScrolling.GetChild(previousSelectedIndex).GetComponent<Image>().color = DefaultSkinColor;
    }
    
    CurrentSkin.sprite = SkinList[newSelectedIndex].Image;

    // Now, print the list of songs for the selected skin
    Debug.Log($"Selected Skin: {SkinList[newSelectedIndex].Songs.Count} songs");
    foreach (Song song in SkinList[newSelectedIndex].Songs)
    {
        Debug.Log($"Song: {song.SongName}, Composer: {song.Composer}");
    }
}

    void ClearSkinUI()
    {
        foreach (Transform child in SkinScrolling)
        {
            Destroy(child.gameObject);
        }
    }
    public void ResetData()
    {
        for (int i = 1; i < Shopping.Instance.ShopItemList.Count; i++)
        {
            PlayerPrefs.SetInt("ItemAvailable_" + i, 0);
            if (SkinScrolling.childCount > i) // Check if the child exists
            {
                Destroy(SkinScrolling.GetChild(i).gameObject);
            }
        }
        PlayerPrefs.Save();
        SelectSkin(0);
        LoadPurchasedSkins(); // Reload the skins to update the UI
    }
}
