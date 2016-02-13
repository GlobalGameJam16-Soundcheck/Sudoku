using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class masterBehavior : MonoBehaviour {

	//p0 is uptown - blue, p1 is empire - red

	public GameObject[] cellPrefabs; //0 is 0 points, 1 is 1 pt, 2 is 2 pts, 3 is pts, 4 is black mid square
	public playerBehavior[] players;
	private int[] score;

	private GameObject[,] grid;
	private int n;
	private int currPlayer;
	public float zDist;

	public GameObject[] textFields;
	public GameObject[] inventoryList; //text field for pieces, from i = 0 to len/2 is p0, rest is p1

	public GameObject[] pieces; //pieces[0] = array of p0 pieces, pieces[1] = array of p1's pieces
	public GameObject endPiece;
	public float endPieceZ;
	public GameObject homeButton;

	public GameObject[] tutorials; //holds the info for each tutorial
	public GameObject tutorialTextField;
	private bool tutorialMode;
	private bool tutModeFin;
	private int currTut;
	private bool needNewTut;
	private tutorialSteps tutStep;

	// Use this for initialization
	void Start () {
		n = 5; //n x n grid
		grid = new GameObject[n,n];
		for (int i = 0; i < n; i++){
			for (int j = 0; j < n; j++) {
				int whichPrefab = 0; //boring spots
				int diff = Mathf.Abs (i - j);
				if ((diff == 0 || diff == n - 1) && (i == 0 || i == n - 1)) { //corners
					whichPrefab = 3;
				} else if (diff == 1 && (i == n/2 || j == n/2)){ //plus sign not center
					whichPrefab = 1;
				} else if (((diff == 1 || diff == n - 2) && (i <= 1 || i == n - 1 || i == n - 2)) ||
							(diff == 1 || diff == n - 2) && (j <= 1 || j == n - 1 || j == n - 2)){//surrounding corners
					whichPrefab = 2;
				} else if (i == n/2 && j == n/2){//mid
					whichPrefab = 4;
				}
				Vector3 pos = new Vector3 (transform.position.x + 1.1f*j*transform.localScale.x - 0.5f, 
										   transform.position.y - 1.1f*i*transform.localScale.y, zDist);
//				Quaternion rot = Quaternion.AngleAxis (180f, transform.up);
				grid[i,j] = (GameObject)Instantiate(cellPrefabs[whichPrefab], pos, transform.rotation);
				cellBehavior cellScript = grid [i, j].GetComponent<cellBehavior> ();
				cellScript.Init ();
				cellScript.i = i;
				cellScript.j = j;
			}
		}
		currPlayer = 0;
		foreach (playerBehavior playerScript in players) {
			playerScript.grid = grid;
		}
		score = new int[players.Length];
		for (int i = 0; i < score.Length; i++) {
			score [i] = 0;
		}
		tutorialMode = tutorials.Length > 0;
		currTut = 0;
		tutModeFin = false;
		needNewTut = true;
	}
	
	// Update is called once per frame
	void Update () {
		updatePiecesColor ();
		if (!tutorialMode) {
			if (!tutModeFin) {
				players [currPlayer].makeTurn ();
				if (players [currPlayer].playedTurn) {
					players [currPlayer].playedTurn = false;
					currPlayer = (currPlayer + 1) % 2;
				}
				calculateScore ();
				if (checkGameOver ()) {
					Color winColor = endPiece.GetComponent<MeshRenderer> ().material.color;
					if (score [0] > score [1]) {
						winColor = players [0].playerColor;
					} else if (score [1] > score [0]) {
						winColor = players [1].playerColor;
					}
					endPiece.GetComponent<MeshRenderer> ().material.color = winColor;
					Vector3 pos = endPiece.transform.position;
					endPiece.transform.position = new Vector3 (pos.x, pos.y, endPieceZ);
					homeButton.SetActive (true);
					players [0].gameOver = true;
					players [1].gameOver = true;
				}
			}
		} else {
			if (needNewTut) {
				if (currTut >= tutorials.Length) {
					//tuts are over
					tutorialMode = false;
					tutModeFin = true;
					homeButton.SetActive (true);
					needNewTut = false;
				} else {
					tutStep = tutorials [currTut].GetComponent<tutorialSteps> ();
					tutorialTextField.GetComponent<Text> ().text = tutStep.instruction;
					currTut++;
					for (int i = 0; i < tutStep.pieceTags.Length; i++) {
						//set pieces up
						players [0].pieceDict [tutStep.pieceTags [i]] = tutStep.p0PieceAmt [i];
						players [1].pieceDict [tutStep.pieceTags [i]] = tutStep.p1PieceAmt [i];
					}
					needNewTut = false;
				}
			} else {
				//use the tut that's in progress till it's finished
				if (Input.GetMouseButton (0)) {
					tutorialTextField.GetComponent<Text> ().text = "";
					tutStep.deActivateBground ();
				} else {
					tutorialTextField.GetComponent<Text> ().text = tutStep.instruction;
					tutStep.activateBground ();
				}
				players[currPlayer].makeTurn();
				if (players [currPlayer].playedTurn) {
					tutorialTextField.GetComponent<Text> ().text = tutStep.completed;
					if (Input.GetMouseButtonDown (0) || Input.GetMouseButtonDown (1)) {
						players [currPlayer].playedTurn = false;
						currPlayer = (currPlayer + 1) % 2;
						needNewTut = true;
						tutStep.activateBground ();
					}
				}
				calculateScore ();
			}
		}
	}

	void updatePiecesColor(){
		bool setAllToGray;
		for (int i = 0; i < pieces.Length; i++) {
			if (!players [i].canGo (true)) {
				setAllToGray = true;
			} else {
				setAllToGray = false;
			}
			bool myTurn = (i == currPlayer);
			foreach (Transform piece in pieces[i].transform) {
				bool nonStarPieceTurnedGray = false;
				if (!piece.gameObject.CompareTag (players [currPlayer].star_piece_movable) && 
					!players [currPlayer].canGo (false) && myTurn) {
					piece.GetComponent<SpriteRenderer> ().color = Color.gray;
					nonStarPieceTurnedGray = true;
				}
				if (setAllToGray || !myTurn || players [i].pieceDict [piece.tag] <= 0){
					piece.GetComponent<SpriteRenderer> ().color = Color.gray;
				} else if (!nonStarPieceTurnedGray) {
					piece.GetComponent<SpriteRenderer> ().color = piece.GetComponent<pieceBehavior> ().origColor;
				}
			}
		}
	}

	private void calculateScore(){
		int p0 = 0;
		int p1 = 0;
		for (int i = 0; i < grid.GetLength (0); i++) {
			for (int j = 0; j < grid.GetLength (1); j++) {
				cellBehavior cellScript = grid [i, j].GetComponent<cellBehavior> ();
				int p0count = cellScript.dotCount [0];
				int p1count = cellScript.dotCount [1];
				int defaultCount = cellScript.dotCount [2];
				int total = p0count + p1count + defaultCount;
				MeshRenderer mr = cellScript.outline.GetComponent<MeshRenderer> ();
				Color color;
				if (p0count > p1count) {
					p0 += total;
//					color = new Color(players [0].playerColor.r, players [0].playerColor.g, players [0].playerColor.b, players [0].playerColor.a/2);
					color = players[0].outlineColor;
					mr.material.color = color;
				} else if (p1count > p0count) {
					p1 += total;
//					color = new Color(players [1].playerColor.r, players [1].playerColor.g, players [1].playerColor.b, players [1].playerColor.a/2);
					color = players[1].outlineColor;
					mr.material.color = color;
				} else {
					if (p0count == 0 && p1count == 0)
						mr.material.color = cellScript.outlineOrigColor;
					else
						mr.material.color = cellScript.tiedColor;
				}
			}
		}
		score [0] = p0;
		score [1] = p1;
	}

	private bool checkGameOver(){
		bool atLeastOneCanGo = false;
		foreach (playerBehavior playerScript in players) {
			atLeastOneCanGo = atLeastOneCanGo || playerScript.canGo (true);
		}
		return !atLeastOneCanGo;
	}

	void OnGUI(){
		//textField.transform.position = new Vector3 (Screen.width/2f, Screen.height - Screen.height/10f, textField.transform.position.z);
		string txt0 = "UPTOWN: " + score [0];
		string txt1 = "EMPIRE: " + score [1];
		textFields[0].GetComponent<Text>().text = txt0;
		textFields[1].GetComponent<Text>().text = txt1;
		//update inventory
		for (int i = 0; i < inventoryList.Length; i++) { //[p0_A, p0_B, p0_c, p0_star, p1_A, p1_B, p1_C, p1_star]
			int player = 0;
			int j = i;
			if (i >= inventoryList.Length / 2) {
				player = 1;
				j -= inventoryList.Length / 2;
			}
			int amtLeft = players [player].pieceDict [convertToTag (j)];
			inventoryList [i].GetComponent<Text> ().text = amtLeft.ToString ();
			if (amtLeft <= 0) {
				inventoryList [i].GetComponent<Text> ().color = Color.gray;
			} else {
				if (tutorialMode)
					inventoryList [i].GetComponent<Text> ().color = players[player].inventoryColor;
			}
		}
	}

	string convertToTag(int index){
		if (index == 0) {
			return players [0].a_piece;
		} else if (index == 1) {
			return players [0].b_piece;
		} else if (index == 2) {
			return players [0].c_piece;
		} else if (index == 3) {
			return players [0].star_piece_movable;
		}
		return "";
	}
}
