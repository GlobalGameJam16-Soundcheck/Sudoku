using UnityEngine;
using System.Collections;

public class TileBehavior : MonoBehaviour {

    public int num = 0;
    public int owner = 0;
    public Texture[] textures;
    Renderer myMat;
    public Vector2 gridPos;

	// Use this for initialization
	void Start () {
        myMat = gameObject.GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
        myMat.material.mainTexture = textures[num - 1];
	}
}
