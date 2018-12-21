using UnityEngine;
using UnityEngine.UI;

public class MenuPopup : MonoBehaviour //Component class for menu popup
{
    //Buttons references
    public Button sound;
    public Button music;
    public Button backToGame;
    public Button backToMainMenu;
    public Button exit;

    private void Start()
    {   
        Initialize();
    }

    private void OnEnable()
    {
        LevelManager.isPaused = true;
        MusicManager.instance.PlaySound("menus");
    }
    private void OnDisable()
    {
        LevelManager.isPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            gameObject.SetActive(false);
        }
    }

    private void Initialize()
    {
        //Read registry keys to setup buttons activity
        if (!MusicManager.instance.isMusic)
        {
            music.GetComponent<Image>().color -= new Color(0, 0, 0, 0.5f);
        }
        if (!MusicManager.instance.isSounds)
        {
            sound.GetComponent<Image>().color -= new Color(0, 0, 0, 0.5f);
        }

        //Attach OnClick methods to menu buttons
        music.onClick.AddListener(() => StaticBehaviors.MusicClick(music));
        sound.onClick.AddListener(() => StaticBehaviors.SoundsClick(sound));
        backToGame.onClick.AddListener(() => gameObject.SetActive(false));
        backToMainMenu.onClick.AddListener(() => StaticBehaviors.LoadScene(0));
        exit.onClick.AddListener(() => Application.Quit());
    }
}
