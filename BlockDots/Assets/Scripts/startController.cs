using UnityEngine;
using System.Collections;

public class startController : MonoBehaviour {

	public GameObject[] howToShots;
	public int clickCount { get; set; }
	private bool howToWasClicked;
	private bool wasSetInactive;
	public GameObject startButton;
	public GameObject howToButton;
	public float posZ;

	void Start(){
		clickCount = -1;
		howToWasClicked = false;
		wasSetInactive = false;
	}

	void Update(){
		if (howToButton.GetComponent<startScreen> ().howToWasClicked) {
			howToWasClicked = true;
		} else {
			howToWasClicked = false;
			clickCount = -1;
			wasSetInactive = false;
		}

		if (howToWasClicked && Input.GetMouseButtonDown (0)) {
			clickCount++;
			Debug.Log ("click count = " + clickCount);
		}

		if (howToWasClicked && Input.GetMouseButtonDown (1)) {
			clickCount--;
			Debug.Log ("right click, clickCount = " + clickCount);
		}

		if (clickCount == 0 && !wasSetInactive) {
			howToButton.SetActive (false);
			startButton.SetActive (false);
			wasSetInactive = true;
		} else if (clickCount == -1 && wasSetInactive) {
			howToButton.SetActive (true);
			startButton.SetActive (true);
			wasSetInactive = false;
		}

		if (clickCount >= 0 && clickCount < howToShots.Length) {
			Vector3 pos = howToShots [clickCount].transform.position;
			howToShots [clickCount].transform.position = new Vector3 (pos.x, pos.y, posZ);
			for (int i = 0; i < howToShots.Length; i++) {
				if (i != clickCount) {
					pos = howToShots [i].transform.position;
					howToShots[i].transform.position = new Vector3 (pos.x, pos.y, 0f);
				}
			}
		}

		if (clickCount >= howToShots.Length) {
			for (int i = 0; i < howToShots.Length; i++) {
				Vector3 pos = howToShots [i].transform.position;
				howToShots[i].transform.position = new Vector3 (pos.x, pos.y, 0f);
			}
			clickCount = -1;
			wasSetInactive = true;
		}

		//display howToShots[clickCount] if clickCount >= 0 && clickCount < howToShots.Length;
	}
}
