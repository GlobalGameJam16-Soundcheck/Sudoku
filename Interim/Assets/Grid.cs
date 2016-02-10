using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {

    public GameObject tilePrefab;
    GameObject[,] ourGrid;
    int gridDim = 9;
    public int selectedNumber;
    public int currentPlayer = 1;
    public GUIText p1Score;
    public GUIText p2Score;
    public int player1score = 0;
    public int player2score = 0;

	// Use this for initialization
	void Start () {
        ourGrid = new GameObject [9, 9];
        for (int i = 0; i < 9; i ++)
        {
            for (int j = 0; j < 9; j ++)
            {
                ourGrid [i, j] = (GameObject) Instantiate (tilePrefab, new Vector3 (transform.position.x + i*transform.localScale.x, transform.position.y * transform.localScale.y, transform.position.z + j * transform.localScale.z), transform.rotation);
                ourGrid[i, j].GetComponent<TileBehavior>().num = 10;
                ourGrid[i, j].GetComponent<TileBehavior>().gridPos = new Vector2(i, j);
            }
        }
        updateScore();
    }
	
	// Update is called once per frame
	void Update () {
        //mouse has been clicked
        if (Input.GetMouseButtonDown(0))
        {
            GameObject tempObject = objectClicked();
            if (objectClicked() != null)
            {
                //if we clicked a number icon
                if (tempObject.tag == "Num")
                {
                    selectedNumber = tempObject.GetComponent<NumBehavior>().num;
                }
                //if we clicked a tile
                if (tempObject.tag == "Tile")
                {
                    Vector2 tempGridPos = tempObject.GetComponent<TileBehavior>().gridPos;
                    int tempTileNum = tempObject.GetComponent<TileBehavior>().num;
                    //make sure it's valid to place the current number here
                    if (canPlaceCheck(tempGridPos, selectedNumber))
                    {
                        tempObject.GetComponent<TileBehavior>().num = selectedNumber;
                        tempObject.GetComponent<TileBehavior>().owner = currentPlayer;
                        if(currentPlayer == 1)
                        {
                            currentPlayer = 2;
                        }
                        else
                        {
                            currentPlayer = 1;
                        }
                    }
                }
            }
        }
	}

    GameObject objectClicked()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.transform.gameObject;
        }
        else
        {
            return null;
        }
    }

    bool canPlaceCheck(Vector2 gridPos, int num)
    {
        if(ourGrid[(int)gridPos.x, (int)gridPos.y].GetComponent<TileBehavior>().num != 10)
        {
            return false;
        }
        for(int i = 0; i < gridDim; i++)
        {
            //check row
            if(ourGrid[i, (int)gridPos.y].GetComponent<TileBehavior>().num == num)
            {
                return false;
            }
            //check col
            if (ourGrid[(int)gridPos.x, i].GetComponent<TileBehavior>().num == num)
            {
                return false;
            }
        }
        //check box
        int rowStart = ((int)gridPos.x / 3) * 3;
        int colStart = ((int)gridPos.y / 3) * 3;
        int rowEnd = rowStart + 2;
        int colEnd = colStart + 2;
        for (int x = rowStart; x <= rowEnd; x++)
        {
            for (int y = colStart; y <= colEnd; y++)
            {
                if (ourGrid[x, y].GetComponent<TileBehavior>().num == num)
                {
                    return false;
                }
            }
        }
        //it all checks out
        return true;
    }

    public void updateScore()
    {
        p1Score.text = "Player 1 Score: " + player1score;
        p2Score.text = "Player 2 Score: " + player2score;
    }     
}
