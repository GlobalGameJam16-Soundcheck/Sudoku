using UnityEngine;
using System.Collections;

public class startController : MonoBehaviour {

	public GameObject[] howToShots;
	public int clickCount { get; set; }
	private bool howToWasClicked;
	private bool isActive;
	public GameObject startButton;
	public GameObject howToButton;
	public float posZ;

	void Start(){
		clickCount = -1;
		howToWasClicked = false;
		isActive = true;
	}

	void Update(){
		if (howToButton.GetComponent<startScreen> ().howToWasClicked) {
			howToWasClicked = true;
		}

		if (howToWasClicked && Input.GetMouseButtonDown (0)) {
			clickCount++;
			Debug.Log ("click count = " + clickCount);
		}

		if (howToWasClicked && Input.GetMouseButtonDown (1)) {
			clickCount--;
			Debug.Log ("right click, clickCount = " + clickCount);
		}

		if (clickCount == 0 && howToWasClicked && isActive) {
			howToButton.SetActive (false);
			startButton.SetActive (false);
			isActive = false;
		} else if ((clickCount == -1 || clickCount >= howToShots.Length) && !isActive) {
			howToButton.SetActive (true);
			startButton.SetActive (true);
			isActive = true;
			for (int i = 0; i < howToShots.Length; i++) {
				Vector3 pos = howToShots [i].transform.position;
				howToShots[i].transform.position = new Vector3 (pos.x, pos.y, 0f);
			}
			howToWasClicked = false;
			clickCount = -1;
			isActive = true;
			howToButton.GetComponent<startScreen> ().setClickerToFalse ();
		}

		if (clickCount >= 0 && clickCount < howToShots.Length && !isActive) {
			Vector3 pos = howToShots [clickCount].transform.position;
			howToShots [clickCount].transform.position = new Vector3 (pos.x, pos.y, posZ);
			for (int i = 0; i < howToShots.Length; i++) {
				if (i != clickCount) {
					pos = howToShots [i].transform.position;
					howToShots[i].transform.position = new Vector3 (pos.x, pos.y, 0f);
				}
			}
		}

//		if (clickCount >= howToShots.Length) {
//			for (int i = 0; i < howToShots.Length; i++) {
//				Vector3 pos = howToShots [i].transform.position;
//				howToShots[i].transform.position = new Vector3 (pos.x, pos.y, 0f);
//			}
//			clickCount = 0;
//			isActive = true;
//		}

		if (howToButton.activeInHierarchy) {
			howToWasClicked = false;
			clickCount = -1;
			isActive = true;
		}

		//display howToShots[clickCount] if clickCount >= 0 && clickCount < howToShots.Length;
	}
}
