using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class startScreen : MonoBehaviour {

	public void PressStart(){
		SceneManager.LoadScene ("blockDots");
	}
}
