using UnityEngine;
using System.Collections;

public class playerBehavior : MonoBehaviour {

	public int A_amt; //3
	public int B_amt; //2
	public int C_amt; //3
	public int star_amt; //1

	public int score { get; set; } //gameMaster might use this, not this class
	private bool holding;
	private GameObject piece;
	private bool clickedStar;
	private LayerMask pieceLayer;
	private LayerMask cellLayer;

	// Use this for initialization
	void Start () {
		score = 0;
		holding = false;
		piece = null;
		clickedStar = false;
		pieceLayer = 1 << LayerMask.NameToLayer ("piece");
		cellLayer = 1 << LayerMask.NameToLayer ("cell");
	}
	
	void makeTurn () {
		//track mouse pos
		//check if click no hold on star, if star_amt > 0, clickedStar == !clickedStar
		//check if click and hold on piece layer
		//if hold on a pieceLayer check tag with amt left. if amt left > 0
		//instantiate copy game object as piece. holding is true
		//if holding, set piece transform.pos to 
	}
}
