
using UnityEngine;
using System.Collections;

public class CarSelector : MonoBehaviour {

	public GameObject Car1;
	public GameObject Car2;
	public GameObject Car3;
	public GameObject Car4;
	public GameObject Car5;
	public GameObject Car6;
	public GameObject Car7;
	public GameObject Car8;
	public GameObject Car9;
	public GameObject Car10;
	public int BallSelected;

	// Use this for initialization
	void Start () {
		Car1.SetActive (true);
		Car2.SetActive (false);
		Car3.SetActive (false);
		Car4.SetActive (false);
		Car5.SetActive (false);
		Car6.SetActive (false);
		Car7.SetActive (false);
		Car8.SetActive (false);
		Car9.SetActive (false);
		Car10.SetActive (false);}

	public void LoadCar1(){
		Car1.SetActive (true);
		Car2.SetActive (false);
		Car3.SetActive (false);
		Car4.SetActive (false);
		Car5.SetActive (false);
		Car6.SetActive (false);
		Car7.SetActive (false);
		Car8.SetActive (false);
		Car9.SetActive (false);
	}

	public void LoadCar2(){
		Car1.SetActive (true);
		Car2.SetActive (true);
		Car3.SetActive (false);
		Car4.SetActive (false);
		Car5.SetActive (false);
		Car6.SetActive (false);
		Car7.SetActive (false);
		Car8.SetActive (false);
		Car9.SetActive (false);
	}

	public void LoadCar3(){
		Car1.SetActive (true);
		Car2.SetActive (false);
		Car3.SetActive (true);
		Car4.SetActive (false);
		Car5.SetActive (false);
		Car6.SetActive (false);
		Car7.SetActive (false);
		Car8.SetActive (false);
		Car9.SetActive (false);
	}
	public void LoadCar4(){
		
		Car2.SetActive (false);
		Car3.SetActive (false);
		Car4.SetActive (true);
		Car5.SetActive (false);
		Car6.SetActive (false);
		Car7.SetActive (false);
		Car8.SetActive (false);
		Car9.SetActive (false);
	}
	public void LoadCar5(){
		
		Car2.SetActive (false);
		Car3.SetActive (false);
		Car4.SetActive (true);
		Car5.SetActive (true);
		Car6.SetActive (false);
		Car7.SetActive (false);
		Car8.SetActive (false);
		Car9.SetActive (false);
	}
	public void LoadCar6(){
		
		Car2.SetActive (false);
		Car3.SetActive (false);
		Car4.SetActive (true);
		Car5.SetActive (true);
		Car6.SetActive (true);
		Car7.SetActive (false);
		Car8.SetActive (false);
		Car9.SetActive (false);
	}
	public void LoadCar7(){
		Car1.SetActive (false);
		Car2.SetActive (false);
		Car3.SetActive (false);
		Car4.SetActive (false);
		Car5.SetActive (false);
		Car6.SetActive (false);
		Car7.SetActive (true);
		Car8.SetActive (false);
		Car9.SetActive (false);
	}
	public void LoadCar8(){
		
		Car2.SetActive (false);
		Car3.SetActive (false);
		Car4.SetActive (false);
		Car5.SetActive (false);
		Car6.SetActive (false);
		Car7.SetActive (false);
		Car8.SetActive (true);
		Car9.SetActive (false);
	}
	public void LoadCar9(){
		
		Car2.SetActive (false);
		Car3.SetActive (false);
		Car4.SetActive (false);
		Car5.SetActive (false);
		Car6.SetActive (false);
		Car7.SetActive (false);
		Car8.SetActive (false);
		Car9.SetActive (true);
		Car10.SetActive (true);
	}
	public void LoadCar10(){
		
		Car2.SetActive (false);
		Car3.SetActive (false);
		Car4.SetActive (false);
		Car5.SetActive (false);
		Car6.SetActive (false);
		Car7.SetActive (false);
		Car8.SetActive (false);
		Car9.SetActive (true);
		Car10.SetActive (true);
	}
}