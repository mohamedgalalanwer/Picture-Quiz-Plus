using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HintPopup : MonoBehaviour //Component class for hints popup
{
    public Button[] buttons; //Array of buttons references
    public Button IAPbutton; //Reference to the IAPbutton setted from the Inspector
    public static event UnityAction<HintPopup> OnHintPopupClicked; //Event is fired when popup created

    private void OnEnable()
    {   
        //Is 'Remove letters' button should be disabled
        if (LevelManager.GetCurrentState().isLettersRemoved && buttons[1].interactable == true)
        {
            StaticBehaviors.DisableButton(buttons[1]);
        }
        LevelManager.isPaused = true;
        MusicManager.instance.PlaySound("menus");
    }
    private void OnDisable()
    {
        LevelManager.isPaused = false;
        transform.Find("ADcanvas").gameObject.SetActive(false);
    }

    void Start()
    {
        OnStart();
    }

    void Update()
    {
        if (Input.GetKeyDown("escape")) //Is Escape or Back pressed
        {
          gameObject.SetActive(false);
        }
    }
   
    public void OnStart()
    {
        OnHintPopupClicked(this); //Event is fired when popup created

        //Attach OnClick methods to buttons
        buttons[3].onClick.AddListener(() => GetComponent<Animator>().Play("adFlip")); //Play flip animation on Get Coins clicked 
        #if UNITY_IAP
        IAPbutton.onClick.AddListener(() => IAP.instance.BuyConsumable()); //Try to buy setted consumable 
        #endif
        GetComponent<Button>().onClick.AddListener(() => gameObject.SetActive(false)); //Disable popup wnen background image clicked
    }
    
    private void OnDestroy()
    {
        OnHintPopupClicked = null;
    }

}
