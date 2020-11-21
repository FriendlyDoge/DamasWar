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

    [Range(-1, 1)] [SerializeField] int backwardsDirection = 1;

    [Range(1f, 3f)] [SerializeField] float deathAnimationDuration = 1f;

    BoardController boardController = null;

    // Data for highlighting
    [SerializeField] GameObject highlightingPrefab = null;
    GameObject highlightingObject = null;

    /*[SerializeField] GameObject supDir = null;
    [SerializeField] GameObject supEsq = null;
    [SerializeField] GameObject infDir = null;
    [SerializeField] GameObject infEsq = null;
    GameObject positionHighlightObject = null;*/

    [SerializeField] GameObject isDama = null;

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
        if (highlightingObject)
        {
            DestroyImmediate(highlightingObject);
        }
    }

    public void AddHighlight()
    {
        highlightingObject = Instantiate(highlightingPrefab, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
    }

    public void makeDama()
    {
        bool dama = false;
        Debug.Log("GetPlayerNumber() " + GetPlayerNumber());
        Debug.Log("GetYPosition()" + GetYPosition());
        if(GetPlayerNumber() == 1){
            if(GetYPosition() == 9) {
                dama = true;
                Debug.Log("player 1 and Yposition = 0");
            }
        }else{
            if(GetYPosition() == 0){
             dama = true;
             Debug.Log("player 0 ou 2 and Yposition = 9") ;
            }
        }
        if(dama) this.transform.Find("isDama").gameObject.SetActive(true);
        //return dama;
    }
    public void AddHighlightPossibleMovements()
    {
        if(GetPlayerNumber() == 1)
        {
            bool diagSupDir = true;
            bool diagSupEsq = true;
            bool diagInfDir = true;
            bool diagInfEsq = true;
            if(GetXPosition() == 0)
            {
                diagSupEsq = false;
                diagInfEsq = false;
                //if (!CanMoveBackwards())
                //{
                 //   diagInfDir = false;
                //}
            }
            if(GetYPosition() == 0)
            {
                diagInfDir = false;
                diagInfEsq = false;
            }
            if(GetXPosition() == 9)
            {
                diagSupDir = false;
                diagInfDir = false;
               // if (!CanMoveBackwards())
                //{
                 //   diagInfEsq = false;
                //}
            }
            if(GetYPosition() == 9)
            {
                diagSupEsq = false;
                diagSupDir = false;
            }
            if (boardController.CanMoveToSpace(this, GetXPosition() - 1, GetYPosition() - 1))
            {
                //infEsq.GetComponent<Renderer>().enabled = true;
                this.transform.Find("infesq").gameObject.SetActive(true);
                Debug.Log("pode ir pra inferior esquerda");
            }
            if (boardController.CanMoveToSpace(this, GetXPosition() + 1, GetYPosition() - 1))
            {
                //infDir.GetComponent<Renderer>().enabled = true;
                this.transform.Find("infdir").gameObject.SetActive(true);
                Debug.Log("pode ir pra inferior direita");
            }
            if ( boardController.CanMoveToSpace(this, GetXPosition() + 1, GetYPosition()+ 1))
            {
                //supDir.GetComponent<Renderer>().enabled = true;
                this.transform.Find("supdir").gameObject.SetActive(true);
                Debug.Log("pode ir pra superior direita");
            }
            if (boardController.CanMoveToSpace(this, GetXPosition() - 1, GetYPosition() + 1))
            {
                //supEsq.GetComponent<Renderer>().enabled = true;
                this.transform.Find("supesq").gameObject.SetActive(true);
                Debug.Log("pode ir pra superior esquerda");
            }
        }
        
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
     //   if(isDama != null){
            Debug.Log("Moving piece at " + transform.position.x.ToString() + "," + transform.position.y.ToString() + " " +
                "To " + xPosNew.ToString() + "," + yPosNew.ToString());
            Vector2 newPosition = new Vector2(xPosNew, yPosNew);
       // }else{

        //}
        this.transform.position = newPosition;

    }

    public bool IsThisBackwards(int yPosNew)
    {
        int currY = GetYPosition();

        if (Mathf.Sign(yPosNew - currY) == backwardsDirection) // Direction that is moving
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

        if (Mathf.Abs(posYNew - currY) <= maxMovementSquares)
        {
            return true;
        } else
        {
            return false;
        }

    }
}
