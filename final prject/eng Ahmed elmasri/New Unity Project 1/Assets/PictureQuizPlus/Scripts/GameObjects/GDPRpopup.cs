using UnityEngine;
using UnityEngine.UI;
#if GDPR
using UnityEngine.Analytics;
#endif

public class GDPRpopup : MonoBehaviour
{
    public GameObject consent;
    public Button ads, yes, no, back;

    // Use this for initialization
    void Start()
    {
#if GDPR
        DataPrivacyButton but = consent.AddComponent<DataPrivacyButton>();
        but.onClick.AddListener(() => MusicManager.instance.PlaySound("blup"));
#endif
        ads.onClick.AddListener(() => gameObject.GetComponent<Animator>().Play("AdsConsentFlip"));
        if (DataManager.Instance.adsSettings.nonpersonalizedAds)
        {
            ChangeColor(no);
        }
        else
        {
            ChangeColor(yes);
        }
        yes.onClick.AddListener(() => HandleADconsent(true));
        no.onClick.AddListener(() => HandleADconsent(false));
        back.onClick.AddListener(() => { Destroy(gameObject); MusicManager.instance.PlaySound("blup"); });
    }

    private void ChangeColor(Button but)
    {
        if (but.gameObject.GetComponent<Image>().color == Color.white)
        {
            but.gameObject.GetComponent<Image>().color -= new Color(0, 0.5f, 0.5f, 0);
        }
        else
        {
            but.gameObject.GetComponent<Image>().color += new Color(0, 0.5f, 0.5f, 0);
        }

    }

    private void HandleADconsent(bool state)
    {
        if (state)
        {
            if (DataManager.Instance.adsSettings.nonpersonalizedAds)
            {
                ChangeColor(yes);
                ChangeColor(no);
                PlayerPrefs.SetInt("gdpr", 0);
                DataManager.Instance.adsSettings.nonpersonalizedAds = false;
#if ENABLE_ADMOB
                AdMob.RequestAtStart();
#endif
            }
        }
        else
        {
            if (!DataManager.Instance.adsSettings.nonpersonalizedAds)
            {
                ChangeColor(no);
                ChangeColor(yes);
                PlayerPrefs.SetInt("gdpr", 1);
                DataManager.Instance.adsSettings.nonpersonalizedAds = true;
#if ENABLE_ADMOB
                AdMob.RequestAtStart();
#endif
            }

        }

    }

}
