using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class playerController : MonoBehaviour {

	private bool holding;
	private Vector3 mousePos;

	public GameObject pickups;
	public GameObject grabber;
	private GameObject pickup;
	private pickUpBehavior pickUpScript;
	public float grabDist = 0.35f;
	private float sW;
	private float sH;
    private AudioSource p1;
    private AudioSource p2;
	public GameObject textField;

	// Use this for initialization
	void Start () {
		holding = false;
		mousePos = Vector3.zero;
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = false;
		pickup = null;
		pickUpScript = null;
        AudioSource[] A = grabber.GetComponents<AudioSource>();
        p1 = A[0];
        p2 = A[1];
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 camMousPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		mousePos = new Vector3 (camMousPos.x, camMousPos.y, -camMousPos.z);
		setToMousePos (grabber);
		Debug.DrawRay (transform.position, mousePos, Color.yellow);
//		Debug.Log (mousePos.x + " " + mousePos.y);

		checkForGrabbing ();
	}

	void checkForGrabbing(){
		if (Input.GetMouseButtonDown(0) && !holding) {
			pickup = findPickUp();
			if (pickup != null){
				pickUpScript = pickup.GetComponent<pickUpBehavior> ();
				if (pickUpScript.houseIsAvailable ()) {
					Debug.Log ("pickup");
					Debug.Log ("mousPos: " + mousePos + " " + "hitPos: " + pickup.transform.position);
					Debug.Log ("distance btwn mouse and pickup: " + Vector2.Distance (mousePos, pickup.transform.position));
                    //					Debug.Break ();
                    p1.Play();
					setToMousePos (pickup);
					pickUpScript.heldByMouse = true;
					holding = true;
				} else {
					Debug.Log ("cannot pick up, house is not activated yet or you just missed...");
					if (pickUpScript != null) {
						Debug.Log ("houseavailable: " + pickUpScript.houseIsAvailable ());
					} else {
						Debug.Log ("picupScript is null");
					}
					pickup = null;
					pickUpScript = null;
					holding = false;
				}
			} else {
				pickup = null;
				pickUpScript = null;
				holding = false;
			}
		}
		if (Input.GetMouseButton (0)) {
			if (holding) {
				setToMousePos (pickup);
				pickUpScript.heldByMouse = true;
			}
		} else {
			release ();
		}
	}

	GameObject findPickUp(){
		float mindist = Mathf.Infinity;
		Transform closestPickUp = null;
		foreach (Transform pickupT in pickups.transform) {
			float distance = Vector2.Distance (pickupT.transform.position, mousePos);
			if (distance <= mindist) {
				closestPickUp = pickupT;
				mindist = distance;
			}
		}
		if (mindist <= grabDist && closestPickUp != null) {
			return closestPickUp.gameObject;
		} else {
			return null;
		}
	}

	void release(){
		if (holding) {
            //call pickup.release function that checks for any bins nearby and snap to nearest one, otherwise
            //go back to original position
            p2.Play();
			pickUpBehavior pickUpScript = pickup.GetComponent<pickUpBehavior>();
			pickUpScript.released (mousePos);
			holding = false;
			pickup = null;
			Debug.Log ("holding == false");
		}
	}

	void setToMousePos(GameObject obj){
		if (obj != null) {
			obj.transform.position = new Vector3 (mousePos.x, mousePos.y, obj.transform.position.z);
		}
	}

	void OnGUI(){
		string txt = "";
		if (holding) {
			txt = pickUpScript.text;
		}
//		sW = Screen.width;
//		sH = Screen.height;
//		Debug.Log ("display item text: " + txt);
//		Debug.Log ("ScreenHeight: " + sH + " " + "ScreenWidth: " + sW);
//		GUI.Box (new Rect (3 * sW / 10, sH / 12, 2 * sW / 5, sH / 10), txt);
		textField.GetComponent<Text>().text = txt;
	}
}
