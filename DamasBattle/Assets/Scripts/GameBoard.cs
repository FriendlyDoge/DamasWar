using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    BoardController boardController = null;

    // Start is called before the first frame update
    void Start()
    {
        boardController = FindObjectOfType<BoardController>();
        if (!boardController)
        {
            Debug.LogError("Board Controller is null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseDown()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePos);

        Debug.Log(mousePosWorld);
        boardController.EmptySpaceClicked(Mathf.FloorToInt(mousePosWorld.x), Mathf.FloorToInt(mousePosWorld.y));
    }
}
