using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
 
public class RestartScript : MonoBehaviour {

	public void restartgame(int RestartTheGame)
	{
	    SceneManager.LoadScene (RestartTheGame);
	}
}
