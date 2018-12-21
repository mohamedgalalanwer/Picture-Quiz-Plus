using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDropObject : MonoBehaviour {

	Vector3 initialposition;
	// Use this for initialization
	void Start () {
		initialposition = gameObject.transform.position;//we save the initial position the object
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void Drag()
	{
		//stare the position of the mouse and moves the object Image accordingly
		GameObject.Find ("Image").transform.position = Input.mousePosition;
		print ("We Are Dragging The Mouse");
	}
	public void Drop()
	{
		for (int i = 1; i <= 3; i++)//we loop through the three placehelders 
		{
			GameObject phl = GameObject.Find ("placeHolderl"+i);//placehelders 1-2-3
			float distance = Vector3.Distance (GameObject.Find ("Image").transform.position, phl.transform.position);
			if (distance < 50) {
				if (phl.tag == "match") {
					GameObject.Find ("Image").transform.position = phl.transform.position;
					print ("MATCH!!");
				} else 
				{
					GameObject.Find ("Image").transform.position = initialposition;
					print ("Sorry, not a  MATCH!!");
				}
			}else
			{
				GameObject.Find ("Image").transform.position = initialposition;
			}
		}
	}
}
