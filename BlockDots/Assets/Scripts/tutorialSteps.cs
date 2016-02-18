using UnityEngine;
using System.Collections;

public class tutorialSteps : MonoBehaviour {

	public string[] pieceTags;
	public int[] p0PieceAmt;
	public int[] p1PieceAmt; //A_amt, B_amt, C_amt, Star_amt;
	public string instruction;
	public string completed;
	public string badCompleted;
	public bool useCompleted { get; set; }
	public int timer; //count down for the amount of seconds to allow the players to play
	public GameObject[] tutTxtBackgrounds; //0 for p0, 1 for p1
	private bool[] deactivated;
	public bool saveGameBoard;
	public bool testContestSpace;
	public bool testStarPiece;

	void Start(){
		deactivated = new bool[2];
		deactivated [0] = false;
		deactivated [1] = false;
		useCompleted = true;
	}

	public void deActivateBground(int player){
		if (!deactivated[player]) {
			Vector3 pos = tutTxtBackgrounds[player].transform.position;
			tutTxtBackgrounds[player].transform.position = new Vector3 (pos.x, pos.y, pos.z + 100f);
			deactivated[player] = true;
		}

	}

	public void activateBground(int player){
		if (deactivated[player]) {
			Vector3 pos = tutTxtBackgrounds[player].transform.position;
			tutTxtBackgrounds[player].transform.position = new Vector3 (pos.x, pos.y, pos.z - 100f);
			deactivated[player] = false;
		}
	}

}
