using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shopping : MonoBehaviour
{
    #region SingleTon:Shopping
    public static Shopping Instance;
    public AudioSource Sounds;
    public AudioClip Buy;
    public AudioClip CantBuy;
    void Awake()
    {
        if(Instance == null)
        Instance = this;
        else
        Destroy(gameObject);
    }
    #endregion
    [System.Serializable]
    public class ShopItem
{
    public Sprite Image;
    public string Title;
    public int price;
    public bool isAvailable = false;
    public List<Song> Songs = new List<Song>(); // List of songs associated with the item
}
    public List<ShopItem> ShopItemList;
    [SerializeField]GameObject ItemTemplate;
    GameObject g;
    [SerializeField] Transform ShopScrolling;
    [SerializeField] GameObject ShopPanel;
    [SerializeField] Text Balance;
    Button buyBtn;
    public GameObject NotEnough_MSG;
    void Start()
    { 
        LoadItemAvailability();
        NotEnough_MSG.SetActive(false);
        int len = ShopItemList.Count;
        for(int i = 0; i < len; i++)
        {
            g = Instantiate(ItemTemplate, ShopScrolling);
            g.transform.GetChild(0).GetComponent<Image>().sprite = ShopItemList[i].Image;
            g.transform.GetChild(1).GetComponent<Text>().text = ShopItemList[i].Title;
            g.transform.GetChild(2).GetComponent<Text>().text = ShopItemList[i].price.ToString();
            buyBtn = g.transform.GetChild(3).GetComponent<Button>();
            if(ShopItemList[i].isAvailable)
            {
                DisableBuyButton();
            }
            buyBtn.AddEventListener(i, OnBuyBtnClicked);
        }
    }
    IEnumerator MessageHang()
    {
            for(float f = 1f; f >= -0.05f; f -= 0.05f)
            {
                yield return new WaitForSeconds(0.1f);
            }
            NotEnough_MSG.SetActive(false);
    }
    void LoadItemAvailability()
    {
        for (int i = 0; i < ShopItemList.Count; i++)
        {
            ShopItemList[i].isAvailable = PlayerPrefs.GetInt("ItemAvailable_" + i, 0) == 1;
        }
    }
    void OnBuyBtnClicked(int itemIndex)
    {
        if(Product.Instance.HasEnoughCoins(ShopItemList[itemIndex].price))
        {
        Sounds.PlayOneShot(Buy);
        Product.Instance.UseCoins(ShopItemList[itemIndex].price);
        ShopItemList[itemIndex].isAvailable = true;
        PlayerPrefs.SetInt("ItemAvailable_" + itemIndex, 1); // Save purchase
        PlayerPrefs.Save();
        buyBtn = ShopScrolling.GetChild(itemIndex).GetChild(3).GetComponent<Button>();
        DisableBuyButton();
        Profile.Instance.AddSkin(ShopItemList[itemIndex].Image, ShopItemList[itemIndex].Songs);
        }
        else
        {
            Sounds.PlayOneShot(CantBuy);
            Debug.Log("Not Enough!");
            NotEnough_MSG.SetActive(true);
            StartCoroutine("MessageHang");
        }
    }
public void ResetPurchases()
{
    for (int i = 1; i < ShopItemList.Count; i++) // Start from 1 to skip the default item
    {
        ShopItemList[i].isAvailable = false;
        PlayerPrefs.SetInt("ItemAvailable_" + i, 0);

        if (ShopScrolling != null && ShopScrolling.childCount > i)
        {
            Transform itemTransform = ShopScrolling.GetChild(i);
            Button buyButton = itemTransform.GetChild(3).GetComponent<Button>();
            if (buyButton != null)
            {
                buyButton.interactable = true;
                buyButton.transform.GetChild(0).GetComponent<Text>().text = "Buy";
            }
        }
    }
    PlayerPrefs.SetInt("Total", 0);
    PlayerPrefs.Save();

    // No need to call LoadItemAvailability() here since we're already updating the UI
}

    void DisableBuyButton()
    {
        buyBtn.interactable = false;
        buyBtn.transform.GetChild(0).GetComponent<Text>().text = "Owned";
    }
    void Update()
    {
        Balance.text = Product.Instance.Coins.ToString();
        LoadItemAvailability();
    }
    public void OpenShop()
    {
        ShopPanel.SetActive(true);
    }
    public void CloseShop()
    {
        ShopPanel.SetActive(false);
    }
}
[System.Serializable]
public class Song
{
    public AudioClip Clip;
    public string SongName;
    public string Composer;
}
