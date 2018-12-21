using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//The main class that constructs new level graphics and UI
public class UIManager : MonoBehaviour
{
    //References to UI gameobjects
    public Image taskImage;
    public Text taskLevel;
    public Transform letterPrefab;
    public Transform letterFieldPrefab;
    public Transform border;
    public Transform main;
    public Transform lettersBoard;
    public Transform lettersFields;
    public Button letterDeleteButton;
    public Button letterClearButton;
    public Button menuButton;
    public Button hintsButton;
    public Transform hintsPopup;
    public Transform menuPopup;
    public Transform winPopup;
    public Texture2D texture;
    public GameObject ratePrefab;

    //Collections with letters
    private List<Letter> letterList;
    private List<LetterField> letterFieldsList;
    private string currentAnswer;
    public static Color levelColor;

    //This events is for extensions reason. You can easily subscribe some new logic to them
    public static event UnityAction<int, object> OnHintUsed;
    public static event UnityAction OnLevelComplete;


    private void Awake()
    {

        SubscribeForEvents();
    }

    private void Update()
    {
        CheckForRightAnswer();
    }

    private void CheckForRightAnswer() //Check if the users input matches the right answer after last field filled
    {
        if (!LevelManager.isPaused)
        {
            if (letterFieldsList[letterFieldsList.Count - 1].text.text != null)
            {
                foreach (var item in letterFieldsList)
                {
                    currentAnswer += item.text.text.ToLower();
                }
                if (currentAnswer == LevelManager.rightAnswerNoSpaces)
                {
                    winPopup.gameObject.SetActive(true);
                    OnLevelComplete();
                }
                else currentAnswer = string.Empty;
            }
        }

    }

    private void SubscribeForEvents()
    {
        try
        {
            LevelManager.OnLevelStart += SpawnTask;
            HintPopup.OnHintPopupClicked += AddButtonsListeneres;
            letterDeleteButton.onClick.AddListener(() => StaticBehaviors.ClearField(letterFieldsList));
            letterClearButton.onClick.AddListener(() => StaticBehaviors.ClearAll(letterFieldsList));
            hintsButton.onClick.AddListener(() => hintsPopup.gameObject.SetActive(true));
            menuButton.onClick.AddListener(() => menuPopup.gameObject.SetActive(true));
            OnHintUsed += DataManager.Instance.SpendCoins;
        }
        catch (System.NullReferenceException newEx)
        {
            Debug.LogError("There is no reference to " + newEx.Source);
        }
        catch (System.Exception)
        {
            Debug.LogError("You should start the game from 'StartMenu' scene. Or some references are broken");
        }
    }

    private void AddButtonsListeneres(HintPopup popup)
    {
        popup.buttons[0].onClick.AddListener(() => UseHint(0));
        popup.buttons[1].onClick.AddListener(() => UseHint(1));
        popup.buttons[2].onClick.AddListener(() => UseHint(2));
    }

    private void SpawnTask(LevelInfo data, GameType type) //Create a new level scene
    {
        if (DataManager.Instance.RemoveClearButtons)
        {
            letterDeleteButton.transform.parent.gameObject.SetActive(false);
            lettersFields.GetComponent<RectTransform>().sizeDelta += new Vector2(200f, 0f);
        }
        if (DataManager.Instance.isRatePopupNeeded && PlayerPrefs.GetInt("rate", 0) == 0 && (data.currentLevel - 1) != 0 &&
            (data.currentLevel - 1) % DataManager.Instance.afterEeachLevel == 0)
        {
            Instantiate(ratePrefab, main);
        }
        try
        {
            // Load and create new sprite from Resources in RAM optimization purpose
            Texture2D newtask = Resources.Load(data.directoryName + "\u002F" + data.currentLevel) as Texture2D;
            taskImage.sprite = Sprite.Create(newtask, new Rect(0, 0, newtask.width, newtask.height), new Vector2(0.5f, 0.5f));
        }
        catch (System.Exception)
        {
            Debug.LogError("You made a mistake in filenames in the Resources folder probably");
        }

        levelColor = DataManager.Instance.colors[Random.Range(0, DataManager.Instance.colors.Length - 1)];
        StaticBehaviors.SetCoinsText();

        //Provide requested manager with data and start its logic
        switch (type)
        {
            case GameType.Default:
                break;

            case GameType.Pixel:

                PixelManager pixel = taskImage.gameObject.GetComponent<PixelManager>();
                pixel.enabled = true;
                pixel.OnStart(data);
                break;

            case GameType.Erasure:

                ErasureManager erasure = taskImage.gameObject.GetComponent<ErasureManager>();
                erasure.enabled = true;
                erasure.OnStart(data);
                break;

            case GameType.Planks:

                PlanksManager planks = taskImage.gameObject.GetComponent<PlanksManager>();
                planks.OnStart(data);
                break;

            case GameType.ImageText:
                ImageTextManager imageText = taskImage.gameObject.GetComponent<ImageTextManager>();
                imageText.OnStart();
                break;

            default:
                break;
        }

        taskLevel.text = data.currentLevel.ToString();
        letterList = CreateLetterList(LevelManager.fullList); //Instantiate letter prefabs and add their scripts to the collection
        letterFieldsList = CreateLetterFields(LevelManager.rightAnswerList);//Instantiate letter fields and add their scripts to the collection
        border.gameObject.GetComponent<Image>().color = levelColor;

        StartCoroutine(SpawnLetters(letterList, data)); //Spawn letters 
        MusicManager.instance.PlaySound("start");
    }

    public void ClearAll()
    {
        StaticBehaviors.ClearAll(letterFieldsList);
    }

    private IEnumerator SpawnLetters(List<Letter> letterList, LevelInfo data)
    {
        foreach (var item in letterList)
        {
            item.gameObject.GetComponent<Image>().enabled = true;
            item.gameObject.GetComponentInChildren<Text>().enabled = true;
            item.gameObject.GetComponent<Animation>().Play("FadeIn");
            yield return new WaitForSeconds(0.06f); //Wait a bit to make a dominoes effect for letters
        }
        yield return new WaitForSeconds(0.4f);
        //Handle if some hints already been used earlier
        if (data.lettersOppened != 0)
        {
            int i = data.lettersOppened;
            do
            {
                StaticBehaviors.RevealLetter(letterFieldsList, letterList, LevelManager.rightAnswerList);
                i--;
            } while (i != 0);
        }
        if (data.isLettersRemoved)
        {
            StaticBehaviors.RemoveWrongLetters(letterFieldsList, letterList, LevelManager.rightAnswerList);
        }

        LevelManager.isPaused = false;
    }

    List<Letter> CreateLetterList(List<char> arr) //Instantiate letter prefabs and add their scripts to collection
    {
        List<Letter> letterList = new List<Letter>();
        foreach (char item in arr)
        {
            GameObject go = Instantiate(letterPrefab.gameObject, lettersBoard);
            go.GetComponent<Image>().color = levelColor;
            string letter = item.ToString().ToUpper();
            Letter script = go.GetComponent<Letter>();
            script.textField.text = letter;
            script.Clicked += x => StaticBehaviors.LetterClick(x, letterFieldsList);
            letterList.Add(script);
        }
        return letterList;
    }

    List<LetterField> CreateLetterFields(List<char> arr) //Instantiate letter fields and add their scripts to collection
    {
        List<LetterField> letterArr = new List<LetterField>();
        foreach (char item in arr)
        {
            GameObject go = Instantiate(letterFieldPrefab.gameObject, lettersFields);
            if (item != ' ')
            {
                LetterField _this = go.GetComponent<LetterField>();
                letterArr.Add(_this);
                if (DataManager.Instance.RemoveClearButtons)
                {
                    
                    Button temp = go.AddComponent<Button>();
                    temp.transition = Selectable.Transition.None;
                    temp.onClick.AddListener(() => StaticBehaviors.Clear(_this));
                }
            }
            else
            {
                go.GetComponent<Image>().enabled = false;
            }
        }
        return letterArr;
    }

    //Hints handler
    private void UseHint(int index)
    {
        if (StaticBehaviors.EnoughCoinsForHint(index))
        {
            switch (index)
            {
                case 0:
                    StaticBehaviors.RevealOneLetter(letterFieldsList, letterList, LevelManager.rightAnswerList);
                    break;
                case 1:
                    StaticBehaviors.RemoveWrongLetters(letterFieldsList, letterList, LevelManager.rightAnswerList);
                    break;
                case 2:
                    StaticBehaviors.SolveTask(letterFieldsList, letterList, LevelManager.rightAnswerList);
                    break;
            }
            HintEvent(index, null);
            hintsPopup.gameObject.SetActive(false);
        }

    }
    public static void HintEvent(int hint, object obj)
    {
        OnHintUsed(hint, obj);
    }
    private void OnDestroy()
    {
        OnHintUsed = null;
        OnLevelComplete = null;
    }
}
