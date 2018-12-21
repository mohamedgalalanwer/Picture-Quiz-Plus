using UnityEngine;
using UnityEngine.UI;

//Stretches letters depending on the devices resolution ratio
public class LettersBoard : MonoBehaviour
{
    private const int canvasWidth = 1330; //Global canvas scale constant
    private int sizeX; //Width of the letter buttons
    public static bool ratio2 = false; //Is 18:9(IphoneX) ratio?

    void Awake()
    {
        sizeX = canvasWidth / DataManager.Instance.howMuchLettersOnBoard; //Calculations for letter button width

        int width = Screen.width;
        int height = Screen.height;
        int portraitWidth = width > height ? width : height; //For horizontal view only tablets
        int portraitHeight = width > height ? height : width;
        float ratio = portraitWidth / (float)portraitHeight;
        ratio2 = ratio >= 2;

        if (ratio <= 1.67f) //Is tablets?
        {
            sizeX += 9;
        }
        if (ratio2) //Is 18:9(IphoneX) ratio?
        {
            sizeX -= 5;
            FindObjectOfType<CanvasScaler>().referenceResolution += new Vector2(0, 120f); //Scale main canvas to prevent UI cutting
            GameObject.FindGameObjectWithTag("winImage").transform.parent.
                GetComponent<RectTransform>().sizeDelta -= new Vector2(0, 92f); //Task canvas back to square form
        }
        gameObject.GetComponent<GridLayoutGroup>().cellSize = new Vector2(sizeX, 150f); //Set letters width
    }

}
