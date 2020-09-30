using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    int playerNumber = 1;

    [SerializeField]
    int maxMovementSquares = 1;

    [SerializeField] bool backwardsAllowed = false;
    [SerializeField] bool lineMoveAllowed = false;

    [Range(-1,1)][SerializeField] int backwardsDirection = 1;

    [Range(1f, 3f)][SerializeField] float deathAnimationDuration = 1f; 

    BoardController boardController = null;
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

    public void Die()
    {
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("Died");
        yield return new WaitForSeconds(deathAnimationDuration);
        Destroy(this.gameObject);
        StopCoroutine(DeathRoutine());
    }

    public void RemoveHighlight()
    {

    }

    public void AddHighlight()
    {

    }

    public int GetXPosition()
    {
        return Mathf.FloorToInt(transform.position.x);
    }

    public int GetYPosition()
    {
        return Mathf.FloorToInt(transform.position.y);
    }

    private void OnMouseDown()
    {
        Debug.Log("Piece clicked!");
        boardController.PieceWasClicked(this);
    }

    public int GetPlayerNumber()
    {
        return this.playerNumber;
    }

    public bool CanMoveInLine()
    {
        return lineMoveAllowed;
    }

    public bool CanMoveBackwards()
    {
        return backwardsAllowed;
    }

    public void MoveTo(int xPosNew, int yPosNew)
    {
        Debug.Log("Moving piece at " + transform.position.x.ToString() + "," + transform.position.y.ToString() + " " +
            "To " + xPosNew.ToString() + "," + yPosNew.ToString());
        Vector2 newPosition = new Vector2(xPosNew, yPosNew);

        this.transform.position = newPosition;

    }

    public bool IsThisBackwards(int yPosNew) 
    {
        int currY = GetYPosition();

        if( Mathf.Sign(yPosNew - currY) == backwardsDirection) // Direction that is moving
        {
            return true;
        }
        else 
        {
            return false;
        }
    }

    public bool CanReach(int posYNew)
    {
        int currY = GetYPosition();

        if( Mathf.Abs(posYNew - currY) <= maxMovementSquares)
        {
            return true;
        } else
        {
            return false;
        }

    }
}
