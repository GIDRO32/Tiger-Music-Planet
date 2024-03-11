using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppsFlyerSDK;

// This class is intended to be used the the AppsFlyerObject.prefab

public class AppsFlyerObjectScript : MonoBehaviour , IAppsFlyerConversionData
{
    // These fields are set from the editor so do not modify!
    //******************************//
    public string devKey;
    public string appID;
    public string UWPAppID;
    public string macOSAppID;
    public bool isDebug;
    public bool getConversionData;

    //******************************//


    void Start()
    {
        AppsFlyer.setIsDebug(isDebug);
        AppsFlyer.initSDK(devKey, appID, getConversionData ? this : null);

        AppsFlyer.startSDK();
    }


    public void onConversionDataSuccess(string ddaattggaPPofl)
    {
        AppsFlyer.AFLog("didReceiveConversionData", ddaattggaPPofl);
        
        Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(ddaattggaPPofl);
        conversionDataDictionary.Add("af_id", AppsFlyer.getAppsFlyerId());
        conversionDataDictionary.Add("locale", Application.systemLanguage.ToString());
        conversionDataDictionary.Add("store_id", "6479052852");
        conversionDataDictionary.Add("os", "iOS");
        conversionDataDictionary.Add("bundle_id", "plemko.music.planet");
        
        PlayerPrefs.SetString("conversionDataDictionary", JsonUtility.ToJson(conversionDataDictionary));
    }

    public void onConversionDataFail(string error)
    {
        AppsFlyer.AFLog("didReceiveConversionDataWithError", error);
        PlayerPrefs.SetString("conversionDataDictionary", "");
    }

    public void onAppOpenAttribution(string attributionData)
    {
        AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
        PlayerPrefs.SetString("conversionDataDictionary", "");
    }

    public void onAppOpenAttributionFailure(string error)
    {
        AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
        PlayerPrefs.SetString("conversionDataDictionary", "");
    }

}
