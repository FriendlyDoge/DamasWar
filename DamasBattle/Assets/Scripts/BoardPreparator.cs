using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPreparator : MonoBehaviour
{
    [SerializeField] Piece whitePiecePrefab = null;
    [SerializeField] Piece blackPiecePrefab = null;

    [SerializeField]
    bool doClearBoard = true;

    private void Start()
    {

    }

    public Piece[][] PrepareBoard()
    {
        if(doClearBoard)
            ClearBoard();
        return PreparePieces();
    }
    private void ClearBoard()
    {
        Piece[] leftoverPieces = FindObjectsOfType<Piece>();
        foreach(Piece p in leftoverPieces)
        {
            Destroy(p.gameObject);
        }
    }

    public Piece[][] PreparePieces()
    {
        Piece[][] piecesBoard = new Piece[10][];
        for (int i = 0; i < 10; i++) {
            piecesBoard[i] = new Piece[10];
            for(int j = 0; j < 10; j++) {
                piecesBoard[i][j] = CreatePieceAtPos(i,j);
            }
        }
        return piecesBoard;
    }

    private Piece CreatePieceAtPos(int i, int j)
    {

        if (j < 3 && IsDarkSquare(i, j))
        {
            return Instantiate<Piece>(whitePiecePrefab, new Vector2(i, j), Quaternion.identity);
        }
        else if (j > 6 && IsDarkSquare(i, j))
        {
            return Instantiate<Piece>(blackPiecePrefab, new Vector2(i, j), Quaternion.identity);
        }
        else
        {
            return null;
        }
    }

    public bool IsDarkSquare(int i, int j)
    {
        if (((i % 2 == 0)&&(j % 2 == 0)) || ((i % 2 != 0 )&&(j % 2 != 0)) ) {
            return true;
        }
        return false;
    }

#if UNITY_EDITOR
    public void setTestData(Piece p)
    {
        whitePiecePrefab = p;
        blackPiecePrefab = p;
    }

    public void setInstantiateField()
    {
        PreparePieces();
    }

    public void setPrefabsTest(Piece w, Piece b)
    {
        this.whitePiecePrefab = w;
        this.blackPiecePrefab = b;
    }
#endif
}
