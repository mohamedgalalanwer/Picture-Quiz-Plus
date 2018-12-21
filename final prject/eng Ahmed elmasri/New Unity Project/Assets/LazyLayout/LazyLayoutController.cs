using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LazyLayoutController : MonoBehaviour 
{    
    public GameObject canvasInScene;

    [SerializeField]
    public List<LazyLayout.LL_Layout> savedLazyLayouts = new List<LazyLayout.LL_Layout>();

    RectTransform[] rectTransformsInScene;

    public int index_CurrentlySelectedLayout;

    public bool showSavedLayouts, 
                loadLayoutInAwake, adaptLayoutsInRuntime,
                switchTextContent;

    public string layoutName;

    Vector2 savedScreenSize;


    void Awake()
    {
        if (loadLayoutInAwake)
        { LoadLayoutAtStartUp(); }      
    }

    void Start()
    {
        if (!loadLayoutInAwake)
        { LoadLayoutAtStartUp(); }         
    }

    void Update()
    {
        if (adaptLayoutsInRuntime)
        {
            if (Screen.width != savedScreenSize.x || Screen.height != savedScreenSize.y)
            {
                // We have to wait one frame so Unity recognizes the corret Screen size and AR
                Invoke("ApplySavedLayoutAtRuntime", Time.deltaTime);
            }
        }

        savedScreenSize = new Vector2(Screen.width, Screen.height);
    }


    void LoadLayoutAtStartUp()
    {
        LazyLayout.Trigger trigger = new LazyLayout.Trigger();
        trigger.rectTransformsInScene = canvasInScene.GetComponentsInChildren<RectTransform>(true);
        LazyLayout.LL_Layout ll;
        try
        {
            ll = LazyLayout.LL_Layout.GetLLAtStartup(canvasInScene.GetComponent<Canvas>(), savedLazyLayouts, LazyLayout_DPIHelper.GetCorrectScreenSize());
        }
        catch
        {
            Debug.LogError("--LAZYLAYOUT ERROR-- COULD NOT FIND ANY LAYOUT FOR DEVICE SIZE -" + LazyLayout_DPIHelper.GetCorrectScreenSize() + "- AND AN ASPECT RATIO OF -" + LazyLayout.LL_Layout.GetCurrentAR() + "- (" + Camera.main.aspect + ")");
            return;
        }

        if (ll != null)
        {
            trigger.ll = LazyLayout.LL_Layout.GetLLAtStartup(canvasInScene.GetComponent<Canvas>(), savedLazyLayouts, LazyLayout_DPIHelper.GetCorrectScreenSize());
            trigger.TriggerLayout(switchTextContent);
            Debug.Log("--LazyLayout-- Screen-size: " + LazyLayout_DPIHelper.GetCorrectScreenSize() + "- and aspect-ratio: " + LazyLayout.LL_Layout.GetCurrentAR());
        }
        else
        {
            Debug.LogError("--LAZYLAYOUT ERROR-- COULD NOT FIND ANY LAYOUT FOR DEVICE SIZE -" + LazyLayout_DPIHelper.GetCorrectScreenSize() + "- AND AN ASPECT RATIO OF -" + LazyLayout.LL_Layout.GetCurrentAR() + "- (" + Camera.main.aspect + ")");
        }

        savedScreenSize = new Vector2(Screen.width, Screen.height);
    }

    public void ApplySavedLayout(int _index)
    {
        LazyLayout.Trigger trigger = new LazyLayout.Trigger();
        trigger.rectTransformsInScene = canvasInScene.GetComponentsInChildren<RectTransform>(true);
        trigger.ll = LazyLayout.LL_Layout.GetLLAtStartup(canvasInScene.GetComponent<Canvas>(), savedLazyLayouts, LazyLayout_DPIHelper.GetCorrectScreenSize());

        trigger.TriggerLayout(switchTextContent);

        Debug.Log("--LazyLayout-- applying layout -" + savedLazyLayouts[_index].layoutName + "-");
    }

    public void ApplySavedLayout(string _layoutName)
    {
        LazyLayout.Trigger trigger = new LazyLayout.Trigger();
        trigger.rectTransformsInScene = canvasInScene.GetComponentsInChildren<RectTransform>(true);
        trigger.ll = LazyLayout.LL_Layout.GetLLAtStartup(canvasInScene.GetComponent<Canvas>(), savedLazyLayouts, LazyLayout_DPIHelper.GetCorrectScreenSize());

        trigger.TriggerLayout(switchTextContent);
    }

    public void ApplySavedLayoutAtRuntime()
    {
        LazyLayout.Trigger trigger = new LazyLayout.Trigger();
        trigger.rectTransformsInScene = canvasInScene.GetComponentsInChildren<RectTransform>(true);
        LazyLayout.LL_Layout ll;
        try
        {
            ll = LazyLayout.LL_Layout.GetLLAtStartup(canvasInScene.GetComponent<Canvas>(), savedLazyLayouts, LazyLayout_DPIHelper.GetCorrectScreenSize());
        }
        catch
        {
            Debug.LogError("--LAZYLAYOUT ERROR-- COULD NOT FIND ANY LAYOUT FOR DEVICE SIZE -" + LazyLayout_DPIHelper.GetCorrectScreenSize() + "- AND AN ASPECT RATIO OF -" + LazyLayout.LL_Layout.GetCurrentAR() + "- (" + Camera.main.aspect + ")");
            return;
        }
        

        if(ll == null)
        {
            Debug.LogError("--LAZYLAYOUT ERROR-- COULD NOT FIND ANY LAYOUT FOR DEVICE SIZE -" + LazyLayout_DPIHelper.GetCorrectScreenSize() + "- AND AN ASPECT RATIO OF -" + LazyLayout.LL_Layout.GetCurrentAR() + "- (" + Camera.main.aspect + ")");
            return;
        }

        trigger.ll = ll;
        trigger.TriggerLayout(switchTextContent);
    }

}
