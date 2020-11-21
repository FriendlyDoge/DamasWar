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

    void EndCurrentPlayerTurn(bool atackAgain)
    {
        Debug.Log("Ending current player turn");
        RemoveCurrentPieceSelection();
        if(!atackAgain) UpdateCurrentPlayer();
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
             
            currentPiece.RemoveHighlightPossibleMovements();
            currentPiece.RemoveHighlight();
        }
        Debug.Log("Piece was clicked");
        if (piece.GetPlayerNumber() == currentPlayerNumber)
        {
            piece.AddHighlightPossibleMovements();
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
        bool attackAgain = false;
        Debug.Log("Empty Space Clicked");
        if (currentPiece == null)
        {
            Debug.Log("There is no piece selected");
        }
        else
        {
            if (currentPiece.GetPlayerNumber() == currentPlayerNumber)
            {
                Piece targetPiece = PieceToAttackFromPosition(currentPiece, xWorldPos, yWorldPos);
                if (targetPiece && boardPreparator.IsDarkSquare(xWorldPos, yWorldPos))
                {
                    Debug.Log("Can attack!");
                    ExecuteAttack(currentPiece, targetPiece);
                    ExecuteMove(currentPiece.GetXPosition(), currentPiece.GetYPosition(), xWorldPos, yWorldPos);
                    attacked = true;
                    attackAgain = CanAttackAgain(currentPiece.GetXPosition(), currentPiece.GetYPosition());
                    EndCurrentPlayerTurn(attackAgain);
                }
                else if (CanMoveToSpace(currentPiece, xWorldPos, yWorldPos))
                {
                    Debug.Log(xWorldPos.ToString() + " AND " + yWorldPos.ToString());
                    ExecuteMove(currentPiece.GetXPosition(), currentPiece.GetYPosition(), xWorldPos, yWorldPos);
                    EndCurrentPlayerTurn(attackAgain);

                }
            }
            else
            {
                Debug.Log("That's not your turn funny boy...");
            }
        }

        CheckBoardState(attacked);
        
    }

    private bool CanAttackAgain(int x, int y)
    {
        Piece targetPiece1 = null;
        Piece targetPiece2 = null;
        Piece targetPiece3 = null;
        Piece targetPiece4 = null;
        if(y+2<10){
            if(x+2 < 10 && boardState[x+2][y+2] == null) targetPiece1 = PieceToAttackFromPosition(currentPiece, x+2, y+2);
            if(x-2>0 && boardState[x-2][y+2] == null) targetPiece2 = PieceToAttackFromPosition(currentPiece, x-2, y+2);
        }
        if(y-2>0){
            if(x-2 > 0 && boardState[x-2][y-2] == null) targetPiece3 = PieceToAttackFromPosition(currentPiece, x-2, y-2);
            if(x+2 < 10 && boardState[x+2][y-2] == null) targetPiece4 = PieceToAttackFromPosition(currentPiece, x+2, y-2);
        }
        if(targetPiece1 || targetPiece2 || targetPiece3 || targetPiece4){
            if(targetPiece1) print("Peca 1");
            if(targetPiece2) print("Peca 2");
            if(targetPiece3) print("Peca 3");
            if(targetPiece4) print("Peca 4");
            return true;
        }
        return false;
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
        currentPiece.RemoveHighlightPossibleMovements();
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
        print("!p.isThisDama(): "+ !p.isThisDama());
        if (  p.IsThisBackwards(posYNew)) { // Can't walk back up
            if (!p.isThisDama() &&!p.CanMoveBackwards())
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
        boardState[targetPiece.GetXPosition()][targetPiece.GetYPosition()] = null;
        targetPiece.Die();
    }

    Vector2Int ExistsAtackedPiece(Piece attackingPiece, int xEndPos, int yEndPos)   
    {
        Piece p;
        //contar quantas peças há na linha
        int x =attackingPiece.GetXPosition(), y = attackingPiece.GetYPosition();

        int casasAndadasX = -x +  xEndPos;
        int dirX = 0;
        if(casasAndadasX > 0) dirX = 1;
        else dirX = -1;
        casasAndadasX = Math.Abs(casasAndadasX);

        int casasAndadasY = -y +  yEndPos;
        int dirY = 0;
        if(casasAndadasY > 0)   dirY = 1;
        else                    dirY= -1;
        casasAndadasY = Math.Abs(casasAndadasY);

        int pecasLinha = 0;
        int pecasOutraCorLinha = 0;

        int retx = 0, rety = 0;
        for(int i = 0; i < casasAndadasX; i++){
            x+=dirX;
            y+=dirY;
            p = boardState[x][y];

            if(p != null){
                pecasLinha++;
                if(PosHasPieceOfDifferentTag(x, y, attackingPiece.tag)){
                    pecasOutraCorLinha++;
                    retx = x;
                    rety = y;
                }
            }
        }
        Vector2Int attackingDirection = new Vector2Int(retx, rety);
        //Se for apenas uma, saber se é de outra cor
        //Caso seja de outra cor, retorne a posição dela.

        if(pecasLinha == pecasOutraCorLinha && pecasOutraCorLinha == 1) return new Vector2Int(retx, rety);
        
        return new Vector2Int(0, 0);
    }
    Piece PieceToAttackFromPosition(Piece attackingPiece, int xEndPos, int yEndPos)
    {
        string attackingPieceTag = attackingPiece.tag;
        Debug.Log("PieceToAttackFromPosition: Can piece of tag " + attackingPieceTag + " attack position?");
        if (!PieceAtPosExists(xEndPos, yEndPos) && boardPreparator.IsDarkSquare(xEndPos, yEndPos))
        {
            //Vector2Int attackingPosition = GetAttackingPosition(attackingPiece.GetXPosition(), attackingPiece.GetYPosition(), xEndPos, yEndPos);
            Vector2Int attackingPosition = ExistsAtackedPiece(attackingPiece, xEndPos, yEndPos);
            //if (PieceAtPosExists(attackingPosition.x, attackingPosition.y) && PosHasPieceOfDifferentTag(attackingPosition.x, attackingPosition.y, attackingPieceTag))
            if (attackingPosition.x != 0)
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
            currentPiece.RemoveHighlightPossibleMovements();
            currentPiece = null;
        }
    }
}
