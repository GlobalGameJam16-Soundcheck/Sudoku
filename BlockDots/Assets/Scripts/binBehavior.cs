using UnityEngine;
using System.Collections;

public class binBehavior : MonoBehaviour {

	public GameObject pickups;
	public string[] attrs;
	public bool activated { get; set; }
    SpriteRenderer myRend;
	public float snapDist = 0.1f;
	public GameObject house;
	private pickUpBehavior[] pickUpScripts;
	private Light plight;

	// Use this for initialization
	void Awake () {
		activated = false;
        myRend = gameObject.GetComponent<SpriteRenderer>();
		pickUpScripts = new pickUpBehavior[pickups.transform.childCount];
		for (int i = 0; i < pickups.transform.childCount; i++){
			pickUpScripts [i] = pickups.transform.GetChild (i).gameObject.GetComponent<pickUpBehavior> ();
		}
		plight = null;
		foreach (Transform child in transform) {
			if (child.CompareTag ("light")) {
				plight = child.GetComponent<Light>();
				Debug.Log ("this is a light");
			}
		}
	}

	public bool isOccupied(){
		//loop through all the pickups every frame and check if any of their positions is same as this, and if it is,
		//occupied true, and if none, then occupied false?
		foreach (pickUpBehavior pickUpScript in pickUpScripts){
			Transform pickup = pickUpScript.transform;
			if (Vector2.Distance(pickup.position, transform.position) <= snapDist &&
				!pickUpScript.heldByMouse) { //this pickup is not being held by mouse
				Debug.Log ("this bin is occupado");
				return true;
			}
		}
		if (!transform.CompareTag ("startbin")) {
			myRend.color = new Color (1.0f, 0.0f, 0.0f);
			if (plight != null) {
				plight.color = new Color (1.0f, 0.0f, 0.0f);
			}
		}
        return false;
	}

    public void checkActivation(string[] pickupAttrs) {
        //invariant is that occupied == false
		int matchingAttrsCount = 0;
        foreach (string attr in attrs) {
			foreach (string pickUpAttr in pickupAttrs) {
				if (string.Compare (attr, pickUpAttr) == 0) {
					matchingAttrsCount++;
				}
			}
        }
		if (matchingAttrsCount > 0 && matchingAttrsCount == attrs.Length) {
			myRend.color = new Color (0.0f, 1.0f, 0.0f);
			if (plight != null) {
				plight.color = new Color (0.0f, 1.0f, 0.0f);
			}
			Debug.Log ("this pickup turns me on");
			activated = true;
		} else {
			Debug.Log ("this pickup turns me off");
			activated = false;
		}
	}
}
