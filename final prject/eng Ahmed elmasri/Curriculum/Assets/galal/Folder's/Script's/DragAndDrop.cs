using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour {
	private Animator anim;
	public Transform DistinoationPnit;
	public Transform[] WayPoints;
	public float DistanceToPoint;
	public Transform TGT;
	public float DistanceToTarget;
	public float AttackDistance;
	private bool Patrol = true;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		DistinoationPnit = WayPoints[Random.Range(0,WayPoints.Length)];
		
	}
	
	// Update is called once per frame 
	void Update () {
		//Give motion
		anim.SetBool ("Move", true);
		anim.SetBool ("Attack", false);


		//distance
		DistanceToPoint = Vector3.Distance(DistinoationPnit.position, transform.position);
		//FistanceToTarget
		DistanceToTarget =Vector3.Distance(TGT.position, transform.position);

		if (DistanceToPoint < 1 && Patrol) {
			DistinoationPnit = WayPoints [Random.Range (0, WayPoints.Length)];
		} else if (Patrol && DistinoationPnit == TGT) {
			DistinoationPnit = WayPoints [Random.Range (0, WayPoints.Length)];
		}
		//player detection
		if (DistanceToTarget < AttackDistance) {
			
			Patrol = false;
			DistinoationPnit = TGT;
			anim.SetBool ("Attack", true);
			anim.SetBool ("Move", false);


		} else {
			Patrol = true;
		}

		//Rotate enemy
		Vector3 DPP = DistinoationPnit.position -transform.position;
		DPP.y = 0;

	}
}
