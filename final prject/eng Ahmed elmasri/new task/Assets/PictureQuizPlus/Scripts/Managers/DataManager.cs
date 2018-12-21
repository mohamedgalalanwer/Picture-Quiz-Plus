using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public enum GameType
{
    Default,
    Pixel,
    Erasure,
    Planks,
    ImageText,

}

//Add new elements here when they are needed to be localized
public enum ElementType
{
    play_button, info_button, quit_button, back_button, info_text, locked_text, reveal_a_letter, remove_letters, get_an_answer,
    get_coins, watch_ad_button, purchase_button, continue_button, back_to_menu_button, level_complete_header, rate_text, rate, never, later,
    gdpr_consent_button, personalized_ads_button, ads_dialog, yes, no
}

public class DataManager : MonoBehaviour //Main class that handles and stores game data and preferences
{
    [Header("Common game preferences"), Space(10)]
    public GameType defaultGameType;
    [HideInInspector]
    public char[] randomLetters;
    [Tooltip("The colors of some UI elements that will be chosen randomly")]
    public Color[] colors;

    [Range(8, 14), Tooltip("Letters buttons count")]
    public int howMuchLettersOnBoard;
    [Tooltip("Default reward for victory")]
    public int defaultWinCoins;
    [Tooltip("Price for \"Get An Answer hint\"")]
    public int solveTaskCost;
    [Tooltip("How much coins player have on game start")]
    public int startCoins;

    [Header("Pixelate Game Type"), Space(10)]
    [Range(5, 100), Tooltip("Pixel grid for the task image")]
    public int pixelateFirst = 15;
    [Range(5, 100)]
    public int pixelateSecond = 20;
    [Range(5, 100)]
    public int pixelateThird = 25;
    [Range(5, 100)]
    public int finalImage = 50;
    [Range(1, 10), Tooltip("Pixelation animation speed")]
    public float animationSpeed = 4f;
    [Tooltip("Price for Pixelate hint")]
    public int pixelateCost;

    [Header("Erasure Game Type"), Space(10)]
    [Range(0.05f, 0.5f)]
    public float penSpeed = 0.2f;
    public Texture2D pen;
    public int erasureCost;

    [Header("Planks Game Type"), Space(10)]
    [Range(2, 10), Tooltip("How many planks")]
    public int gridSize = 5;
    [Range(2, 10)]
    public float aimSpeed = 6;
    [Tooltip("Chose between simple clicking on planks or shoting them with aim")]
    public bool isMovingAimType;
    public int plankCost;
    [Header("Level Locking"), Space(10)]
    [Tooltip("Is level locking enabled?")]
    public bool enableLevelLocking;
    public int unlockedAtStart = 2;
    public int unlockStep = 1;
    [Tooltip("Here stores the reference to AD&AIP Settings"), Space(20)]
    public AdsIapSettings adsSettings;
    [Tooltip("Reference to the Language Popup")]
    public GameObject languagePopup;
    [Header("Rate App Popup"), Space(10)]
    [Tooltip("Is Rate Popup Needed")]
    public bool isRatePopupNeeded;
    public int afterEeachLevel = 10;
    public string appUrl = "https://@appStoreUrl";

    [Tooltip("Remove clear buttons to extend letters fields"), Space(20)]
    public bool RemoveClearButtons;

    [HideInInspector]
    public string currentDir;
    [HideInInspector]
    public TasksDictionary[] levelsData;
    [HideInInspector]
    public bool isLocalizedTextReady;
    string path = string.Empty;
    private CurrentTask dictionary;
    private int coins;
    private Dictionary<ElementType, string> localizedText;
    public string[] Languages;

    public SaveManager saveInstance { get; private set; }
    public static DataManager Instance { get; private set; }

    void Awake()
    {
        //Unity singleton 
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        InitLanguage();
        InitializeSaveData();
    }

    private void InitLanguage() //Initialize all text data in the game depending on chosen language
    {
#if UNITY_EDITOR
        string path = Application.dataPath + "/StreamingAssets";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
#endif
        if (!PlayerPrefs.HasKey("language") && (Languages.Count() > 1)) //On first launch. If there is more than one language than popup appears
        {
            languagePopup.SetActive(true);
        }
        else
        {
            if (Languages.Count() == 1) //If only one language available
            {
                PlayerPrefs.SetString("language", Languages.First());
            }
        }
        LoadLocalizedText(PlayerPrefs.GetString("language", "english"));
    }

    public void InitializeSaveData() //Instantiate Save Manager and restore data from the save file
    {

        path = Path.Combine(Application.persistentDataPath, "saves.json");
        saveInstance = new SaveManager(path);
        coins = saveInstance.GetCoins();
#if UNITY_IAP
        gameObject.AddComponent<IAP>();
#endif
#if ENABLE_ADMOB
        gameObject.AddComponent<AdMob>();
#endif
        LevelManager.OnDataSave += SaveGameState; //Subscribe for the data saving event

        adsSettings.nonpersonalizedAds = PlayerPrefs.GetInt("gdpr", 0) == 1 ? true : false;
    }

    public void LoadLocalizedText(string fileName) //Load all text data from json file and store it in the suitable fields
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName + ".json");
        string _default = Path.Combine(Application.dataPath + "/PictureQuizPlus/Resources", "english.json");

#if UNITY_ANDROID && !UNITY_EDITOR //On android you cant Read and write directly to "Path". It is needed to be opened by any jar reader (WWW class in our case)
        string filePathAndroid = Path.Combine(Application.persistentDataPath, fileName + ".json");
        WWW loadDB = new WWW(filePath);
        while (!loadDB.isDone) { };
        if (File.Exists(filePathAndroid))
        {
            string oldfile = File.ReadAllText(filePathAndroid);
            if (loadDB.text != oldfile)
            {
                File.Delete(filePathAndroid);
                File.WriteAllBytes(filePathAndroid, loadDB.bytes);
            }
        }
        else
        {
            File.WriteAllBytes(filePathAndroid, loadDB.bytes);
        }
        filePath = filePathAndroid;
#endif
        //Read localized data from file if it exists, or load default english file from resources folder when in the editor
        string dataAsJson = File.Exists(filePath) ? File.ReadAllText(filePath) :
        File.Exists(_default) ? File.ReadAllText(_default) :
        (new Func<string>(() => { PlayerPrefs.SetString("language", Languages.First()); return string.Empty; }))();

        localizedText = new Dictionary<ElementType, string>();

        if (!string.IsNullOrEmpty(dataAsJson))
        {
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);
            for (int i = 0; i < loadedData.gameItems.Length; i++)
            {
                localizedText.Add(loadedData.gameItems[i].key, loadedData.gameItems[i].value);
            }
            levelsData = loadedData.tasksData;
            randomLetters = loadedData.randomLetters;
            isLocalizedTextReady = true;
        }
        else
        {
            isLocalizedTextReady = false;
        }
    }

    public string GetLocalizedValue(ElementType key) //Method that is called from each element that needs to know localized text
    {
        if (localizedText.ContainsKey(key))
        {
            return localizedText[key];
        }
        else return "???";
    }

    public CurrentTask GetData()
    {
        return this.GetData(currentDir);
    }
    public CurrentTask GetData(string dirName) //Return information about current level from requested directory as CurrentTask structure
    {
        currentDir = dirName;
        foreach (var item2 in levelsData)
        {
            if (item2.name == currentDir)
            {
                dictionary.gameType = item2.gameType;
                dictionary.localizedDir = item2.localizedName;
            }
        }
        LevelInfo savedData = saveInstance.GetDirectoryState(currentDir);
        dictionary.savedData = savedData;
        dictionary.currentDir = currentDir;
        foreach (var item in levelsData)
        {
            if (item.name == currentDir)
            {
                if (savedData.currentLevel <= item.Levels.Length)
                {
                    dictionary.rightAnswer = item.Levels[savedData.currentLevel - 1].rightAnswer;
                    dictionary.levelCost = item.Levels[savedData.currentLevel - 1].cost == 0 ?
                        defaultWinCoins : item.Levels[savedData.currentLevel - 1].cost;
                    dictionary.imageText = item.Levels[savedData.currentLevel - 1].imageText;
                }
                dictionary.dirLength = item.Levels.Length;
            }
        }
        return dictionary;
    }

    public int GetCoinsCount() //Return coins count to suppliant
    {
        return coins;
    }

    public void SpendCoins(int hint, object obj) //Set coins count when hints are used
    {
        coins -= LevelManager.GetCostValues()[hint];
        saveInstance.SetCoinsData(coins);
        StaticBehaviors.SetCoinsText();
    }

    public void EarnCoins(int value, bool sound) //Set coins count when earning coins
    {
        coins += value;
        if (sound)
        {
            MusicManager.instance.PlaySound("coins");
        }
        saveInstance.SetCoinsData(coins);
        StaticBehaviors.SetCoinsText();
    }

    public void SaveGameState(LevelInfo data) //Save current game state to the file
    {
        saveInstance.SetDirectoryState(data);
        saveInstance.Save(path);
    }

    public string[] GetUnlockedDirs() //Method that returns categories names that are unlocked
    {
        var atStartUnlocked = from x in levelsData
                              where x.orderNumber <= unlockedAtStart
                              select x.name;

        string[] request = atStartUnlocked.ToArray().Concat(saveInstance.GetUnlockedDirectories()).ToArray();
        return request;

    }

    internal void UnlockNextLevels(string[] opened, IEnumerable<TasksDictionary> sorted) //Unlock next leveles depending on the preferences
    {
        var dirsToUnlock = sorted.Select(x => x.name).Except(opened).Take(unlockStep);
        foreach (var item in dirsToUnlock)
        {
            LevelInfo state = saveInstance.GetDirectoryState(item);
            saveInstance.SetDirectoryState(state);
        }
    }

    public TaskExample[] GetTasksInfo(string dirname)
    {
        foreach (var item in levelsData)
        {
            if (item.name == dirname)
            {
                return item.Levels;
            }
        }
        return null;
    }

    public void ZeroingHintAfterLanguageChanged()
    {
        saveInstance.ZeroingLetters();
        saveInstance.Save(path);
    }
}

public struct CurrentTask //Structure with current task data
{
    public LevelInfo savedData;
    public string currentDir;
    public string localizedDir;
    public GameType gameType;
    public string rightAnswer;
    public string imageText;
    public int levelCost;
    public int dirLength;
}