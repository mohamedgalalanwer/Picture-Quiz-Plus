using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LetterField : MonoBehaviour,  IDragHandler, IEndDragHandler //Component class that attached to the each letter field
{
    public Text text;

    //States of the field
    public bool isEmpty = true;
    public bool isLocked = false;
    public bool isLast = false;
    public bool isDragged = false;

    public Letter letterReference; //Stores the letter reference when it is clicked

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.LogWarning(eventData.pressPosition - eventData.position);
        if (DataManager.Instance.RemoveClearButtons && !isDragged && Mathf.Abs(eventData.pressPosition.x - eventData.position.x) > 200)
        {
            GameObject.FindObjectOfType<UIManager>().ClearAll();
            isDragged = true;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragged = false;
    }
}
