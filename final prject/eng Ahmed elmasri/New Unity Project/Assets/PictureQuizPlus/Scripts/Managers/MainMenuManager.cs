using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//Handle main menu interactions
public class MainMenuManager : MonoBehaviour
{
    public Transform mainTransform;
    public Transform directorys;
    public Transform gameinfo;
    public Transform buttons;
    public Transform languagePopup;
    public Button play, info, exit, back, music, sounds, language, gdpr; //Buttons references
    public GameObject gdprPrefab;

    private void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        yield return new WaitUntil(() => MusicManager.instance != null); //Make sure that MusicManager is loaded
        directorys.gameObject.SetActive(false);

        //Set up sounds buttons color
        if (!MusicManager.instance.isMusic)
        {
            music.GetComponent<Image>().color -= new Color(0, 0, 0, 0.5f);
        }
        if (!MusicManager.instance.isSounds)
        {
            sounds.GetComponent<Image>().color -= new Color(0, 0, 0, 0.5f);
        }
        //Add OnClick methods to all buttons
        play.onClick.AddListener(PlayHandler);
        info.onClick.AddListener(() => { gameinfo.gameObject.SetActive(true); MusicManager.instance.PlaySound("blup"); });
        exit.onClick.AddListener(() => Application.Quit());
        back.onClick.AddListener(BackHandler);
        music.onClick.AddListener(() => StaticBehaviors.MusicClick(music));
        sounds.onClick.AddListener(() => StaticBehaviors.SoundsClick(sounds));

        if (DataManager.Instance.adsSettings.GDPRconsent) //Add GDPR button to the settings panel
        {
            gdpr.gameObject.SetActive(true);
            gdpr.onClick.AddListener(() => Instantiate(gdprPrefab, mainTransform));
        }

        if (DataManager.Instance.Languages.Count() > 1) //Reveal Language popup if there is more than one language
        {
            language.gameObject.SetActive(true);
            language.onClick.AddListener(() => languagePopup.gameObject.SetActive(true));
            language.onClick.AddListener(() => MusicManager.instance.PlaySound("blup"));
        }
    }

    void PlayHandler() //"Play button" logic
    {
        MusicManager.instance.PlaySound("blup");
        gameinfo.gameObject.SetActive(false);
        buttons.gameObject.SetActive(false);
        directorys.gameObject.SetActive(true);
    }

    void BackHandler() //"Back button" logic
    {
        MusicManager.instance.PlaySound("blup");
        directorys.gameObject.SetActive(false);
        buttons.gameObject.SetActive(true);
    }

    public void ClearSavedData() //Use it if you want to create clearing saves button in game
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, "saves.json");
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }
    }

}
