using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AutoAi : MonoBehaviour {

	private NavMeshAgent agent;
	public Transform Target;
	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent> ();
		//agent.destination = Target.position;
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit Hit;
		if (Input.GetMouseButton (0)) {
			Ray thisray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if(Physics.Raycast(thisray,out Hit))
				{
				agent.SetDestination (Hit.point);

			}
		}
	}
}
