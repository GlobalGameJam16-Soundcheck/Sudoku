using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class masterBehavior : MonoBehaviour {

	public GameObject[] cellPrefabs; //0 is 0 points, 1 is 1 pt, 2 is 2 pts, 3 is pts, 4 is black mid square
	public playerBehavior[] players;
	private int[] score;

	private GameObject[,] grid;
	private int n;
	private int currPlayer;
	public float zDist;

	public GameObject textField;

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
				Vector3 pos = new Vector3 (transform.position.x + 1.1f*j*transform.localScale.x - 1.5f, 
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
	}
	
	// Update is called once per frame
	void Update () {
		players [currPlayer].makeTurn ();
		if (players[currPlayer].playedTurn){
			players [currPlayer].playedTurn = false;
			currPlayer = (currPlayer + 1) % 2;
		}
		calculateScore ();
		if (checkGameOver ()) {
			Debug.Log ("game is over!");
			Debug.Break ();
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
			atLeastOneCanGo = atLeastOneCanGo || playerScript.canGo ();
		}
		return !atLeastOneCanGo;
	}

	void OnGUI(){
		textField.transform.position = new Vector3 (Screen.width/2f, Screen.height - Screen.height/10f, textField.transform.position.z);
		string txt = "Player 1: " + score [0] + " Player 2: " + score [1];
		textField.GetComponent<Text>().text = txt;
//		GUI.Box (new Rect ((Screen.width)/2 -(Screen.width)/8,(Screen.height)/2-(Screen.height)/8,(Screen.width)/4,(Screen.height)/4), txt);
		//pls someone figure out how to programmitacally center this so it also works with resizing screens
	}
}
