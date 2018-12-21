using UnityEngine;
using UnityEngine.UI;

public class RatePopup : MonoBehaviour
{
    public Button later, never, rate;

    private void Update()
    {
        if (Input.GetKey("escape"))
        {
            Destroy(gameObject);
        }
    }
    // Use this for initialization
    void Start()
    {
        later.onClick.AddListener(() => Destroy(gameObject));
        never.onClick.AddListener(() => { PlayerPrefs.SetInt("rate", 1); Destroy(gameObject); });
        rate.onClick.AddListener(RateApp);
    }

    private void RateApp()
    {
        PlayerPrefs.SetInt("rate", 1);
        Destroy(gameObject);
        Application.OpenURL(DataManager.Instance.appUrl);
    }
}