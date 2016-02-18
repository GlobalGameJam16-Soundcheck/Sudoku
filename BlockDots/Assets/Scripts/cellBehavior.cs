using UnityEngine;
using System.Collections;

public class cellBehavior : MonoBehaviour {

	//two players, indexed by 0 and 1

	public int occupiedPlayer { get; set; } //0 or 1 for player, -1 for free
	public int i;
	public int j;
	public GameObject[] player0dots; //8 dots total per player
	public GameObject[] player1dots; 
	public int[] dotCount { get; set; } //dotCount[0] == player 0's number of dots on this cell
										//dotCount[1] == player 1's number of dots on this cell
										//dotCount[2] == cell's default number of dots
	private GameObject[][] playerDots;
	public int dotCap { get; set; } //player x cant place anymore dots here if dotcap is reached for this player
	public int defaultAmt;
	public Color origColor { get; set; }
	public GameObject outline;
	public Color outlineOrigColor { get; set; }
	public Color tiedColor;
	public bool starOnHere { get; set; }

	private int oldOccupiedPlayer;
	private int[] oldDotCount;
	private Color oldOrigColor;
	private Color oldOutlineColor;
	private bool oldStarOnHere;
	private Material oldMat;

	// Use this for initialization
	public void Init () {
		occupiedPlayer = -1;
		dotCount = new int[3];
		dotCount [0] = 0;
		dotCount [1] = 0;
		dotCount [2] = defaultAmt;
		dotCap = 8;
		playerDots = new GameObject[2][];
		playerDots [0] = player0dots; //should alias these references to arrays
		playerDots [1] = player1dots;
		origColor = GetComponent<MeshRenderer> ().material.color;
		outlineOrigColor = outline.GetComponent<MeshRenderer> ().material.color;
		starOnHere = false;

		oldOccupiedPlayer = occupiedPlayer;
		oldDotCount = new int[3];
		oldDotCount [0] = 0;
		oldDotCount [1] = 1;
		oldDotCount [2] = defaultAmt;
		oldStarOnHere = false;
		oldMat = transform.GetComponent<MeshRenderer> ().material;
	}

	public bool canBePlayedOn(int player, bool star, bool firstTurn){
		if (occupiedPlayer >= 0 || dotCount [player] >= dotCap) {
			return false;
		}
		if (dotCount [player] > 0) {
			//player has a dot here so he can play
			return true;
		} else if (firstTurn) {
			return true;
//		} else if (star && dotCount [(player + 1) % 2] > 0 && dotCount [(player + 1) % 2] < dotCap) {
			//used a star so can play on unoccupied cell with at least one dot of other player but less than cap
		} else if (star) {
			//used like a first turn
			return true;
		} else {
			return false;
		}
		
	}

	public void hoverDot(int player, Color color, bool orig){
		if (dotCount [player] < dotCap && playerDots[player].Length > 0) {
			GameObject dot = playerDots[player][dotCount[player]];
			dotBehavior dotScript = dot.GetComponent<dotBehavior> ();
			dotScript.setHover (true);
			if (!dotScript.coloredIn) {
				if (!orig) {
					dotScript.setDotColor (color);
				} else {
					dotScript.setDotColor (dotScript.origColor);
				}
			}
		}
	}

	public void updateDots(int player, Color color){
		if (dotCount [player] < dotCap) {
			GameObject dot = playerDots[player][dotCount[player]];
			dotBehavior dotScript = dot.GetComponent<dotBehavior> ();
			dotScript.setHover (false);
			if (!dotScript.coloredIn) {
				dotScript.setDotColor (color);
				dotScript.coloredIn = true;
				dotCount[player]++;
			}
		}
	}
		
	public void makePlacement(int player, Material mat, bool isStar){
		if (occupiedPlayer < 0) {
			occupiedPlayer = player;
			transform.GetComponent<MeshRenderer> ().material = mat;
		}
		origColor = Color.white;
		starOnHere = isStar;
	}

	public void setColor(Color color){
		GetComponent<MeshRenderer> ().material.color = color;
	}

	public bool isOccupied(){
		return (occupiedPlayer >= 0);
	}

	public void saveState(){
		oldOccupiedPlayer = occupiedPlayer;
		oldDotCount [0] = dotCount [0];
		oldDotCount [1] = dotCount [1];
		oldOrigColor = origColor;
		oldOutlineColor = outline.GetComponent<MeshRenderer> ().material.color;
		oldStarOnHere = starOnHere;
		oldMat = transform.GetComponent<MeshRenderer> ().material;
		for (int k = 0; k < playerDots.GetLength (0); k++) {
			foreach (GameObject dot in playerDots[k]) {
				dot.GetComponent<dotBehavior> ().saveState ();
			}
		}
	}

	public void goBackToPrevState(){
		occupiedPlayer = oldOccupiedPlayer;
		dotCount [0] = oldDotCount [0];
		dotCount [1] = oldDotCount [1];
		origColor = oldOrigColor;
		outline.GetComponent<MeshRenderer> ().material.color = oldOutlineColor;
		starOnHere = oldStarOnHere;
		transform.GetComponent<MeshRenderer> ().material = oldMat;
		for (int k = 0; k < playerDots.GetLength (0); k++) {
			foreach (GameObject dot in playerDots[k]) {
				dot.GetComponent<dotBehavior> ().goBackToPrevState();
			}
		}
	}
}
