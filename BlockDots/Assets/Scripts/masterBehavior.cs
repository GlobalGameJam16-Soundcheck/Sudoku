using UnityEngine;
using System.Collections;

public class masterBehavior : MonoBehaviour {

	public GameObject[] cellPrefabs; //0 is 0 points, 1 is 1 pt, 2 is 2 pts, 3 is pts, 4 is black mid square
	public playerBehavior[] players;

	private GameObject[,] grid;
	private int n;
	private int currPlayer;

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
				Vector3 pos = new Vector3 (transform.position.x + j*transform.localScale.x, 
										   transform.position.y - i*transform.localScale.y, 
										   transform.position.z * transform.localScale.z);
				grid[i,j] = (GameObject)Instantiate(cellPrefabs[whichPrefab], pos, transform.rotation);
				cellBehavior cellScript = grid [i, j].GetComponent<cellBehavior> ();
				cellScript.Init ();
				cellScript.i = i;
				cellScript.j = j;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
