using UnityEngine;
using System.Collections;

public class colorDot : MonoBehaviour {

    pickUpBehavior props;
    public string prop;
    SpriteRenderer myRend;

    // Use this for initialization
    void Awake () {
        props = GetComponentInParent<pickUpBehavior>();
        myRend = gameObject.GetComponent<SpriteRenderer>();
        for (int i = 0; i < props.attrs.Length; i++)
        {
            if (props.attrs[i] == prop)
            {
                myRend.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                break;
            }
            else
            {
                Debug.Log(prop);
                myRend.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
     
	}
}
