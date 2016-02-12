using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class startScreen : MonoBehaviour {

	public bool howToWasClicked { get; set; }
	public GameObject startMaster;

	void Start(){
		howToWasClicked = false;
	}

	public void PressStart(){
		SceneManager.LoadScene ("blockDots");
	}

	public void PressHowTo(){
		Debug.Log ("how to was pressed");
		howToWasClicked = true;
		startMaster.GetComponent<startController> ().clickCount = 0;
	}

}
