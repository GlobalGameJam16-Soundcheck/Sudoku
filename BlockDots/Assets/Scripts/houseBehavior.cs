using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class houseBehavior : MonoBehaviour {

	public GameObject[] housebins;
	public bool activated { get; set; }
	public bool available { get; set; }
	private binBehavior[] binScripts;
	public GameObject nextHouse;
	public GameObject ghosts;
	private float secsTillGameEnd;
	private bool startTimer;
	private float timer;
	private AudioSource houseOpen;

	// Use this for initialization
	void Start () {
		activated = false;
		available = false;
		if (transform.gameObject.name.Contains("1"))
			available = true;
		binScripts = new binBehavior[housebins.Length];
		for (int i = 0; i < housebins.Length; i++) {
			binScripts [i] = housebins [i].GetComponent<binBehavior> ();
		}
		secsTillGameEnd = 1.5f;
		timer = 0f;
		startTimer = false;
		houseOpen = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!activated && available) {
			int badCount = 0;
			foreach (binBehavior binScript in binScripts) {
				if (!binScript.activated) {
					badCount++;
				}
			}
			Debug.Log (transform.gameObject.name + " badCount: " + badCount);
			if (badCount == 0) {
				activated = true;
				houseOpen.Play ();
				makeNextHouseAvailable ();
				Debug.Log ("house has been activated! make next house available");
			}
		}
		if (startTimer) {
			timer += Time.deltaTime;
			Debug.Log ("timer: " + timer);
			if (timer >= secsTillGameEnd) {
				SceneManager.LoadScene ("end");
			}
		}
	}

	void makeNextHouseAvailable(){
		if (nextHouse != null) {
			Debug.Log ("nextHouse not null");
			nextHouse.GetComponent<houseBehavior> ().available = true;
			if (nextHouse.CompareTag ("lastHouse")) {
				Debug.Log ("5th house active");
				foreach (Transform ghost in ghosts.transform) {
					ghost.GetComponent<ghostBehavior> ().origAnim ();
				}
			}
			foreach (Transform child in nextHouse.transform) {
				if (child.name.Contains ("wall")) {
					child.GetComponent<fadeBehavior> ().startFading = true;
				}
			}
		} else {
			Debug.Log ("this is the last house! you win!");
			foreach (Transform ghost in ghosts.transform) {
				ghost.GetComponent<ghostBehavior> ().doFinalAnimation ();
			}
			startTimer = true;
		}
	}
				
}
