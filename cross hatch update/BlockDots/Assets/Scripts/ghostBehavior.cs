using UnityEngine;
using System.Collections;

public class ghostBehavior : MonoBehaviour {

	public GameObject house;
	private houseBehavior houseScript;
	private Animator controller;
	private bool activated;

	// Use this for initialization
	void Start () {
		controller = GetComponent<Animator> ();
		houseScript = house.GetComponent<houseBehavior> ();
		activated = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (houseScript.activated && !activated) {
			Debug.Log ("ghost should move up now");
//			controller.
			activated = true;
//			int val = controller.GetInteger ("value") + 1;
			controller.SetInteger ("value", 1);
		}
	}

	//when all 5 ghosts are done
	public void doFinalAnimation(){
//		int val = controller.GetInteger ("value") + 1;
		controller.SetInteger("value", 2);
	}

	public void origAnim(){
		controller.SetInteger("value", 0);
	}
}
