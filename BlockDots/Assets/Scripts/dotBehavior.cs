using UnityEngine;
using System.Collections;

public class dotBehavior : MonoBehaviour {

	public bool coloredIn { get; set; }
	public Color origColor { get; set; } 

	public bool isHovering { get; set; }

	private bool oldColoredIn;
	private Color oldColor;

	void Start(){
		coloredIn = false;
		origColor = GetComponent<SpriteRenderer> ().material.color;

		oldColoredIn = false;
		oldColor = origColor;

		isHovering = false;
	}

	public void setDotColor(Color color){
		GetComponent<SpriteRenderer> ().material.color = color;
	}

	public void saveState(){
		if (!isHovering) {
			oldColoredIn = coloredIn;
			oldColor = GetComponent<SpriteRenderer> ().material.color;
		}
	}

	public void goBackToPrevState(){
		coloredIn = oldColoredIn;
		GetComponent<SpriteRenderer> ().material.color = oldColor;
	}

	public void setHover(bool hover){
		isHovering = hover;
	}
}
