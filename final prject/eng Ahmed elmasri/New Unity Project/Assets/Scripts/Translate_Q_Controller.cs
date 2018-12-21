using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//diferent scenes or one scene changes:
//1. do next questions
//2. Answer Display
//3. start

public class Translate_Q_Controller : MonoBehaviour
{

    public Translate_Q_Class[] Questions;
    public ArabicText TextQuestion;
    public InputField TextAnswer;
    public GameObject ButtonAnswer;
    public GameObject TextTitle, TextCorrectAnswer;
    public GameObject HeaderPanel;
    public ArabicText TextHeaderPanel;

    public int Score = 0;
    
    Translate_Q_Class CurrentQuestion;
    Color OldHeaderPanelColor, CorrectColor = Color.green, WrongColor = Color.red;
    string OldTextHeaderPanel, CorrectText = "إجابة صحيحة", WrongText = "إجابة خاطئة",
        OldButtonText, NewButtonText = "إستمرار";
    List<int> DisplayedQuestions = new List<int>();
    bool AllQAnswered = false;
    bool ButtonFirstClick = true;

    public GameObject StarParent;
    public Sprite EmptyStar, FullStar;
    public Text TimerText;
    public int TimeInSeconds;
    bool TimeUp = false;

    public GameObject CurrentCanvas, ScoreCanvas;

    private void Start()
    {
        //for full scene
        CurrentCanvas.SetActive(true);
        GetQuestionFromList();
        OldHeaderPanelColor = HeaderPanel.GetComponent<Image>().color;
        OldTextHeaderPanel = TextHeaderPanel.Text;
        OldButtonText = ButtonAnswer.transform.GetChild(0).GetComponent<ArabicText>().Text;
    }

    void GetQuestionFromList()
    {
        TimeUp = false;
        StartCoroutine(Timer(TimeInSeconds));

        System.Random R = new System.Random();

        int i = -1;

        while (i == -1 || DisplayedQuestions.Contains(i) || i >= Questions.Length)
        {
            if (DisplayedQuestions.Count >= Questions.Length)
            {
                Debug.Log("All Translate Selected");
                AllQAnswered = true;
                return;
            }
            i = R.Next(Questions.Length);
        }
        DisplayedQuestions.Add(i);

        Translate_Q_Class Q = new Translate_Q_Class(Questions[i].Question, Questions[i].CorrectAnswer);
        TextQuestion.Text = Q.Question;
        TextCorrectAnswer.GetComponent<Text>().text = Q.CorrectAnswer;
        CurrentQuestion = Q;
        SoundController.PlaySoundQuestion();
    }

    void Update()
    {
        if (TimeUp)
            ButtonAnswer.SetActive(true);
        else if (!TextAnswer.text.Trim().Equals(""))
            ButtonAnswer.SetActive(true);
        else
            ButtonAnswer.SetActive(false);
        if (AllQAnswered)
            DoNextQuestions();
    }

    void DoNextQuestions()
    {
        CurrentCanvas.SetActive(false);
        this.gameObject.SetActive(false);

        //for full controller
        GameControllerScript.ActivateCurrentController();

        //for current controller
        /*ScoreCanvas.SetActive(true);
        PlayerPrefs.SetInt("Score", Score);*/
    }

    public void OnButtonClick()
    {
        if (ButtonFirstClick)
        {
            ButtonFirstClick = false;
            string UserAnswer = TextAnswer.text;
            if (UserAnswer.Trim().ToLower().Equals(CurrentQuestion.CorrectAnswer.Trim().ToLower()))
            {
                AnswerDisplay(true);
            }
            else
            {
                AnswerDisplay(false);
            }
        }
        else
        {
            ButtonFirstClick = true;
            NextQuestion();
        }
    }

    void AnswerDisplay(bool Correct)
    {
        if (Correct)
        {
            Score += 10;
            SoundController.PlaySoundCorrect();
            //for Full Game Controller
            GameControllerScript.Score += 10;
            HeaderPanel.GetComponent<Image>().color = CorrectColor;
            TextHeaderPanel.Text = CorrectText;
        }
        else
        {
            SoundController.PlaySoundWrong();
            HeaderPanel.GetComponent<Image>().color = WrongColor;
            TextHeaderPanel.Text = WrongText;
            TextTitle.SetActive(true);
            TextCorrectAnswer.SetActive(true);
        }
        ButtonAnswer.transform.GetChild(0).GetComponent<ArabicText>().Text = NewButtonText;
        TextAnswer.readOnly = true;

        ChangeStars();
    }

    void NextQuestion()
    {
        HeaderPanel.GetComponent<Image>().color = OldHeaderPanelColor;
        TextHeaderPanel.Text = OldTextHeaderPanel;
        ButtonAnswer.transform.GetChild(0).GetComponent<ArabicText>().Text = OldButtonText;
        TextAnswer.text = "";
        TextAnswer.readOnly = false;
        ButtonAnswer.SetActive(false);
        TextTitle.SetActive(false);
        TextCorrectAnswer.SetActive(false);
        GetQuestionFromList();
    }


    void ChangeStars()
    {
        if (Score == Questions.Length * 10)
        {
            StarParent.transform.GetChild(0).GetComponent<Image>().sprite = FullStar;
            StarParent.transform.GetChild(1).GetComponent<Image>().sprite = FullStar;
            StarParent.transform.GetChild(2).GetComponent<Image>().sprite = FullStar;
        }
        else if (Score >= (Questions.Length * 10 * 0.6f))
        {
            print(Score + "  " + Questions.Length);
            StarParent.transform.GetChild(0).GetComponent<Image>().sprite = FullStar;
            StarParent.transform.GetChild(1).GetComponent<Image>().sprite = FullStar;
            StarParent.transform.GetChild(2).GetComponent<Image>().sprite = EmptyStar;
        }
        else if (Score >= (Questions.Length * 10 * 0.3f))
        {
            StarParent.transform.GetChild(0).GetComponent<Image>().sprite = FullStar;
            StarParent.transform.GetChild(1).GetComponent<Image>().sprite = EmptyStar;
            StarParent.transform.GetChild(2).GetComponent<Image>().sprite = EmptyStar;
        }
    }

    IEnumerator Timer(int t)
    {
        if (!TimeUp)
        {
            if (!ButtonFirstClick)
            {
                TimeUp = true;
            }
            else
            {
                TimerText.text = t.ToString();
                yield return new WaitForSeconds(1f);
                t--;
                StartCoroutine(Timer(t));
            }
        }
        else
        {
            ButtonAnswer.SetActive(true);
            ButtonAnswer.transform.GetChild(0).GetComponent<ArabicText>().Text = NewButtonText;
            OnButtonClick();
        }
        if (t == 0)
        {
            SoundController.PlaySoundTimeUP();
            TimeUp = true;
        }
    }
}