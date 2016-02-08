using UnityEngine;
using System.Collections;

public class cellBehavior : MonoBehaviour {

	//two players, indexed by 0 and 1

	public int occupiedPlayer { get; set; } //0 or 1 for player, -1 for free
	public int i;
	public int j;
	public GameObject[] player0dots; //6 dots total per player
	public GameObject[] player1dots; 
	public int[] dotCount { get; set; } //dotCount[0] == player 0's number of dots on this cell
										//dotCount[1] == player 1's number of dots on this cell
										//dotCount[2] == cell's defauly number of dots
	private GameObject[][] playerDots;
	public int dotCap { get; set; } //player x cant place anymore dots here if dotcap is reached for this player
	public int defaultAmt;

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
	}

	public bool canBePlayedOn(int player, bool star){
		if (occupiedPlayer >= 0 || dotCount[player] >= dotCap)
			return false;
		if (dotCount [player] > 0) {
			//player has a dot here so he can play
			return true;
		} else if (star && dotCount [(player + 1) % 2] > 0 && dotCount [(player + 1) % 2] < dotCap) {
			//used a star so can play on unoccupied cell with at least one dot of other player but less than cap
			return true;
		} else {
			return false;
		}
		
	}
		
	public void makePlacement(int player, Color color, Texture texture){
		occupiedPlayer = player;
		foreach (GameObject dot in playerDots[player]) {
			if (!dot.GetComponent<dotBehavior> ().coloredIn) {
				dot.GetComponent<Material> ().color = color;
				break;
			}
		}
		//make this cell's texture the passed in one
		dotCount[player]++;
	}
	
	// Update is called once per frame
//	void Update () {
//	
//	}
}
