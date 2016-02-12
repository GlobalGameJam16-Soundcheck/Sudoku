using UnityEngine;
using System.Collections;

//comment

public class pickUpBehavior : MonoBehaviour {

	private Vector3 origPos;
	public GameObject bins; //empty gameobject containing all the bins
	public GameObject house; //house item initially belongs to
	public float binDist = 1f;
	private GameObject currBin;
	public string[] attrs;
	public bool heldByMouse { get; set; }
	public string text;

	// Use this for initialization
	void Start () {
		origPos = transform.position;
		currBin = null;
		heldByMouse = false;
	}

	public void released(Vector3 mousePos){
		//loop through all the bins, find the closest bin, if closest <= binDist, 
		//translate pickup to that binPos and check if bin is correct bin, otherwise translate to origpos
		Debug.Log("I was released!");
		float mindist = Mathf.Infinity;
		float distance;
		Transform closestBin = transform;
		foreach (Transform bin in bins.transform) {
			binBehavior binScript = bin.gameObject.GetComponent<binBehavior> ();
			if (!binScript.isOccupied() && binScript.house.GetComponent<houseBehavior>().available) { 
				distance = Vector2.Distance (transform.position, bin.position);
				Debug.Log (distance);
				if (distance < mindist) {
					mindist = distance;
					closestBin = bin;
				}
			}
		}
		if (mindist < binDist) {
			Debug.Log ("snapped to bin!");
			transform.position = closestBin.transform.position;
			resetCurrBin ();
			closestBin.gameObject.GetComponent<binBehavior> ().checkActivation (attrs);
			currBin = closestBin.gameObject;
		} else {
			Debug.Log ("snapped to oldPos, wasnt close enough to a vacant bin!");
			if (currBin == null) {
				transform.position = origPos;
			} else {
				transform.position = currBin.transform.position;
				currBin.GetComponent<binBehavior> ().checkActivation (attrs);
			}
		}
		heldByMouse = false;
	}

	private void resetCurrBin(){
		if (currBin != null) {
			//it was last at this bin, so this bin is now free
			binBehavior currBinScript = currBin.GetComponent<binBehavior>();
			currBinScript.activated = false;
			Debug.Log ("this bin is no longer holding anything");
		}
		currBin = null;
	}

	public bool houseIsAvailable(){
		return house.GetComponent<houseBehavior> ().available;
	}
}
