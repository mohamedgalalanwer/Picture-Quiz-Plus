using System;
using System.Collections.Generic;

//Structures to hold levels states in the data file
[Serializable]
public struct DataToSave
{
    public int coinsCount;
    public List<LevelInfo> levelsInfo;
    public DataToSave(int coins)
    {
        coinsCount = coins;
        levelsInfo = new List<LevelInfo>();
    }
}

[Serializable]
public class LevelInfo
{
    public LevelInfo(string name, int current)
    {
        directoryName = name;
        currentLevel = current;
        lettersOppened = 0;
        isLettersRemoved = false;
        openedPictures = 0;
    }
    public string directoryName;
    public int currentLevel;
    public int lettersOppened;
    public bool isLettersRemoved;
    public int openedPictures;
    public int[] openedPlanks;
    public string maskPath;
}

//Classes to hold game data in the DataManager
[Serializable]
public class TaskExample
{
    public int number;
    public string rightAnswer;
    public string imageText;
    public int cost;
    public TaskExample(int number)
    {
        rightAnswer = "?";
        this.number = ++number;
    }
}

[Serializable]
public class TasksDictionary
{
    public string name;
    public string localizedName;
    public GameType gameType;
    public int orderNumber;
    public TaskExample[] Levels;
    public TasksDictionary(string name, int lvls, GameType game, int num)
    {
        orderNumber = ++num;
        gameType = game;
        this.name = name;
        localizedName = name;
        Levels = new TaskExample[lvls];
        for (int i = 0; i < Levels.Length; i++)
        {
            Levels[i] = new TaskExample(i);
        }
    }
}






