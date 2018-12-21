using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveManager //Handle save data interactions
{
    DataToSave saveData; //Main structure that stores all loaded or created game data and ready to be saved

    //Load saved data
    public SaveManager(string path)
    {
        saveData = LoadDataFromJson(path);
    }
    //Check the save file exists
    public bool Check(string path)
    {
        return File.Exists(path);
    }
    
    //Save prepared data
    public void Save(string path)
    {
        SaveDataToJson(path);
    }
    
    private DataToSave LoadDataFromJson(string path)
    {
        if (Check(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<DataToSave>(json);
        }
        else 
        {
            return new DataToSave(DataManager.Instance.startCoins);
        }
    }

    private void SaveDataToJson(string path)
    {
        string json = JsonUtility.ToJson(saveData, true);
        StreamWriter sw = File.CreateText(path);
        sw.Close();
        File.WriteAllText(path, json);
    }

    //Write fresh level states to the data structure
    public void SetDirectoryState(LevelInfo current)
    {
        saveData.levelsInfo.RemoveAll(t => t.directoryName == current.directoryName); //Replace old data with new
        saveData.levelsInfo.Add(current); 
    }
    //Returns the requested directory state
    public LevelInfo GetDirectoryState(string dirName)
    {
        if (saveData.levelsInfo.Count != 0)
        {
            foreach (var item in saveData.levelsInfo)
            {
                if (item.directoryName == dirName)
                {
                    return item;
                }
            }
        }
        return new LevelInfo(dirName, 1); //Return clear state if there is no saved data
    }

    //Handle coins changes
    public void SetCoinsData(int coins)
    {
        saveData.coinsCount = coins;
    }

    public int GetCoins()
    {
        return saveData.coinsCount;
    }

    public string[] GetUnlockedDirectories()
    {
        var request = from x in saveData.levelsInfo
                      select x.directoryName;
        return request.ToArray();
    }

    public void ZeroingLetters()
    {
        for (int i = 0; i < saveData.levelsInfo.Count; i++)
        {
            saveData.levelsInfo[i].lettersOppened = 0;
        }
    }

}
