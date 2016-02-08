using UnityEngine;
using System.Collections;

public class dotBehavior : MonoBehaviour {

	public bool coloredIn { get; set; }
	public Color origColor { get; set; } 

	void Start(){
		coloredIn = false;
		origColor = GetComponent<MeshRenderer> ().material.color;
	}
}
