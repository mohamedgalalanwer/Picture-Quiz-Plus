using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PixelManager : MonoBehaviour //Class to manage task with Pixel Type
{
    //References from the Inspector
    public Button pixelateButton;
    public Sprite buttonSprite;
    public Material pixelMaterial;

    private int pixelateFirst;
    private int pixelateSecond;
    private int pixelateThird;
    private int finalImage;
    private float animationSpeed;
    private int openedPics;
    private int[] arrayOfValues;
    private Image taskImage; 

    public void OnStart(LevelInfo data) //Initializing method called from the UIMnager at the level start
    {
        //Get values from the DataManager
        pixelateFirst = DataManager.Instance.pixelateFirst; 
        pixelateSecond = DataManager.Instance.pixelateSecond;
        pixelateThird = DataManager.Instance.pixelateThird;
        finalImage = DataManager.Instance.finalImage;
        animationSpeed = DataManager.Instance.animationSpeed / 100;

        //Initialize level data
        openedPics = data.openedPictures;
        arrayOfValues = new int[4] { pixelateFirst, pixelateSecond, pixelateThird, finalImage };
        taskImage = GetComponent<Image>(); //Main task image
        taskImage.material = pixelMaterial;

        //Configure hint button
        pixelateButton.GetComponent<Image>().sprite = buttonSprite;

        if (DataManager.Instance.RemoveClearButtons) //Force task image to be a hint button
        {
            taskImage.raycastTarget = true;
            Button newBut = taskImage.gameObject.AddComponent<Button>();
            newBut.transition = Selectable.Transition.None;
            pixelateButton = newBut;
        }
        pixelateButton.onClick.RemoveAllListeners();
        pixelateButton.onClick.AddListener(() => StartCoroutine(Pixelate(openedPics)));
        SetPixelGrid(arrayOfValues[openedPics]); //Set up first view

        if (openedPics == arrayOfValues.Length - 1) 
        {
            pixelateButton.interactable = false; //Disable hint button if all attempts are already wasted
        }
    }

    private void SetPixelGrid(int grid)
    {
        taskImage.material.SetFloat("_PixelCountU", grid);
        taskImage.material.SetFloat("_PixelCountV", grid);
    }

    private IEnumerator Pixelate(int hintCase) //Try to use hint
    {
        if (StaticBehaviors.EnoughCoinsForHint(3))
        {
            MusicManager.instance.PlaySound("pixelate");

            float delay = animationSpeed / 0.2f / Mathf.Abs(arrayOfValues[hintCase] - arrayOfValues[hintCase + 1]);
            pixelateButton.interactable = false;

            for (int i = 1; i < 10; i++) //Animation of decreasing pixel grid
            {
                yield return new WaitForSeconds(animationSpeed);
                SetPixelGrid(arrayOfValues[hintCase] - i);
            }

            for (int i = 1; i < Mathf.Abs(arrayOfValues[hintCase] - arrayOfValues[hintCase + 1]) + 10; i++) //Animation of encreasing pixel grid
            {
                yield return new WaitForSeconds(delay);
                SetPixelGrid(arrayOfValues[hintCase] - 9 + i);
            }

            pixelateButton.interactable = hintCase == arrayOfValues.Length - 2 ? false : true; //Check is hint button should be disabled
            openedPics++;

            UIManager.HintEvent(3, null);
        }
    }
}

