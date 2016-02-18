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

	public GameObject[] textFields; //for score
	public GameObject[] inventoryList; //text field for pieces, from i = 0 to len/2 is p0, rest is p1

	public GameObject[] pieces; //pieces[0] = array of p0 pieces, pieces[1] = array of p1's pieces
	public GameObject endPiece;
	public float endPieceZ;
	public GameObject homeButton;

	public GameObject[] tutorials; //holds the info for each tutorial
	public GameObject[] tutorialTextFields; //tutorialTextFields[0] == p0 text field, tutorialTextFields[1] == p1 text field
	private bool tutorialMode;
	private bool tutModeFin;
	private int currTut;
	private bool needNewTut;
	private tutorialSteps tutStep;

	private bool currPlayerIsHovering; //if player is hovering, add score is displayed in white and previews what score would be
	private Color[] origScoreColor;

	public Texture[] gameOverTextures;

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
		origScoreColor = new Color[2];
		origScoreColor [0] = textFields [0].GetComponent<Text> ().color;
		origScoreColor [1] = textFields [1].GetComponent<Text> ().color;
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
//					Color winColor = endPiece.GetComponent<MeshRenderer> ().material.color;
					Texture texture = gameOverTextures [2]; //default for tie
					if (score [0] > score [1]) {
//						winColor = players [0].playerColor;
						texture = gameOverTextures[0];
						if (players[1].numPiecesLeft() > 0){
							texture = gameOverTextures [3]; //win by lockout bc player1 can still go
						}
					} else if (score [1] > score [0]) {
//						winColor = players [1].playerColor;
						texture = gameOverTextures[1];
						if (players[0].numPiecesLeft() > 0){
							texture = gameOverTextures [4]; //win by lockout bc player0 can still go
						}
					}
					endPiece.GetComponent<MeshRenderer> ().material.mainTexture = texture;
					Vector3 pos = endPiece.transform.position;
					endPiece.transform.position = new Vector3 (pos.x, pos.y, endPieceZ);
					homeButton.SetActive (true);
					players [0].gameOver = true;
					players [1].gameOver = true;
				}
			}
		} else {
			if (needNewTut || tutModeFin) {
				if (currTut >= tutorials.Length) {
					//done with tutorials
					tutStep.deActivateBground (0);
					tutStep.deActivateBground (1);
					tutorialTextFields[0].GetComponent<Text> ().text = "";
					tutorialTextFields[1].GetComponent<Text> ().text = "";
					tutModeFin = true;
					Debug.Log ("homer");
					if (!homeButton.activeInHierarchy)
						homeButton.SetActive (true);
					needNewTut = false;
				} else {
					tutStep = tutorials [currTut].GetComponent<tutorialSteps> ();
					tutorialTextFields[currPlayer].GetComponent<Text> ().text = tutStep.instruction;
					currTut++;
					for (int i = 0; i < tutStep.pieceTags.Length; i++) {
						players [0].pieceDict [tutStep.pieceTags [i]] = tutStep.p0PieceAmt [i];
						players [1].pieceDict [tutStep.pieceTags [i]] = tutStep.p1PieceAmt [i];
					}
					needNewTut = false;
				}
			} else {
				int otherPlayer = (currPlayer + 1) % 2;
				//use the tut that's in progress till it's finished
				if (!players [currPlayer].playedTurn) {
//					Debug.Log ("waiting for currPlayer to make turn");
					tutorialTextFields[otherPlayer].GetComponent<Text> ().text = "";
					tutStep.deActivateBground(otherPlayer);
					tutStep.activateBground (currPlayer);
					if (tutStep.saveGameBoard) {
						saveGridState ();
					}
					players [currPlayer].makeTurn ();
				} else {
					testSuggestedInstructions ();
					if (tutStep.useCompleted) {
						string completedText = tutStep.completed;
						if (tutStep.displayScoreInCompleted){
							int numBlocksInControl = 0;
							int currPlayerScore = score [currPlayer];
							for (int i = 0; i < grid.GetLength (0); i++) {
								for (int j = 0; j < grid.GetLength (1); j++) {
									cellBehavior cellScript = grid [i, j].GetComponent<cellBehavior> ();
									if (cellScript.dotCount [currPlayer] > cellScript.dotCount [otherPlayer]) {
										numBlocksInControl++;
									}
								}
							}
							completedText += " " + numBlocksInControl.ToString() + " " + tutStep.completed1;
						}
						tutorialTextFields [currPlayer].GetComponent<Text> ().text = completedText;
						if (Input.GetMouseButtonDown (1)) {
							players [currPlayer].playedTurn = false;
							tutStep.activateBground (otherPlayer);
							tutStep.activateBground (currPlayer);
							currPlayer = otherPlayer;
							needNewTut = true;
						}
					} else {
						tutorialTextFields[currPlayer].GetComponent<Text> ().text = tutStep.badCompleted;
						if (Input.GetMouseButtonDown (1)) {
							players [currPlayer].playedTurn = false;
							Debug.Log ("it was set to false");
//							Debug.Break ();
							tutorialTextFields[currPlayer].GetComponent<Text> ().text = tutStep.instruction;
							for (int i = 0; i < grid.GetLength (0); i++) {
								for (int j = 0; j < grid.GetLength (1); j++) {
									grid [i, j].GetComponent<cellBehavior> ().goBackToPrevState ();
								}
							}
							players [currPlayer].firstTurn = players [currPlayer].oldFirstTurn;
							tutStep.useCompleted = true;
							for (int i = 0; i < tutStep.pieceTags.Length; i++) {
								players [0].pieceDict [tutStep.pieceTags [i]] = tutStep.p0PieceAmt [i];
								players [1].pieceDict [tutStep.pieceTags [i]] = tutStep.p1PieceAmt [i];
							}
						}
					}
				}
				calculateScore ();
			}
		}
	}

	void saveGridState(){
		for (int i = 0; i < grid.GetLength (0); i++) {
			for (int j = 0; j < grid.GetLength (1); j++) {
				grid [i, j].GetComponent<cellBehavior> ().saveState ();
			}
		}
	}

	void testSuggestedInstructions(){
		//see if player did the 'suggested' placement of contesting a space
		if (tutStep.testContestSpace) {
			Debug.Log ("testing contestedSpace");
			bool contestedSpace = false;
			for (int i = 0; i < grid.GetLength (0); i++) {
				for (int j = 0; j < grid.GetLength (1); j++) {
					cellBehavior cellScript = grid [i, j].GetComponent<cellBehavior> ();
					if (cellScript.dotCount [0] > 0 && cellScript.dotCount [0] == cellScript.dotCount [1]) {
						contestedSpace = true;
						break;
					}
				}
			}
			if (!contestedSpace) {
				tutStep.useCompleted = false;
			}
		} else if (tutStep.testStarPiece) {
			Debug.Log ("testing starlet Piece");
			//see if player did the 'suggested' placement of starlet
			for (int i = 0; i < grid.GetLength (0); i++) {
				for (int j = 0; j < grid.GetLength (1); j++) {
					cellBehavior cellScript = grid [i, j].GetComponent<cellBehavior> ();
					if (cellScript.starOnHere) {
						if (cellScript.dotCount [currPlayer] > 1) { //played star on space that already has own barrel
							tutStep.useCompleted = false;
							break;
						}
					}
				}
			}
		} else if (tutStep.testBadSpotForUptown) {
			int numFreeSpaces = 0;
			for (int i = 0; i < grid.GetLength (0); i++) {
				for (int j = 0; j < grid.GetLength (1); j++) {
					cellBehavior cellScript = grid [i, j].GetComponent<cellBehavior> ();
					if (cellScript.dotCount [currPlayer] > 0 && !cellScript.isOccupied ()) {
						numFreeSpaces++;
					}
				}
			}
			if (numFreeSpaces < tutStep.neededAmountOfFreeSpots) {
				tutStep.useCompleted = false;
			}
		} else if (tutStep.testSetUpForStar) {
			bool otherPlayerCanPlayStarOnCurrCell = false;
			for (int i = 0; i < grid.GetLength (0); i++) {
				for (int j = 0; j < grid.GetLength (1); j++) {
					cellBehavior cellScript = grid [i, j].GetComponent<cellBehavior> ();
					if (cellScript.dotCount [currPlayer] > 0 && cellScript.dotCount [(currPlayer + 1) % 2] == 0
					    && !cellScript.isOccupied ()) {
						otherPlayerCanPlayStarOnCurrCell = true;
						break;
					}
				}
			}
			if (!otherPlayerCanPlayStarOnCurrCell) {
				tutStep.useCompleted = false;
			}
		} else if (tutStep.testPlaceOnContested) {
			bool playedOnContested = false;
			for (int i = 0; i < grid.GetLength (0); i++) {
				for (int j = 0; j < grid.GetLength (1); j++) {
					cellBehavior cellScript = grid [i, j].GetComponent<cellBehavior> ();
					if (cellScript.dotCount[currPlayer] > 0 && cellScript.dotCount [(currPlayer + 1) % 2] > 0 && 
						cellScript.dotCount[currPlayer] > cellScript.dotCount [(currPlayer + 1) % 2]){
						playedOnContested = true;
						break;
					}
				}
			}
			if (!playedOnContested) {
				tutStep.useCompleted = false;
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
		currPlayerIsHovering = false;
		for (int i = 0; i < grid.GetLength (0); i++) {
			for (int j = 0; j < grid.GetLength (1); j++) {
				cellBehavior cellScript = grid [i, j].GetComponent<cellBehavior> ();
				int p0count = cellScript.dotCount [0];
				int p1count = cellScript.dotCount [1];
				int defaultCount = cellScript.dotCount [2];
				if (cellScript.beingHoveredOn) {
					currPlayerIsHovering = true;
					if (currPlayer == 0) {
						p0count++;
					} else {
						p1count++;
					}
				}
				int total = p0count + p1count + defaultCount;
				MeshRenderer mr = cellScript.outline.GetComponent<MeshRenderer> ();
				Texture texture;
				if (p0count > p1count) {
					p0 += total;
//					color = players[0].outlineColor;
					texture = players [0].outlineNeonTexture;
					cellScript.setOutlineEmissiveColor (false);
				} else if (p1count > p0count) {
					p1 += total;
//					color = players[1].outlineColor;
					texture = players [1].outlineNeonTexture;
					cellScript.setOutlineEmissiveColor (false);
				} else {
					if (p0count == 0 && p1count == 0) {
//						color = cellScript.outlineOrigColor;
						texture = players[0].outlineOrigNeonTexture;
						cellScript.setOutlineEmissiveColor (true);
					} else {
//						color = cellScript.tiedColor;
						texture = players[0].outlineTieNeonTexture;
						cellScript.setOutlineEmissiveColor (false);
					}
				}
				cellScript.changeOutlineColor (texture);
			}
		}
		score [0] = p0;
		score [1] = p1;
	}

	private bool checkGameOver(){
		bool atLeastOneCanGo = false;
//		foreach (playerBehavior playerScript in players) {
//			atLeastOneCanGo = atLeastOneCanGo || playerScript.canGo (true);
//		}
//		return !atLeastOneCanGo;
		//
		// if player cannot go 
		//	if this player is losing, game over
		//  else game is not over?
		//
		//if a player can no longer move, game is over
		for (int i = 0; i < players.Length; i++) {
			playerBehavior playerScript = players [i];
			bool playerCanGo = playerScript.canGo (true);
			atLeastOneCanGo = atLeastOneCanGo || playerCanGo;
			if (!playerCanGo) {
				if (score [i] < score [(i + 1) % 2] && !(Input.GetMouseButton(0))) {
					return true; //this player stupidly locked himself out while losing so game over
				}
			}
		}
		return !atLeastOneCanGo;
	}

	void OnGUI(){
		string txt0 = score [0].ToString();
		string txt1 = score [1].ToString();
		textFields[0].GetComponent<Text>().text = txt0;
		textFields[1].GetComponent<Text>().text = txt1;
		if ((currPlayerIsHovering || (!players[currPlayer].playedTurn && Input.GetMouseButton(0))) && !homeButton.activeInHierarchy) {
			textFields [0].GetComponent<Text> ().color = Color.gray;
			textFields [1].GetComponent<Text> ().color = Color.gray;
		} else {
			textFields [0].GetComponent<Text> ().color = origScoreColor[0];
			textFields [1].GetComponent<Text> ().color = origScoreColor[1];
		}
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
			} 
//			else {
//				if (tutorialMode)
//					inventoryList [i].GetComponent<Text> ().color = players[player].inventoryColor;
//			}
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
