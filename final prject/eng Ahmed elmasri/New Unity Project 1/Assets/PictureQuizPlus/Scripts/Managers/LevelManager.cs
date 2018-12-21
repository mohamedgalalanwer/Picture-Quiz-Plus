using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class LevelManager : MonoBehaviour //The data part of new level constructor
{
    public static string rightAnswer;
    public static string rightAnswerNoSpaces;
    public static string imageText;
    public static int completeLvlCoins;
    public static bool isPaused = true;
    static public List<char> rightAnswerList; //Collection of right answer chars
    static public List<char> fullList; //Collection of all chars
    static int[] hintsCost;
    static LevelInfo infoTosave; //Structure that holds current game state. It is ready to be saved in every moment
    static GameType type;
    public static event Action<LevelInfo> OnDataSave;
    public static event Action<LevelInfo, GameType> OnLevelStart; 

    private void Awake()
    {
        Initialize();
        HintPopup.OnHintPopupClicked += ProvideHintsCost; //Provide hints buttons with related cost
        UIManager.OnHintUsed += SetLevelState; //New level state to save
        UIManager.OnLevelComplete += SetNewGameState; //Set up next level clear state
    }

    void Start()
    {
        OnLevelStart(infoTosave,type);
    }

    void Initialize() //Levels data constructor
    {
        hintsCost = new int[6];

        try
        {
            CurrentTask temp = DataManager.Instance.GetData();
            type = temp.gameType;
            infoTosave = temp.savedData;
            rightAnswer = temp.rightAnswer.ToLower();
            rightAnswerNoSpaces = rightAnswer.Replace(" ", string.Empty);
            imageText = temp.imageText;
            completeLvlCoins = temp.levelCost;
            int letterCount = rightAnswerNoSpaces.ToCharArray().Length;
            hintsCost[2] = DataManager.Instance.solveTaskCost;
            hintsCost[3] = DataManager.Instance.pixelateCost;
            hintsCost[4] = DataManager.Instance.plankCost;
            hintsCost[5] = DataManager.Instance.erasureCost;

            //Calculations to balance hints cost depending on the words length
            if (hintsCost[2] >= 100)
            {
                hintsCost[1] = Mathf.CeilToInt((hintsCost[2] / 1.8f) / 10) * 10;
                hintsCost[0] = Mathf.CeilToInt((hintsCost[2] * 2.5f / letterCount) / 10) * 10;
            }
            else
            {
                hintsCost[1] = Mathf.CeilToInt(hintsCost[2] / 1.8f);
                hintsCost[0] = Mathf.CeilToInt(hintsCost[2] * 2.5f / letterCount);
            }
            fullList = CreateListOfLetters(DataManager.Instance.randomLetters);
            StaticBehaviors.Shuffle(fullList);
        }
        catch (Exception)
        {
            Debug.LogError("You should start the game from 'StartMenu' scene. Or some references are broken");
        }
    }

    void SetLevelState(int hint, object data) //New level state prepare to save when hint is used
    {
        switch (hint)
        {
            case 0:
                infoTosave.lettersOppened++; 
                break;
            case 1:
                infoTosave.isLettersRemoved = true;
                break;
            case 3:
                infoTosave.openedPictures++;
                break;
            case 4:
                infoTosave.openedPlanks = (int[])data;
                break;
            case 5:
                infoTosave.maskPath = (string)data;
                break;
        }
    }

    void ProvideHintsCost(HintPopup popup) //Provide hints buttons with related cost
    {
        for (int i = 0; i < popup.buttons.Length - 1; i++)
        {
            StaticBehaviors.SetUpCost(popup.buttons[i].transform, hintsCost[i]);
        }
    }

    static private void SetNewGameState() //Set up next level clear state
    {
        isPaused = true;
        int nextlevel = ++infoTosave.currentLevel;
        infoTosave = new LevelInfo(DataManager.Instance.GetData().currentDir, nextlevel);
        DataManager.Instance.EarnCoins(completeLvlCoins, false); //Add coins for victory without sound
        SaveCurrentGameState();
    }

    public static int[] GetCostValues() 
    {
        return hintsCost;
    }

    public static void SaveCurrentGameState() //Event is fired when saving data is needed
    {
        OnDataSave(GetCurrentState());
    }

    public static LevelInfo GetCurrentState()
    {
        return infoTosave;
    }

    private void OnApplicationPause(bool pause)
    {   
        SaveCurrentGameState();
    }

    private void OnApplicationQuit()
    {
        SaveCurrentGameState();
    }

    private void OnDestroy()
    {
        SaveCurrentGameState();
        OnLevelStart = null;
    }
    //Create the list of letters depending on Datamanager preferences
    private List<char> CreateListOfLetters(char[] rndList)
    {
        List<char> temp = new List<char>(rndList);

        char[] array = rightAnswerNoSpaces.ToCharArray();
        rightAnswerList = new List<char>(rightAnswer.ToCharArray()); //Collection of right answer chars
        int diff = DataManager.Instance.howMuchLettersOnBoard - array.Length;
        int rndLettersCount = diff >= 0 ? diff: 0 ;
        char[] rndArray = new char[rndLettersCount];
        for (int i = 0; i < rndArray.Length; i++)
        {
            //Fill the collection with random chars
            int rndIndex = UnityEngine.Random.Range(0, temp.Count);
            rndArray[i] = temp[rndIndex];
            temp.RemoveAt(rndIndex);
        }
        return new List<char>(array.Concat(rndArray)); //Return concatination of collections
    }
}
