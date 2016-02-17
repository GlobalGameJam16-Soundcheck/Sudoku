using UnityEngine;
using System.Collections;

public class BluePlaceScript : MonoBehaviour
{

    float startSize;

    // Use this for initialization
    void Start()
    {
        startSize = transform.localScale.x;
        setInvisable();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setVisable()
    {
        transform.localScale = new Vector3(startSize, startSize, startSize);
    }

    public void setInvisable()
    {
        transform.localScale = new Vector3(0, 0, 0);
    }

    public bool getVisable()
    {
        if (transform.localScale.x == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
