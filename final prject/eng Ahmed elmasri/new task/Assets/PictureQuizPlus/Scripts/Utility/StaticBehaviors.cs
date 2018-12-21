using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif
#if ENABLE_ADMOB
using GoogleMobileAds.Api;
#endif

//Static methods that handle almost all game functionality
public static class StaticBehaviors
{
    public static void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }
    public static void CheckAndWriteFilenames()
    {
        try
        {
            GameObject.FindObjectOfType<DataManager>().Languages = new DirectoryInfo(Application.streamingAssetsPath).GetFiles().
            Where(x => (x.Extension != ".meta")).Select(x => Path.GetFileNameWithoutExtension(x.FullName)).ToArray();

        }
        catch (System.Exception)
        {
            Debug.Log("Cant check languages names, DataManager is not founded on the scene");
        }

    }
    //When the letter clicked
    public static void LetterClick(Letter letter, List<LetterField> list)
    {
        foreach (var item in list)
        {
            if (item.isEmpty)
            {
                letter.GetComponent<Animation>().Play("FadeOut");
                letter.GetComponent<Button>().interactable = false;
                item.text.text = letter.textField.text;
                item.isEmpty = false;
                item.letterReference = letter;

                foreach (var item2 in list)
                {
                    if (item2.isLast)
                    {
                        item2.isLast = false;
                    }
                }
                item.isLast = true;
                MusicManager.instance.PlaySound("blup");
                break;
            }
        }
    }

    //Clears letters field
    public static void Clear(LetterField item)
    {
        if (item.isEmpty == false)
        {
            item.letterReference.GetComponent<Animation>().Play("FadeIn");
            item.text.text = null;
            item.isEmpty = true;
            item.letterReference.GetComponent<Button>().interactable = true;
            item.letterReference = null;
            item.isLast = false;
        }
    }
    //Clears all letters fields
    public static void ClearAll(List<LetterField> list)
    {
        foreach (var item in list)
        {
            if (item.letterReference && !item.isLocked)
            {
                Clear(item);
            }
        }
        MusicManager.instance.PlaySound("delete");
    }
    //Clear one letter field
    public static void ClearField(List<LetterField> list)
    {
        foreach (var item in list)
        {
            if (item.isLast && item.letterReference)
            {
                MusicManager.instance.PlaySound("delete");
                Clear(item);
                if (list.IndexOf(item) >= 1)
                {
                    list[list.IndexOf(item) - 1].isLast = true;
                }

            }
        }

    }

    public static void RevealOneLetter(List<LetterField> fields, List<Letter> letters, List<char> rightLetters)
    {
        ClearAll(fields);
        RevealLetter(fields, letters, rightLetters);
    }

    //'Reveal a letter' hint handler 
    public static void RevealLetter(List<LetterField> fields, List<Letter> letters, List<char> rightLetters)
    {
        foreach (var item in letters)
        {
            if (rightLetters[0] == ' ')
            {
                rightLetters.Remove(rightLetters[0]);
            }
            if (item.textField.text.ToLower() == rightLetters[0].ToString())
            {
                rightLetters.Remove(rightLetters[0]);
                item.GetComponent<Animation>().Play("FadeOut");
                item.GetComponent<Button>().interactable = false;
                foreach (var field in fields)
                {
                    if (field.isEmpty)
                    {
                        field.text.text = item.textField.text;
                        field.isEmpty = false;
                        field.isLocked = true;
                        item.textField.text = null;
                        break;
                    }
                }
                break;
            }
        }
    }

    //"Remove letters" hint handler
    public static void RemoveWrongLetters(List<LetterField> fields, List<Letter> letters, List<char> rightLetters)
    {
        ClearAll(fields);
        List<char> newList = new List<char>(rightLetters);
        int counter = 0;
        foreach (var item in letters)
        {
            if (item.GetComponent<Button>().interactable)
            {
                for (int i = 0; i < newList.Count; i++)
                {
                    if (item.GetComponentInChildren<Text>().text.ToLower() == newList[i].ToString())
                    {
                        newList.Remove(newList[i]);
                        goto Next;
                    }
                }
                //Next instructions are to chose randomly wrong letters that should stay on board depending on words length
                counter++;
                if (rightLetters.Count <= 3)
                {
                    if (counter > 3)
                    {
                        DisableButton(item.GetComponent<Button>());
                    }
                    else
                    {
                        if (Random.Range(0, 3) > 0)
                        {
                            DisableButton(item.GetComponent<Button>());
                            counter--;
                        }
                        goto Next;
                    }
                }
                else
                {
                    if ((rightLetters.Count < 8 && counter > 2) || (rightLetters.Count > 7 && counter > 1))
                    {
                        DisableButton(item.GetComponent<Button>());
                    }
                    else
                    {
                        if (Random.Range(0, 3) > 0)
                        {
                            DisableButton(item.GetComponent<Button>());
                            counter--;
                        }
                        goto Next;
                    }
                }
            }
            Next:;
        }
    }

    //"Get an answer"
    public static void SolveTask(List<LetterField> fields, List<Letter> letters, List<char> rightLetters)
    {
        ClearAll(fields);
        do { RevealLetter(fields, letters, rightLetters); }
        while (rightLetters.Count > 0);
    }


    public static void SetUpCost(Transform go, int value)
    {
        go.Find("Image/cost").GetComponent<Text>().text = value.ToString();
    }

    public static void DisableButton(Button button) //Disabling buttons logic
    {
        if (button.GetComponent<Animation>())
        {
            button.GetComponent<Animation>().Play("FadeOut");
        }
        button.interactable = false;
        button.GetComponent<Image>().color -= new Color(0, 0, 0, 0.5f);
    }
    public static void EnableButton(Button button) //Enabling buttons logic
    {
        if (button.GetComponent<Animation>())
        {
            button.GetComponent<Animation>().Play("FadeIn");
        }
        button.interactable = true;
        button.GetComponent<Image>().color += new Color(0, 0, 0, 0.5f);
    }

    //Handlers for sound buttons
    public static void MusicClick(Button button)
    {
        if (MusicManager.instance.isMusic)
        {
            button.GetComponent<Image>().color -= new Color(0, 0, 0, 0.5f);
            MusicManager.instance.SetMusic(false);
        }
        else
        {
            button.GetComponent<Image>().color += new Color(0, 0, 0, 0.5f);
            MusicManager.instance.SetMusic(true);
        }
    }

    public static void SoundsClick(Button button)
    {
        if (MusicManager.instance.isSounds)
        {
            button.GetComponent<Image>().color -= new Color(0, 0, 0, 0.5f);
            MusicManager.instance.SetSound(false);
        }
        else
        {
            button.GetComponent<Image>().color += new Color(0, 0, 0, 0.5f);
            MusicManager.instance.SetSound(true);
        }
    }

    //What to do after level complete
    public static void OnLevelComplete(LevelInfo data)
    {
        bool isUnityAd = DataManager.Instance.adsSettings.unityAds;
        bool isAdmob = DataManager.Instance.adsSettings.adMob;
        int devider = DataManager.Instance.adsSettings.showAdAfterLevel;

        if (devider > 0) //Show AD depending on setted preferences
        {
            if (((data.currentLevel - 1) % devider) == 0)
            {
                if (isUnityAd && isAdmob)
                {
#if ENABLE_ADMOB && UNITY_ADS
                    if (Random.Range(0, 2) == 1)
                    {
                        if (!AdMob.ShowAdMobAD())
                        {
                            ShowUnityAD();
                        }
                    }

                    else if (!ShowUnityAD())
                    {
                        AdMob.ShowAdMobAD();
                    }
#endif
                }
                else
                {

                    if (isUnityAd)
                    {
#if UNITY_ADS
                        ShowUnityAD();
#endif
                    }
#if ENABLE_ADMOB
                    else AdMob.ShowAdMobAD();
#endif
                }
            }
        }
        //Load new scene
        if (data.currentLevel > DataManager.Instance.GetData(data.directoryName).dirLength)
        {
            LoadScene(0);
        }
        else
        {
            LoadScene(1);
        }
    }

#if UNITY_ADS
    private static bool ShowUnityAD()
    {
        if (Advertisement.IsReady())
        {
            Advertisement.Show();
            return true;
        }
        else return false;
    }
#endif

    public static void Shuffle<T>(List<T> list) //Randomly shuffle a collection
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            T tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }

    public static void SetCoinsText() //Coins UI text handler
    {
        GameObject coins = GameObject.FindGameObjectWithTag("coinText");
        coins.GetComponent<Animator>().Play("coinDrag");
        coins.GetComponent<Text>().text = DataManager.Instance.GetCoinsCount().ToString();
    }

    public static bool EnoughCoinsForHint(int hint)
    {
        int hintCost = LevelManager.GetCostValues()[hint];

        if (DataManager.Instance.GetCoinsCount() < hintCost)
        {
            MusicManager.instance.PlaySound("wrong");
            SetCoinsText();
            return false;
        }
        else return true;
    }

    public static bool EnoughCoinsForHint(int hint, bool isSoundNeeded)
    {
        int hintCost = LevelManager.GetCostValues()[hint];

        if (DataManager.Instance.GetCoinsCount() < hintCost)
        {
            if (isSoundNeeded)
            {
                MusicManager.instance.PlaySound("wrong");
            }
            SetCoinsText();
            return false;
        }
        else return true;
    }

    public static string[] CutNames(string[] array)
    {
        string[] cuttedArr = new string[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            string[] temp = array[i].Split(char.Parse("\u005C"));
            string tempString = temp[temp.Length - 1].Split('.')[0];
            cuttedArr[i] = tempString;
        }
        return cuttedArr;
    }
}

