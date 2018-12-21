using UnityEngine;
using UnityEngine.UI;

public class ImageTextManager : MonoBehaviour //Component class to handle Image+Text tasks
{
    public GameObject textArea;
    public Text text;
    // Use this for initialization
    public void OnStart()
    {
        textArea.SetActive(true);
        text.text = LevelManager.imageText;
    }

}
