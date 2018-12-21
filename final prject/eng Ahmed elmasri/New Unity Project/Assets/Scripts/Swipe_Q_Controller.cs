﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//diferent scenes or one scene changes:
//1. do next questions
//2. Answer Display
//3. start

public class Swipe_Q_Controller : MonoBehaviour
{

    public Swipe_Q_Class[] Questions;
    public ArabicText TextWord;
    public Text TextAnswer1, TextAnswer2;
    public Image ImageAnswer1, ImageAnswer2;

    public float SwipeThreshold = 150f;
    public int Score = 0;
    public float TimeBetweenQuestions = 1.5f;

    Vector2 startPos, direction;
    bool directionChosen;
    SwpipeDirection swipeDirection = SwpipeDirection.None;
    Swipe_Q_Class CurrentQuestion;
    Color OldAnswerColor, OldWordColor, OldImageColor, CorrectColor = Color.green,
        WrongColor = Color.red, Transparent = Color.clear;
    List<int> DisplayedQuestions = new List<int>();
    bool AllQAnswered = false;
    bool CoroutineRunning = false;

    public GameObject StarParent;
    public Sprite EmptyStar, FullStar;
    public Text TimerText;
    public int TimeInSeconds;
    bool TimeUp = false;

    public GameObject CurrentCanvas, ScoreCanvas;

    void Start()
    {
        //for full scene
        CurrentCanvas.SetActive(true);
        GetQuestionFromList();
        OldAnswerColor = TextAnswer1.transform.parent.GetComponent<Image>().color;
        OldWordColor = TextWord.transform.parent.GetComponent<Image>().color;
        OldImageColor = ImageAnswer1.GetComponent<Image>().color;
    }

    //update used in swipe
    void Update()
    {
        if (!CoroutineRunning)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        startPos = touch.position;
                        directionChosen = false;
                        break;
                    case TouchPhase.Moved:
                        direction = touch.position - startPos;
                        break;
                    case TouchPhase.Ended:
                        directionChosen = true;
                        break;
                }
            }
            if (directionChosen)
            {
                if (direction.y > SwipeThreshold)
                {
                    swipeDirection = SwpipeDirection.UP;
                }
                else if (direction.y < -SwipeThreshold)
                {
                    swipeDirection = SwpipeDirection.Down;
                }
                direction.y = 0f;
                directionChosen = false;
                if (swipeDirection != SwpipeDirection.None)
                    AnswerCheck();
            }
        }
        if (AllQAnswered)
            DoNextQuestions();
    }

    void AnswerCheck()
    {
        if (swipeDirection == SwpipeDirection.UP && CurrentQuestion.CorrectAnswer == CorrectAnswerDirection.FirstAnswer)
        {
            StartCoroutine(AnswerDisplay(true, true));
        }
        else if (swipeDirection == SwpipeDirection.Down && CurrentQuestion.CorrectAnswer == CorrectAnswerDirection.SecondAnswer)
        {
            StartCoroutine(AnswerDisplay(true, false));
        }
        else
        {
            if (swipeDirection == SwpipeDirection.UP)
                StartCoroutine(AnswerDisplay(false, true));
            else
                StartCoroutine(AnswerDisplay(false, false));
        }
        swipeDirection = SwpipeDirection.None;
    }

    IEnumerator AnswerDisplay(bool Correct, bool Top)
    {
        CoroutineRunning = true;
        if (!TimeUp)
        {
            if (Correct)
            {
                Score += 10;
                SoundController.PlaySoundCorrect();
                //for Full Game Controller
                GameControllerScript.Score += 10;
                TextWord.transform.parent.GetComponent<Image>().color = CorrectColor;
                if (Top)
                {
                    ImageAnswer1.GetComponent<Image>().color = CorrectColor;
                    TextAnswer1.transform.parent.GetComponent<Image>().color = CorrectColor;

                    ImageAnswer2.GetComponent<Image>().color = Transparent;
                    TextAnswer2.transform.parent.GetComponent<Image>().color = Transparent;
                    TextAnswer2.text = "";
                }
                else
                {
                    ImageAnswer2.GetComponent<Image>().color = CorrectColor;
                    TextAnswer2.transform.parent.GetComponent<Image>().color = CorrectColor;

                    ImageAnswer1.GetComponent<Image>().color = Transparent;
                    TextAnswer1.transform.parent.GetComponent<Image>().color = Transparent;
                    TextAnswer1.text = "";
                }
            }
            else
            {
                SoundController.PlaySoundWrong();
                TextWord.transform.parent.GetComponent<Image>().color = WrongColor;
                if (Top)
                {
                    ImageAnswer1.GetComponent<Image>().color = WrongColor;
                    TextAnswer1.transform.parent.GetComponent<Image>().color = WrongColor;
                }
                else
                {
                    ImageAnswer2.GetComponent<Image>().color = WrongColor;
                    TextAnswer2.transform.parent.GetComponent<Image>().color = WrongColor;
                }
            }
        }
        else
        {
            TextWord.transform.parent.GetComponent<Image>().color = WrongColor;
            ImageAnswer1.GetComponent<Image>().color = WrongColor;
            TextAnswer1.transform.parent.GetComponent<Image>().color = WrongColor;
            ImageAnswer2.GetComponent<Image>().color = WrongColor;
            TextAnswer2.transform.parent.GetComponent<Image>().color = WrongColor;
        }
        yield return new WaitForSeconds(TimeBetweenQuestions);
        ChangeStars();
        NextQuestion();
        CoroutineRunning = false;
    }

    void NextQuestion()
    {
        TextWord.transform.parent.GetComponent<Image>().color = OldWordColor;
        ImageAnswer1.GetComponent<Image>().color = OldImageColor;
        ImageAnswer2.GetComponent<Image>().color = OldImageColor;
        TextAnswer1.transform.parent.GetComponent<Image>().color = OldAnswerColor;
        TextAnswer2.transform.parent.GetComponent<Image>().color = OldAnswerColor;
        GetQuestionFromList();
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
                Debug.Log("All Swipe Selected");
                AllQAnswered = true;
                return;
            }
            i = R.Next(Questions.Length);
        }
        DisplayedQuestions.Add(i);

        Swipe_Q_Class Q = new Swipe_Q_Class(Questions[i].QuestionWord, Questions[i].Answer1, Questions[i].Answer2
            , Questions[i].CorrectAnswer, Questions[i].Answer1Img, Questions[i].Answer2Img);
        TextWord.Text = Q.QuestionWord;
        TextAnswer1.text = Q.Answer1;
        TextAnswer2.text = Q.Answer2;
        ImageAnswer1.sprite = Q.Answer1Img;
        ImageAnswer2.sprite = Q.Answer2Img;
        CurrentQuestion = Q;
        SoundController.PlaySoundQuestion();
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
            TimerText.text = t.ToString();
            yield return new WaitForSeconds(1f);
            t--;
            StartCoroutine(Timer(t));
        }
        if (t == 0)
        {
            SoundController.PlaySoundTimeUP();
            TimeUp = true;
            StartCoroutine(AnswerDisplay(false, false));
        }
    }
}