using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DirectoryPopup : MonoBehaviour //Component class for directorys popup
{
    public Transform mainTransform; //Main canvas transform to have an easy access
    public GameObject directoryButtonPrefab; //Directory prefab reference
    public Button back; //Reference to the Back button setted from the Inspector
    public GameObject galleryPrefab;
    LevelInfo currentInfo; //Information about the directory from the saved data
    string[] openedDirs;
    public Sprite defaultPicture; //Default directory image
    private CurrentTask currentTask;

    private void Awake()
    {
        CheckResolution(); //Check for 18:9 ratio
        StartCoroutine(SpawnDirectories()); //Spawn directories buttons
    }

    private void CheckResolution()//Check for 18:9 ratio
    {
        int width = Screen.width;
        int height = Screen.height;

        if ((height / width) >= 2 || (width / height) >= 2) //If Yes than expand categories buttons field and scale main canvas
        {
            mainTransform.GetComponent<CanvasScaler>().referenceResolution += new Vector2(0, 120f);
            gameObject.transform.parent.parent.parent.GetComponent<RectTransform>().sizeDelta += new Vector2(100f, 0);
        }
    }

    //Create game directories from the prefab
    private IEnumerator SpawnDirectories()
    {
        yield return new WaitUntil(() => DataManager.Instance.isLocalizedTextReady == true); //Wait for the datamanager is ready to share the data
        try
        {
            //Create new sorted by ordernumber array of levels
            var sortedDictionary = from x in DataManager.Instance.levelsData
                                   orderby x.orderNumber
                                   select x;

            //Check is unlocked categories completed and unlock next if yes
            if (DataManager.Instance.enableLevelLocking)
            {
                openedDirs = DataManager.Instance.GetUnlockedDirs();

                if (CheckIsCompleted())
                {
                    DataManager.Instance.UnlockNextLevels(openedDirs, sortedDictionary);
                    openedDirs = DataManager.Instance.GetUnlockedDirs();
                }
            }

            foreach (var item in sortedDictionary)
            {
                GetComponent<RectTransform>().sizeDelta +=
                new Vector2(0, directoryButtonPrefab.GetComponent<RectTransform>().sizeDelta.y); //Extend the buttons field for each prefab
                GameObject go = Instantiate(directoryButtonPrefab, gameObject.transform);
                SetDirectoryData(go, item.name); //Provide each directory with data
            }
        }
        catch (NullReferenceException)
        {

            Debug.LogError("You should set up Tasks Data in Data Manager");
        }
    }

    //Check is unlocked catalogs completed 
    private bool CheckIsCompleted()
    {
        int counter = 0;
        foreach (var name in openedDirs)
        {
            if ((DataManager.Instance.GetData(name).savedData.currentLevel - 1) / (float)(DataManager.Instance.GetData(name).dirLength) < 1)
            {
                break;
            }
            counter++;
        }
        return counter == openedDirs.Length;
    }

    //Provide each directory with data
    private void SetDirectoryData(GameObject go, string dirName)
    {
        currentTask = DataManager.Instance.GetData(dirName); //Get saved data for directory
        currentInfo = currentTask.savedData;
        int completeLevels = currentInfo.currentLevel - 1;
        int dirLength = currentTask.dirLength;
        go.transform.Find("Image").GetComponent<Image>().sprite = completeLevels == 0 ? defaultPicture : CreateSprite(dirName);

        //Set up directory view
        go.transform.Find("Text1").GetComponent<Text>().text = currentTask.localizedDir.ToUpper();


        //Should it be unlocked?
        if (CheckIsLocked(dirName))
        {

            go.transform.Find("Text2").GetComponent<Text>().text = DataManager.Instance.GetLocalizedValue(ElementType.locked_text).ToUpper();
            go.GetComponent<Button>().onClick.AddListener(() => MusicManager.instance.PlaySound("wrong"));
            go.transform.GetComponent<Image>().color -= new Color(0, 0, 0, 0.4f);

        }
        else
        {
            go.transform.Find("Text2").GetComponent<Text>().text = completeLevels + "\u002f" + dirLength;

            //Set up progress bar 
            float barPercent = (float)completeLevels / (float)dirLength;
            RectTransform bar = go.transform.Find("Bar/ProgressBar").GetComponent<RectTransform>();
            bar.anchoredPosition += new Vector2((bar.sizeDelta.x * barPercent), 0);
            if (barPercent < 1)
            {
                //Add OnClick methods to directory buttons if there are not completed yet
                go.GetComponent<Button>().onClick.AddListener(() => DataManager.Instance.currentDir = dirName);
                go.GetComponent<Button>().onClick.AddListener(() => StaticBehaviors.LoadScene(1));
            }
            else
            {
                go.GetComponent<Button>().onClick.AddListener(() => SpawnGallery(dirName));
            }
        }

    }

    public void SpawnGallery(string dirName)
    {
        GameObject go = Instantiate(galleryPrefab, mainTransform);
        go.GetComponent<Gallery>().OnStart(dirName);
    }


    private bool CheckIsLocked(string dirName)
    {
        if (openedDirs != null)
        {
            foreach (var item in openedDirs)
            {
                if (item == dirName)
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    //Create sprite from Resources folder
    private Sprite CreateSprite(string dirName)
    {
        try
        {
            Texture2D newSprite = Resources.Load(dirName + "\u005C" + (currentInfo.currentLevel - 1).ToString()) as Texture2D;
            Sprite temp = Sprite.Create(newSprite, new Rect(0, 0, newSprite.width, newSprite.height), new Vector2(0.5f, 0.5f));
            return temp;

        }
        catch (System.Exception)
        {
            Debug.LogError(String.Format("No sprite ({1}) founded in {0}", dirName, currentInfo.currentLevel - 1));
            return null;
        }

    }
    private void OnDisable()
    {
        back.gameObject.SetActive(true);
    }
}
