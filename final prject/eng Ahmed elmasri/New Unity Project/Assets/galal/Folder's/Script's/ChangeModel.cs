using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeModel : MonoBehaviour {

	public GameObject Car1;
	public GameObject Car2;


	public int BallSelected;

	// Use this for initialization
	void Start () {
		
		Car2.SetActive (false);

		}

	public void LoadCar1(){
		Car1.SetActive (true);
		Car2.SetActive (false);

;
	}

	public void LoadCar2(){
		Car1.SetActive (false);
		Car2.SetActive (true);


	}

}
