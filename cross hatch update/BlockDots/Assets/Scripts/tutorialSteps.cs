using UnityEngine;
using System.Collections;

public class tutorialSteps : MonoBehaviour {

	public string[] pieceTags;
	public int[] p0PieceAmt;
	public int[] p1PieceAmt; //A_amt, B_amt, C_amt, Star_amt;
	public string instruction;
	public string completed;
	public int timer; //count down for the amount of seconds to allow the players to play
	public GameObject tutTxtBackground;
	private bool deactivated;

	void Start(){
		deactivated = false;
	}

	public void deActivateBground(){
		if (!deactivated) {
			Vector3 pos = tutTxtBackground.transform.position;
			tutTxtBackground.transform.position = new Vector3 (pos.x, pos.y, pos.z + 100f);
			deactivated = true;
		}

	}

	public void activateBground(){
		if (deactivated) {
			Vector3 pos = tutTxtBackground.transform.position;
			tutTxtBackground.transform.position = new Vector3 (pos.x, pos.y, pos.z - 100f);
			deactivated = false;
		}
	}

}
