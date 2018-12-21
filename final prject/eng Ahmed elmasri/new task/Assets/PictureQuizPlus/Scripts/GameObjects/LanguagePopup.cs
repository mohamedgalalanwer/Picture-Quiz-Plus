using UnityEngine;
using UnityEngine.UI;

public class LanguagePopup : MonoBehaviour //Language popup component class
{

    public Transform field;
    public GameObject languagePrefab;

    void Start()
    {
        var fileNames = DataManager.Instance.Languages; //Get available languages
        foreach (var item in fileNames)
        {
            SpawnLanguageButton(item);
        }
    }

    private void SpawnLanguageButton(string item)
    {
        string current = item;
        GameObject temp = Instantiate(languagePrefab, field);
        temp.transform.Find("Text").GetComponent<Text>().text = current.ToUpper();
        try
        {
            Texture2D newSprite = Resources.Load(current) as Texture2D;
            Sprite tempSprite = Sprite.Create(newSprite, new Rect(0, 0, newSprite.width, newSprite.height), new Vector2(0.5f, 0.5f));
            temp.GetComponent<Image>().sprite = tempSprite;
        }
        catch (System.Exception)
        {
            Debug.LogWarning(string.Format("There is no \"{0}\" sprite in Resources folder", current));
        }

        temp.GetComponent<Button>().onClick.AddListener(() => { ChangeLanguage(current); MusicManager.instance.PlaySound("delete"); });

    }

    void ChangeLanguage(string lang)
    {
        if (PlayerPrefs.GetString("language") == lang)
        {
            gameObject.SetActive(false);
        }
        else
        {
            PlayerPrefs.SetString("language", lang);
            DataManager.Instance.ZeroingHintAfterLanguageChanged();
            DataManager.Instance.LoadLocalizedText(lang);
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            gameObject.SetActive(false);
        }
    }

}
