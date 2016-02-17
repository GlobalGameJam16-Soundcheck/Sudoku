using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class startButtonFade : MonoBehaviour {

	private float timer;
	private float startFadeInTime;
	private float titleFadeInTime;
	public GameObject startButton;
	public GameObject title;
	private Image img;
	private SpriteRenderer sr;
	private float targetVal;
	private float startVal;
	private float startToFade;
	private float startTitleFade;

	// Use this for initialization
	void Start () {
		timer = 0f;
		startFadeInTime = 3f;
		titleFadeInTime = 5f;
		startTitleFade = 2f;
		startToFade = startTitleFade + titleFadeInTime;
		targetVal = 255f;
		img = startButton.GetComponent<Image> ();
		sr = title.GetComponent<SpriteRenderer> ();

		startVal = Mathf.Min(img.color.a, sr.color.a);
	}
	
	// Update is called once per frame
	void Update () {
		float ratio;
		float a;
		timer += Time.deltaTime;
		if (timer >= startToFade) { //fade start button in
			ratio = Time.deltaTime / startFadeInTime;
			a = Mathf.Abs (startVal - targetVal) * ratio / 255f;
			img.color = new Color (img.color.r, img.color.g, img.color.b, img.color.a + a);
		} else {
			//fade title in
			if (timer >= startTitleFade) {
				ratio = Time.deltaTime / titleFadeInTime;
				a = Mathf.Abs (startVal - targetVal) * ratio / 255f;
				sr.color = new Color (sr.color.r, sr.color.g, sr.color.b, sr.color.a + a);
			}
		}
	}
}
