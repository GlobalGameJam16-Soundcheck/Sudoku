using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class homeButton : MonoBehaviour {

	public void pressedHome(){
		Debug.Log ("pressed home");
		SceneManager.LoadScene ("startScreenWithTut");
	}

}
