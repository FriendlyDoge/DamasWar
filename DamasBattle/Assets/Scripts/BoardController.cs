using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using UnityEditor.UIElements;
using UnityEngine;
using TMPro;

public class BoardController : MonoBehaviour
{
    
    BoardPreparator boardPreparator = null;

    Piece[][] boardState = null;

    Piece currentPiece = null;

    int playersNumbers = 2;

    int currentPlayerNumber = 1;

    [SerializeField]
    string playerName = "Jogador";

    [SerializeField]
    string computerName = "Computador";

    [SerializeField]
    TMPro.TextMeshProUGUI currentPlayerText = null;

    void Start()
    {
        boardPreparator = GetComponent<BoardPreparator>();
        boardState = boardPreparator.PrepareBoard();
        if (boardState.Length != 10)
        {
            Debug.LogError("Board of wrong size!");
        }
        else
        {
            foreach (Piece[] line in boardState)
            {
                if (line.Length != 10)
                {
                    Debug.LogError("Board of wrong size!");
                }
            }
        }

        currentPlayerText.SetText(playerName);

    }

    void EndCurrentPlayerTurn()
    {
        Debug.Log("Ending current player turn");
        RemoveCurrentPieceSelection();
        UpdateCurrentPlayer();
    }

    void UpdateCurrentPlayer()
    {
        Debug.Log("Updating current player");
        currentPlayerNumber++;
        if(currentPlayerNumber > playersNumbers)
        {
            currentPlayerNumber = 1;
            currentPlayerText.SetText(playerName);
        } else
        {
            currentPlayerText.SetText(computerName);
        }

    }
    public void PieceWasClicked(Piece piece)
    {
        if(currentPiece != null)
        {
            //currentPiece.AddHighlightPossibleMovements();
            currentPiece.RemoveHighlight();
        }
        Debug.Log("Piece was clicked");
        if (piece.GetPlayerNumber() == currentPlayerNumber)
        {
            piece.AddHighlight();
            currentPiece = piece;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void PromoteToQueen(Piece pieceToPromote)
    {
        Debug.Log("Promoting piece to queen");
    }

    public void EmptySpaceClicked(int xWorldPos, int yWorldPos)
    {
        bool attacked = false;

        Debug.Log("Empty Space Clicked");
        if (currentPiece == null)
        {
            Debug.Log("There is no piece selected");
        }
        else
        {
            if (currentPiece.GetPlayerNumber() == currentPlayerNumber)
            //if(true)
            {
                Piece targetPiece = PieceToAttackFromPosition(currentPiece, xWorldPos, yWorldPos);
                if (targetPiece && boardPreparator.IsDarkSquare(xWorldPos, yWorldPos))
                {
                    Debug.Log("Can attack!");
                    ExecuteAttack(currentPiece, targetPiece);
                    ExecuteMove(currentPiece.GetXPosition(), currentPiece.GetYPosition(), xWorldPos, yWorldPos);
                    attacked = true;
                    EndCurrentPlayerTurn();
                }
                else if (CanMoveToSpace(currentPiece, xWorldPos, yWorldPos))
                {
                    Debug.Log(xWorldPos.ToString() + " AND " + yWorldPos.ToString());
                    ExecuteMove(currentPiece.GetXPosition(), currentPiece.GetYPosition(), xWorldPos, yWorldPos);
                    EndCurrentPlayerTurn();

                }
            }
            else
            {
                Debug.Log("That's not your turn funny boy...");
            }
        }

        CheckBoardState(attacked);
        
    }

    private bool CanAttackAgain()
    {
        throw new NotImplementedException();
    }

    // Check if you can make a queen etc
    private void CheckBoardState(bool attacked)
    {
        //ifCanAttackAgain();
        RemoveCurrentPieceSelection();
        
    }

    private void RemoveCurrentPieceSelection()
    {
        if (!currentPiece)
        {
            Debug.LogError("RemoveCurrentPieceSelection -> No piece selected.");
        }
        Debug.Log("Removing Current Piece Selection");
        currentPiece.RemoveHighlight();
    }

    private void ExecuteMove(int xPosOld, int yPosOld, int xPosNew, int yPosNew)
    {
        
        
        Debug.Log("ExecuteMove: Execution of move action from " + xPosOld.ToString() + "," + xPosNew.ToString() + " ");
        Debug.Log("ExecuteMove: To " + xPosNew.ToString() + "," + yPosNew.ToString());
        boardState[xPosOld][yPosOld] = null;
        boardState[xPosNew][yPosNew] = currentPiece;

        currentPiece.MoveTo(xPosNew, yPosNew);
        currentPiece.makeDama();
        RemoveCurrentPieceSelection();

    }

    public bool CanMoveToSpace(Piece p, int posXNew, int posYNew)
    {
        int posXOld = p.GetXPosition();
        int posYOld = p.GetYPosition();
        Debug.Log("CanMoveToSpace: Can move to space?");

        if( !boardPreparator.IsDarkSquare(posXNew, posYNew))
        {
            Debug.Log("CanMoveToSpace: Cannot move to a white square!");
            return false;
        }
        // If is moving horizontally or vertically, it is just not possible sir
        if (posXOld == posXNew || posYOld == posYNew)
        {
            if (!p.CanMoveInLine() )
            {
                Debug.Log("CanMoveToSpace: Cannot move in line!");
                return false;
            }
        }

        // Can walk Backwards?
        if (p.IsThisBackwards(posYNew)) { // Can't walk back up
            if (!p.CanMoveBackwards())
            {
                Debug.Log("CanMoveToSpace: Cannot move backwards!");
                return false;
            }
        }

        //Is it an allowedMoveRange?
        if (!p.CanReach(posYNew))
        {
            return false;
        }

        //if is occupied
        if (boardState[Mathf.FloorToInt(posXNew)][Mathf.FloorToInt(posYNew)])
        {
            return false;
        }else
        {
            return true;
        }
    }

    private void ExecuteAttack(Piece attacker, Piece targetPiece)
    {
        Debug.Log("ExecuteAttack: Executing attack");
        targetPiece.Die();
    }

    Piece PieceToAttackFromPosition(Piece attackingPiece, int xEndPos, int yEndPos)
    {
        string attackingPieceTag = attackingPiece.tag;
        Debug.Log("PieceToAttackFromPosition: Can piece of tag " + attackingPieceTag + " attack position?");
        if (!PieceAtPosExists(xEndPos, yEndPos) && boardPreparator.IsDarkSquare(xEndPos, yEndPos))
        {
            Vector2Int attackingPosition = GetAttackingPosition(attackingPiece.GetXPosition(), attackingPiece.GetYPosition(), xEndPos, yEndPos);
            
            if (PieceAtPosExists(attackingPosition.x, attackingPosition.y) && PosHasPieceOfDifferentTag(attackingPosition.x, attackingPosition.y, attackingPieceTag))
            {
                Debug.Log("PieceToAttackFromPosition: here is a piece to attack!");
                return boardState[attackingPosition.x][attackingPosition.y];
            }
            else
            {
                Debug.Log("PieceToAttackFromPosition: There is NO piece to attack!");
                return null;
            }
        } else
        {
            return null;
        }
    }
    bool PieceAtPosExists(int xPos, int yPos)
    {
        Debug.Log("PieceAtPosExists: Is there a piece at position X:" + xPos.ToString() + " Y:" +yPos.ToString() + " ?");
        Piece pieceAtPos = boardState[xPos][yPos];
        if (pieceAtPos)
        {
            return true;
        }
        return false;
    }

    bool PosHasPieceOfDifferentTag(int xPos, int yPos, string tagName)
    {
        Debug.Log("PosHasPieceOfDifferentTag: Position piece with a tag different of the tag: " + tagName + " ?");
        Piece pieceAtPos = boardState[Mathf.FloorToInt(xPos)][Mathf.FloorToInt(yPos)];
        if(pieceAtPos && !pieceAtPos.CompareTag(tagName) || !pieceAtPos)
        {
            return true;
        } else
        {
            return false;
        }
    }

    Vector2Int GetAttackingPosition(int xStartPos, int yStartPos, int xEndPos, int yEndPos)
    {
        Debug.Log("GetAttackingPosition: Get attacking position");
        int xAttackPosition = (xEndPos + xStartPos) / 2;
        int yAttackPosition = (yEndPos + yStartPos) / 2;
        Vector2Int attackingDirection = new Vector2Int(xAttackPosition, yAttackPosition);
        return attackingDirection;
    }

    bool IsWhitePiece(Piece p)
    {
        return p.CompareTag("P1 Piece");
    }

    public void RestartGame()
    {
        boardPreparator.PrepareBoard();
        if (currentPlayerNumber != 1)
        {
            UpdateCurrentPlayer();
        }

        boardState = boardPreparator.PrepareBoard();
        if (currentPiece)
        {
            currentPiece.RemoveHighlight();
            currentPiece = null;
        }
    }
}
