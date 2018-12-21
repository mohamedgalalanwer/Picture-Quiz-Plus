using UnityEngine;
using System.Collections;

public class DragObject01 : MonoBehaviour {

    public Camera c;
    Vector3 screenPosition, offset;

    void OnMouseDown(){
        //Convert world position to screen position.
        screenPosition = c.WorldToScreenPoint(transform.position);
        offset = transform.position - c.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z));
    }

    void OnMouseDrag(){
        //track mouse position.
        Vector3 currentScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z);

        //convert screen position to world position with offset changes.
        Vector3 currentPosition = c.ScreenToWorldPoint(currentScreenSpace) + offset;

        Vector3 moveIn2D = new Vector3(currentPosition.x, transform.position.y, currentPosition.z);
        //It will update target gameobject's current postion.
        transform.position = moveIn2D;
    }
}
