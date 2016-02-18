using UnityEngine;
using System.Collections;

public class dotBehavior : MonoBehaviour {

	public bool coloredIn { get; set; }
	public Color origColor { get; set; } 

	private bool oldColoredIn;
	private Color oldColor;

	void Start(){
		coloredIn = false;
		origColor = GetComponent<SpriteRenderer> ().material.color;

		oldColoredIn = false;
		oldColor = origColor;
	}

	public void setDotColor(Color color){
		GetComponent<SpriteRenderer> ().material.color = color;
	}

	public void saveState(){
		oldColoredIn = coloredIn;
		oldColor = GetComponent<SpriteRenderer> ().material.color;
	}

	public void goBackToPrevState(){
		coloredIn = oldColoredIn;
		GetComponent<SpriteRenderer> ().material.color = oldColor;
	}
}
