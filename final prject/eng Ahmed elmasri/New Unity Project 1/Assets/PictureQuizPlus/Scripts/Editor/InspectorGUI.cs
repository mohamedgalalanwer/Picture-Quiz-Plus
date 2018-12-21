using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AdsIapSettings))]
public class InspectorGUI : Editor //To override AdsIapSettings class instance view in the Inspector
{
    AdsIapSettings t;
    new SerializedObject serializedObject;

    public bool GetBool(string name)
    {
        return serializedObject.FindProperty(name).boolValue;
    }

    public void SetProperty(string name, bool value)
    {
        serializedObject.FindProperty(name).boolValue = value;
    }

    public SerializedProperty GetProperty(string name)
    {
        return serializedObject.FindProperty(name);
    }
    public bool IsIAPenabled
    {
        get { return GetBool("unityIap"); }
        set
        {
            bool b = GetBool("unityIap");
            if (b == value)
            {
                return;
            }
            SetProperty("unityIap", value);

            SetScriptingSymbol("UNITY_IAP", BuildTargetGroup.Android, value);
            SetScriptingSymbol("UNITY_IAP", BuildTargetGroup.iOS, value);
        }
    }

    public bool EnableAdMob
    {
        get { return GetBool("adMob"); }
        set
        {
            bool b = GetBool("adMob");
            if (b == value)
            {
                return;
            }
            SetProperty("adMob",value);

            SetScriptingSymbol("ENABLE_ADMOB", BuildTargetGroup.Android, value);
            SetScriptingSymbol("ENABLE_ADMOB", BuildTargetGroup.iOS, value);
        }
    }

    public bool EnableUnityAds
    {
        get { return GetBool("unityAds"); }
        set
        {
            bool b = GetBool("unityAds");
            if (b == value)
            {
                return;
            }
            SetProperty("unityAds", value);

        }
    }
    public bool EnableGDPRconsent
    {
        get { return GetBool("GDPRconsent"); }
        set
        {
            bool b = GetBool("GDPRconsent");
            if (b == value)
            {
                return;
            }
            SetProperty("GDPRconsent", value);

            SetScriptingSymbol("GDPR", BuildTargetGroup.Android, value);
            SetScriptingSymbol("GDPR", BuildTargetGroup.iOS, value);
        }
    }

    public override void OnInspectorGUI()
    {
        t = (AdsIapSettings)target;
        serializedObject = new SerializedObject(t);

#if !UNITY_IOS && !UNITY_ANDROID
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			GUILayout.TextField("PLEASE SWITCH PLATFORM TO iOS OR ANDROID IN THE BUILD SETTINGS");
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			return;
#endif
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("HOW TO\nENABLE\nUNITY ADS", GUILayout.Width(150), GUILayout.Height(50)))
        {
            Application.OpenURL("https://docs.unity3d.com/Manual/UnityAdsUnityIntegration.html");
        }
        if (GUILayout.Button("HOW TO\nENABLE\nIAP", GUILayout.Width(150), GUILayout.Height(50)))
        {
            Application.OpenURL("https://docs.unity3d.com/Manual/UnityIAPSettingUp.html");
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("GET\nADMOB\nSDK", GUILayout.Width(150), GUILayout.Height(50)))
        {
            Application.OpenURL("https://developers.google.com/admob/unity/start");
        }
        if (GUILayout.Button("GET UNITY\n DATA PRIVACY\nPLUGIN", GUILayout.Width(150), GUILayout.Height(50)))
        {
            Application.OpenURL("https://assetstore.unity.com/packages/add-ons/services/unity-data-privacy-plug-in-118922");
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();


        IsIAPenabled = EditorGUILayout.BeginToggleGroup(new GUIContent("Enable UnityIAP"), IsIAPenabled);
        EditorGUILayout.EndToggleGroup();

        EnableAdMob = EditorGUILayout.BeginToggleGroup(new GUIContent("Enable AdMob"), EnableAdMob);
        EditorGUILayout.EndToggleGroup();

        EnableUnityAds = EditorGUILayout.BeginToggleGroup(new GUIContent("Enable UnityAds"), EnableUnityAds);
        EditorGUILayout.EndToggleGroup();

        EnableGDPRconsent = EditorGUILayout.BeginToggleGroup(new GUIContent("Enable GDPR consent"), EnableGDPRconsent);
        EditorGUILayout.EndToggleGroup();

#if UNITY_IAP
        GetProperty("isUnityIapSettingOpened").boolValue = EditorGUILayout.Foldout(GetProperty("isUnityIapSettingOpened").boolValue, "Unity IAP");
        if (GetProperty("isUnityIapSettingOpened").boolValue)
        {
            EditorGUILayout.LabelField(new GUIContent("Product ID Consumable", "Please enter your Product ID Consumable"));
            GetProperty("ProductIDConsumable").stringValue = EditorGUILayout.TextArea(GetProperty("ProductIDConsumable").stringValue);
            EditorGUILayout.LabelField(new GUIContent("Reward for purchase"));
            GetProperty("coinsIAPReward").intValue = Int32.Parse(EditorGUILayout.TextArea(GetProperty("coinsIAPReward").intValue.ToString()));
        }
#endif
#if UNITY_ADS || ENABLE_ADMOB
        GetProperty("isUnityAdsSettingOpened").boolValue = EditorGUILayout.Foldout(GetProperty("isUnityAdsSettingOpened").boolValue, "Rewarded video settings");
        if (GetProperty("isUnityAdsSettingOpened").boolValue)
        {
            EditorGUILayout.LabelField(new GUIContent("Reward for watching the video", "Please enter the reward value"));
            GetProperty("coinsAdReward").intValue= Int32.Parse(EditorGUILayout.TextArea(GetProperty("coinsAdReward").intValue.ToString()));
            EditorGUILayout.LabelField(new GUIContent("Delay between ADs watching HH:MM:SS", "Please enter the needed delay"));
            GetProperty("delayBetweenAds").stringValue = EditorGUILayout.TextArea(GetProperty("delayBetweenAds").stringValue);
            EditorGUILayout.LabelField(new GUIContent("Show AD after each \\X\\ level", "Devider for a level number after which AD should be shown"));
            GetProperty("showAdAfterLevel").intValue = Int32.Parse(EditorGUILayout.TextArea(GetProperty("showAdAfterLevel").intValue.ToString()));
        }
#endif
#if ENABLE_ADMOB
        GetProperty("isUnityAdMobSettingOpened").boolValue = EditorGUILayout.Foldout(GetProperty("isUnityAdMobSettingOpened").boolValue, "AdMob Settings");
        if (GetProperty("isUnityAdMobSettingOpened").boolValue)
        {
#if UNITY_ANDROID           
            EditorGUILayout.LabelField(new GUIContent("Android AdMob App ID", "Please enter your Android AdMob App ID"));
            GetProperty("adMobAndroidAppID").stringValue = EditorGUILayout.TextArea(GetProperty("adMobAndroidAppID").stringValue);
            EditorGUILayout.LabelField(new GUIContent("Android AdMob Rewarded Unit ID", "Please enter your Android AdMob Rewarded Unit ID"));
            GetProperty("adMobAndroidRewardedID").stringValue = EditorGUILayout.TextArea(GetProperty("adMobAndroidRewardedID").stringValue);
            EditorGUILayout.LabelField(new GUIContent("Android AdMob Interstitial Unit ID", "Please enter your Android AdMob Interstitial Unit ID"));
            GetProperty("adMobAndroidInterstitialID").stringValue = EditorGUILayout.TextArea(GetProperty("adMobAndroidInterstitialID").stringValue);
#endif
#if UNITY_IOS
            EditorGUILayout.LabelField(new GUIContent("IOS AdMob App ID", "Please enter your IOS AdMob App ID"));
            GetProperty("adMobIosAppID").stringValue = EditorGUILayout.TextArea(GetProperty("adMobIosAppID").stringValue);
            EditorGUILayout.LabelField(new GUIContent("IOS AdMob Rewarded Unit ID", "Please enter your IOS AdMob Rewarded Unit ID"));
            GetProperty("adMobIosRewardedID").stringValue = EditorGUILayout.TextArea(GetProperty("adMobIosRewardedID").stringValue);
            EditorGUILayout.LabelField(new GUIContent("IOS AdMob Interstitial Unit ID", "Please enter your IOS AdMob Interstitial Unit ID"));
            GetProperty("adMobIosInterstitialID").stringValue = EditorGUILayout.TextArea(GetProperty("adMobIosInterstitialID").stringValue);
#endif
        }
#endif
        if (GUI.changed)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    void SetScriptingSymbol(string symbol, BuildTargetGroup target, bool isActivate)
    {
        var s = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);

        s = s.Replace(symbol + ";", "");

        s = s.Replace(symbol, "");

        if (isActivate)
            s = symbol + ";" + s;

        PlayerSettings.SetScriptingDefineSymbolsForGroup(target, s);
    }

}
