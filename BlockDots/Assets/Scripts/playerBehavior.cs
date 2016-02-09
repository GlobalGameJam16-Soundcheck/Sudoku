using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class playerBehavior : MonoBehaviour {

	public int A_amt; //3
	public int B_amt; //2
	public int C_amt; //3
	public int star_amt; //1
	public int player;//am i 0 or 1?
	public Color playerColor;
	public string star_piece; //these are the tags
	public string a_piece;
	public string b_piece;
	public string c_piece;

	public int score { get; set; } //gameMaster might use this, not this class
	private bool holding;
	private GameObject heldPiece;
	private bool clickedStar;
	private LayerMask pieceLayer;
	private LayerMask cellLayer;
	public Dictionary<string, int> pieceDict { get; set; } //tags are keys, amts are values
	public GameObject[,] grid { get; set; }

	private int hoverCelli;
	private int hoverCellj;

	private bool firstTurn;
	public bool playedTurn { get; set; }

	// Use this for initialization
	void Start () {
		score = 0;
		holding = false;
		heldPiece = null;
		clickedStar = false;
		pieceLayer = 1 << LayerMask.NameToLayer ("piece");
		cellLayer = 1 << LayerMask.NameToLayer ("cell");
		pieceDict = new Dictionary<string,int> ();
		pieceDict.Add (star_piece, star_amt);
		pieceDict.Add (c_piece, C_amt);
		pieceDict.Add (b_piece, B_amt);
		pieceDict.Add (a_piece, A_amt);
		hoverCelli = -1;
		hoverCellj = -1;
		firstTurn = true;
		playedTurn = false;
	}

	void OnMouseUp(){
		if (clickedStar)
			clickedStar = false;
	}

	public bool canGo(){
		for (int i = 0; i < grid.GetLength (0); i++) {
			for (int j = 0; j < grid.GetLength (1); j++) {
				bool canUseStar = false;
				if (pieceDict [star_piece] > 0) {
					canUseStar = true;
				}
				if (grid [i, j].GetComponent<cellBehavior> ().canBePlayedOn (player, canUseStar, firstTurn)) {
					return true;
				}
			}
		}
		return false;
	}
	
	public void makeTurn () {
		//track mouse pos
		//check if click no hold on star, if star_amt > 0, clickedStar == !clickedStar
		//check if click and hold on piece layer
		//if hold on a pieceLayer check tag with amt left. if amt left > 0
		//instantiate copy game object as piece. holding is true
		//if holding, set piece transform.pos to 
		//start of a turn, clickStar is false

		//if no spots to go, playedTurn = true;
		if (!canGo()){
			Debug.Log ("cannot go but other player can");
			playedTurn = true;
		}

		if (Input.GetMouseButtonDown(0) && !holding){ //click
			Debug.Log ("clicked!");
			checkClick();
		}
		if (Input.GetMouseButton (0)) { //holding
			if (holding) { //drag to mouse
				dragPiece();
			}
		} else {
			release ();
		}
	}

	private void dragPiece(){
		//raycast over cells and change their color to green if good (along with other cells that'd be good?)
		//and light up cell dot with player color
		//keep track of last knows 
		Vector3 mousePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, heldPiece.transform.position.z);
		heldPiece.transform.position = Camera.main.ScreenToWorldPoint (mousePos);

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, Mathf.Infinity, cellLayer)) { //hovering over this cell
			cellBehavior cellScript = hit.collider.gameObject.GetComponent<cellBehavior> ();
			hoverCelli = cellScript.i;
			hoverCellj = cellScript.j;
			Debug.Log ("touching cell + " + cellScript.i + " " + cellScript.j);
		} else {
			hoverCelli = -1;
			hoverCellj = -1;
		}

		cellBehavior cellGridScript;
		for (int i = 0; i < grid.GetLength (0); i++) {
			for (int j = 0; j < grid.GetLength (1); j++) {
				cellGridScript = grid [i, j].GetComponent<cellBehavior> ();
				Color color = cellGridScript.origColor;
				bool useOrigDotColor = true;
				if (i == hoverCelli && j == hoverCellj) {
					if (cellGridScript.canBePlayedOn (player, clickedStar, firstTurn)) {
						color = Color.green;
						useOrigDotColor = false;
					} else {
						color = Color.red;
					}
				}
				cellGridScript.setColor (color);
				cellGridScript.hoverDot (player, playerColor, useOrigDotColor);

			}
		}

		//look at the held pieces iDir and jDir, call hoverDot on these cells as well
		if (onGrid (hoverCelli, hoverCellj)) {
			cellGridScript = grid [hoverCelli, hoverCellj].GetComponent<cellBehavior> ();
			if (cellGridScript.canBePlayedOn(player, clickedStar, firstTurn))
				updateOrHoverNeighborDots (true);
		}
	}

	private bool onGrid(int i, int j){
		return (i >= 0 && j >= 0 && i < grid.GetLength (0) && 
				j < grid.GetLength (1) && 
			(i != (grid.GetLength (0))/2 || j != (grid.GetLength (1))/2));
	}

	private void updateOrHoverNeighborDots(bool hover){
		//look at the held pieces iDir and jDir, call hoverDot on these cells as well
		cellBehavior cellGridScript;
		int count = 0;
		int iDirLen = heldPiece.GetComponent<pieceBehavior> ().iDirLen;
		int jDirLen = heldPiece.GetComponent<pieceBehavior> ().jDirLen;
		int newCelli;
		int newCellj;
		while ((count < Mathf.Abs (iDirLen) || count < Mathf.Abs (jDirLen)) && (hoverCelli >= 0 && hoverCellj >= 0)) {
			count++;
			newCelli = hoverCelli;
			newCellj = hoverCellj;
			if (!(iDirLen == 0))
				newCelli = hoverCelli + ((int)Mathf.Sign((float)iDirLen)) * count;
			if (!(jDirLen == 0))
				newCellj = hoverCellj + ((int)Mathf.Sign((float)jDirLen)) * count;
			if (onGrid (newCelli, newCellj)) {
				cellGridScript = grid [newCelli, newCellj].GetComponent<cellBehavior> ();
				if (hover)
					cellGridScript.hoverDot (player, playerColor, false);
				else
					cellGridScript.updateDots (player, playerColor);
			}

			newCelli = hoverCelli;
			newCellj = hoverCellj;
			if (!(iDirLen == 0))
				newCelli = hoverCelli - ((int)Mathf.Sign((float)iDirLen)) * count;
			if (!(jDirLen == 0))
				newCellj = hoverCellj - ((int)Mathf.Sign((float)jDirLen)) * count;
			if (onGrid (newCelli, newCellj)) {
				cellGridScript = grid [newCelli, newCellj].GetComponent<cellBehavior> ();
				if (hover)
					cellGridScript.hoverDot (player, playerColor, false);
				else
					cellGridScript.updateDots (player, playerColor);
			}
		}
	}

	private void release(){
		holding = false;
		if (heldPiece != null){
			//check position of release, if over a cell that's available, cell.MakePlacement
			//and update dots on affected cells then decrement amount of this type of piece
			Vector3 mousePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, heldPiece.transform.position.z);
			heldPiece.transform.position = Camera.main.ScreenToWorldPoint (mousePos);

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, Mathf.Infinity, cellLayer)) { //hovering over this cell
				cellBehavior cellScript = hit.collider.gameObject.GetComponent<cellBehavior> ();
				if (cellScript.canBePlayedOn (player, clickedStar, firstTurn)) {
					firstTurn = false;
					hoverCelli = cellScript.i;
					hoverCellj = cellScript.j;
					Material mat = heldPiece.GetComponent<pieceBehavior> ().mat;
					if (clickedStar)
						mat = heldPiece.GetComponent<pieceBehavior> ().matStar;
					cellScript.makePlacement (player, mat);
					cellScript.updateDots (player, playerColor);
					updateOrHoverNeighborDots (false);
					Debug.Log ("touching cell + " + cellScript.i + " " + cellScript.j);
					if (clickedStar) {
						pieceDict [star_piece]--;
						clickedStar = false;
					}
					pieceDict [heldPiece.tag]--;
					Debug.Log (heldPiece.tag + " has this much left! " + pieceDict [heldPiece.tag]);
					playedTurn = true;
				} else {
					Debug.Log ("cannot play here!");
				}
			}
			Destroy((Object)heldPiece.gameObject);
		}
		if (hoverCelli >= 0 && hoverCellj >= 0) {
			cellBehavior cellGridScript = grid [hoverCelli, hoverCellj].GetComponent<cellBehavior> ();
			cellGridScript.setColor (cellGridScript.origColor);
		}
		hoverCelli = -1;
		hoverCellj = -1;
		heldPiece = null;
	}

	private void checkClick(){
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, Mathf.Infinity, pieceLayer)) { 
			Debug.Log (hit.collider.transform.name + " was hit!");
			GameObject potential = hit.collider.gameObject;
			if (potential.GetComponent<pieceBehavior> ().player == player) {
				Debug.Log ("that's my piece! " + potential.name);
				if (potential.CompareTag (star_piece)) {
					Debug.Log ("star was clicked!");
					if (pieceDict [star_piece] > 0) {
						Debug.Log ("still can use a star!");
						clickedStar = !clickedStar; //toggle
						Debug.Log ("clickedStar " + clickedStar); //show that they clicked the star somehow
						return;
					} else {
						Debug.Log ("cannot use this star anymore");
					}
				} else {
					//clicked on a regular piece
					if (pieceDict[potential.gameObject.tag] > 0){
						//can use it
//						heldPiece = potential;
						Vector3 pos = new Vector3(potential.transform.position.x, potential.transform.position.y, 
																				  potential.transform.position.z - 1f);
						heldPiece = (GameObject)Instantiate((Object)potential, pos, potential.transform.rotation);
						Debug.Log ("now holding + " + heldPiece.name);
						holding = true;
					} else {
						Debug.Log ("cannot use this piece anymore");
					}
				}
			}
		}
	}
}
