using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour {
	public AudioClip[] Clip;

	AudioSource AS;
	AudioClip ClipToplay = null;

	// Use this for initialization
	void Start () {
		AS = GetComponent<AudioSource> ();
	}

	void Update () {
		//reser clip when finish
		if(!AS.isPlaying) {
			ClipToplay = null;
		}
	}
	public void Button(int index){
		if (ClipToplay != Clip[index]) {
			ClipToplay = Clip[index];
			PlaySound ();
		}
	}
	void PlaySound(){
		AS.Stop ();
		AS.PlayOneShot (ClipToplay);
	}
}