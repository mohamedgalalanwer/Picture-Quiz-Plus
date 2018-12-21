using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizedText : MonoBehaviour //Component class that mast be attached to every game element that should be localized
{
    public ElementType key;
    public bool isToUpperNeeded;

    private void Start()
    {
        Text text = GetComponent<Text>();
        text.text = isToUpperNeeded ?
        DataManager.Instance.GetLocalizedValue(key).ToUpper() : DataManager.Instance.GetLocalizedValue(key);
    }
}