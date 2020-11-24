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
        if (atackAgain)
        {
            if (currentPlayerNumber != 1)
            {
                BotAction();
            }
        }
        else
        {
            UpdateCurrentPlayer();
            if (currentPlayerNumber != 1)
            {
                BotAction();
            }
        }
    }

    void BotAction()
    {
        Piece p;
        bool atk = false;
        bool move = false;
        bool attackAgain = false;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (((i + j) % 2) == 0)
                {
                    int[] pos;
                    pos = null;
                    p = boardState[i][j];
                    if (p != null && p.CompareTag("P2 Piece")) pos = BotAttacker(p, i, j);
                    if (p && pos != null && atk == false) // p.CompareTag("P2 Piece") serve pra escolher só peça da IA
                    {
                        Piece targetPiece = PieceToAttackFromPosition(p, pos[0], pos[1]);
                        if (targetPiece && boardPreparator.IsDarkSquare(pos[0], pos[1]))
                        {
                            currentPiece = p;
                            Debug.Log("Can attack!");
                            ExecuteAttack(p, targetPiece);
                            ExecuteMove(p.GetXPosition(), p.GetYPosition(), pos[0], pos[1]);

                            attackAgain = CanAttackAgain(pos[0], pos[1]);
                            EndCurrentPlayerTurn(attackAgain);
                            atk = true;
                        }
                    }
                }
            }
        }
        if(atk == false)
        {
            //Botar um seed, mas funciona
            List<Piece> possibleMoves = new List<Piece>();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    p = boardState[i][j];
                    if (p != null && move == false && p.CompareTag("P2 Piece"))
                    {
                        /*if (IsPositionInsideBoard(i + 1, j - 1) && boardState[i + 1][j - 1] == null && boardPreparator.IsDarkSquare(i+1, j-1))
                        {
                            // aqui estava currentPiece que é a peça selecionada atualmente, só que a peça atual não era p.
                            currentPiece = p;
                            ExecuteMove(p.GetXPosition(), p.GetYPosition(), i+1, j-1);
                            EndCurrentPlayerTurn(attackAgain);
                            move = true;
                        }
                        else if (IsPositionInsideBoard(i - 1, j - 1) && boardState[i - 1][j - 1] == null && boardPreparator.IsDarkSquare(i - 1, j - 1))
                        {
                            currentPiece = p;
                            ExecuteMove(p.GetXPosition(), p.GetYPosition(), i - 1, j - 1);
                            EndCurrentPlayerTurn(attackAgain);
                            move = true;
                        }

                        Implementando o aleatorio a partir daqui, o codigo em cima funfa mas n aleatorio */
                        if (CanBlackMove(p)) possibleMoves.Add(p);//Fazuma lsita de possiveis pecas a serem mexidas
                    }
                }
            }
            int size = possibleMoves.Count;
            var rand = new System.Random();
            p = possibleMoves[rand.Next(size)]; //Escolhe uma peca aleatoriamente do que foi selecionado
            currentPiece = p;
            int direction = rand.Next(2);
            /*Se direction for true ele tenta ir pela direita. Se nao conseguir, ele vai pela esquerda, ja q teoricamente ele tem q conseguir ir pra algum lugar.
             * Se direction for false ele comeca pela esquerda e dps vai pra direita.*/
            if (direction == 1)
            {
                int i = p.GetXPosition(), j = p.GetYPosition();
                if (boardState[i + 1][j - 1] == null)
                {
                    ExecuteMove(p.GetXPosition(), p.GetYPosition(), i + 1, j - 1);
                    EndCurrentPlayerTurn(attackAgain);
                    move = true;
                }
                else
                {
                    ExecuteMove(p.GetXPosition(), p.GetYPosition(), i - 1, j - 1);
                    EndCurrentPlayerTurn(attackAgain);
                    move = true;
                }
            }
            else
            {
                int i = p.GetXPosition(), j = p.GetYPosition();
                if (boardState[i - 1][j - 1] == null)
                {
                    ExecuteMove(p.GetXPosition(), p.GetYPosition(), i - 1, j - 1);
                    EndCurrentPlayerTurn(attackAgain);
                    move = true;
                }
                else
                {
                    ExecuteMove(p.GetXPosition(), p.GetYPosition(), i + 1, j - 1);
                    EndCurrentPlayerTurn(attackAgain);
                    move = true;
                }
            }
        }
    }

    bool CanBlackMove(Piece p)
    {
        int i = p.GetXPosition(), j = p.GetYPosition();
        if (IsPositionInsideBoard(i - 1, j - 1) && boardState[i - 1][j - 1] == null && boardPreparator.IsDarkSquare(i - 1, j - 1)) return true;
        if (IsPositionInsideBoard(i + 1, j - 1) && boardState[i + 1][j - 1] == null && boardPreparator.IsDarkSquare(i + 1, j - 1)) return true;
        return false;
    }

    private bool IsPositionInsideBoard(int v1, int v2)
    {
        return (v1 < 10 && v1 >= 0 && v2 >= 0 && v2 < 10);
    }

    void UpdateCurrentPlayer()
    {
        currentPlayerNumber++;
        if(currentPlayerNumber > playersNumbers)
        {
            currentPlayerNumber = 1;
            currentPlayerText.SetText(playerName);
            Debug.Log("Updating: current player is player " + currentPlayerNumber.ToString());
        }
        else
        {
            currentPlayerText.SetText(computerName);
            Debug.Log("Updating: current player is player " + currentPlayerNumber.ToString());
        }

    }
    public void PieceWasClicked(Piece piece)
    {
        print("CountWhite: "+ countWhitePieces());
        print("CountBlack: "+countBlackPieces());
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
                if (targetPiece && boardPreparator.IsDarkSquare(xWorldPos, yWorldPos)){
                    Debug.Log("Can attack!");
                    ExecuteAttack(currentPiece, targetPiece);
                    ExecuteMove(currentPiece.GetXPosition(), currentPiece.GetYPosition(), xWorldPos, yWorldPos);
                    
                    attackAgain = CanAttackAgain(currentPiece.GetXPosition(), currentPiece.GetYPosition());
                    EndCurrentPlayerTurn(attackAgain);
                }
                else if (!existsAttackingPiece() && CanMoveToSpace(currentPiece, xWorldPos, yWorldPos)){
                    ExecuteMove(currentPiece.GetXPosition(), currentPiece.GetYPosition(), xWorldPos, yWorldPos);
                    EndCurrentPlayerTurn(attackAgain);
                }
            }
            else{
                Debug.Log("That's not your turn funny boy...");
            }
        }

        CheckBoardState();
        
    }

    private bool CanAttackAgain(int x, int y)
    {
        Piece targetPiece1 = null;
        Piece targetPiece2 = null;
        Piece targetPiece3 = null;
        Piece targetPiece4 = null;
        if(y+2<10){
            Debug.Log("X: "+x+" - Y:"+y);
            if(x+2 < 10) if(boardState[x+2][y+2] == null) targetPiece1 = PieceToAttackFromPosition(currentPiece, x+2, y+2);
            if(x-2>=0) if(boardState[x-2][y+2] == null) targetPiece2 = PieceToAttackFromPosition(currentPiece, x-2, y+2);
        }
        if(y-2>=0){
            Debug.Log("X: "+x+" - Y:"+y);
           
            if(x-2 >= 0 && boardState[x-2][y-2] == null) targetPiece3 = PieceToAttackFromPosition(currentPiece, x-2, y-2);
            if(x+2 < 10 && boardState[x+2][y-2] == null) targetPiece4 = PieceToAttackFromPosition(currentPiece, x+2, y-2);
        }
        if (targetPiece1 || targetPiece2 || targetPiece3 || targetPiece4)
        {
            Debug.Log("Can attack again");
            return true;
        }
        return false;
    }

    private bool checkAttacker(Piece p, int x, int y)
    {
       
        Piece targetPiece1 = null;
        Piece targetPiece2 = null;
        Piece targetPiece3 = null;
        Piece targetPiece4 = null;

        if(y+2<10){
           // Debug.Log("X: "+x+" - Y:"+y);
            if(x+2 < 10 && boardState[x+2][y+2] == null) targetPiece1 = PieceToAttackFromPosition(p, x+2, y+2);
            if(x-2>0 && boardState[x-2][y+2] == null) targetPiece2 = PieceToAttackFromPosition(p, x-2, y+2);
        }
        if(y-2>0){
            //Debug.Log("X: "+x+" - Y:"+y);
           
            if(x-2 > 0 && boardState[x-2][y-2] == null) targetPiece3 = PieceToAttackFromPosition(p, x-2, y-2);
            if(x+2 < 10 && boardState[x+2][y-2] == null) targetPiece4 = PieceToAttackFromPosition(p, x+2, y-2);
        }
        if(targetPiece1 || targetPiece2 || targetPiece3 || targetPiece4) return true;

        return false;
    }

    private int[] BotAttacker(Piece p, int x, int y)
    {
        int[] res;
        res = new int[2];
        if (y + 2 < 10)
        {
            Debug.Log("X: " + x + " - Y:" + y);
            if (x + 2 < 10 && boardState[x + 2][y + 2] == null)
            {
                Vector2Int possiblePiece = ExistsAtackedPiece(p, x + 2, y + 2);
                if (possiblePiece.x != 0)
                {
                    res[0] = x + 2;
                    res[1] = y + 2;
                    return res;
                }
            }
            if (x - 2 >= 0 && boardState[x - 2][y + 2] == null)
            {
                Vector2Int possiblePiece = ExistsAtackedPiece(p, x - 2, y + 2);
                if (possiblePiece.x != 0)
                {
                    res[0] = x - 2;
                    res[1] = y + 2;
                    return res;
                }
            }
        }
        if (y - 2 >= 0)
        {
            Debug.Log("X: " + x + " - Y:" + y);

            if (x - 2 >= 0 && boardState[x - 2][y - 2] == null)
            {
                Vector2Int possiblePiece =  ExistsAtackedPiece(p, x - 2, y - 2);
                if (possiblePiece.x != 0)
                {
                    res[0] = x - 2;
                    res[1] = y - 2;
                    return res;
                }
            }
            if (x + 2 < 10 && boardState[x + 2][y - 2] == null)
            {
                Vector2Int possiblePiece = ExistsAtackedPiece(p, x + 2, y - 2);
                if(possiblePiece.x != 0)
                {
                    res[0] = x + 2;
                    res[1] = y - 2;
                    return res;
                }
            }
        }
        return null;
    }
    // Check if you can make a queen etc
    private void CheckBoardState()
    {        
        RemoveCurrentPieceSelection();
    }

    private bool existsAttackingPiece(){
        Piece p;
        bool ret  = false;
        for( int i = 0 ; i<10; i++){
            for( int j= 0; j<10; j++){
                if(((i+j)%2) == 0){
                    p = boardState[i][j];
                    if(p != null && IsWhitePiece(p)) ret = checkAttacker(p, i, j);

                    if(ret){
                        Debug.Log("Há um ataque a se fazer. Procure atentamente...");
                        return true;}
                }
            }
        }
        Debug.Log("Nenhuma possibilidade de ataque, pode se mover se quiser.");
        return false;
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


        Debug.Log("ExecuteMove: Execution of move action from " + xPosOld.ToString() + "," + yPosOld.ToString() + " to "
                                                                 + xPosNew.ToString() + "," + yPosNew.ToString());
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
        //Piece p;

        int x =attackingPiece.GetXPosition(), y = attackingPiece.GetYPosition();

        int casasAndadasX = -x +  xEndPos;
        int dirX = 0;
        if(casasAndadasX > 0) dirX = 1;
        else dirX = -1;
        //casasAndadasX = Math.Abs(casasAndadasX);

        int casasAndadasY = -y +  yEndPos;
        int dirY = 0;
        if(casasAndadasY > 0)   dirY = 1;
        else                    dirY= -1;
        
        bool exists = PieceAtPosExists(x + dirX, y + dirY);
        bool isAnotherColor = PosHasPieceOfDifferentTag(x + dirX, y + dirY, attackingPiece.tag);
        
        if (exists && isAnotherColor) return new Vector2Int(x + dirX, y + dirY);
        else return new Vector2Int(0, 0);

        /*Gente esse codigo ta uma merda com todo o respeito
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
        if (exists) Vector2Int attackingDirection = new Vector2Int(retx, rety);


        if(pecasLinha == pecasOutraCorLinha && pecasOutraCorLinha == 1) return new Vector2Int(retx, rety);
        
        */

    }
    Piece PieceToAttackFromPosition(Piece attackingPiece, int xEndPos, int yEndPos)
    {
        string attackingPieceTag = attackingPiece.tag;
        //Debug.Log("PieceToAttackFromPosition: Can piece of tag " + attackingPieceTag + " attack position?");
        if (!PieceAtPosExists(xEndPos, yEndPos) && boardPreparator.IsDarkSquare(xEndPos, yEndPos))
        {
            Vector2Int attackingPosition = ExistsAtackedPiece(attackingPiece, xEndPos, yEndPos);
            if (attackingPosition.x != 0){
                //Debug.Log("PieceToAttackFromPosition: here is a piece to attack!");
                return boardState[attackingPosition.x][attackingPosition.y];
            }
            else{
                //Debug.Log("PieceToAttackFromPosition: There is NO piece to attack!");
                return null;
            }
        } else
        {
            return null;
        }
    }
    bool PieceAtPosExists(int xPos, int yPos)
    {
        //Debug.Log("PieceAtPosExists: Is there a piece at position X:" + xPos.ToString() + " Y:" +yPos.ToString() + " ?");
        Piece pieceAtPos = boardState[xPos][yPos];
        if (pieceAtPos)
        {
            return true;
        }
        return false;
    }

    bool PosHasPieceOfDifferentTag(int xPos, int yPos, string tagName)
    {
        //Debug.Log("PosHasPieceOfDifferentTag: Position piece with a tag different of the tag: " + tagName + " ?");
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

    public int countBlackPieces(){
        int x = 0;
        bool isBlack = false;
        Piece p;
         for( int i = 0 ; i<10; i++){
            for( int j= 0; j<10; j++){
                if(((i+j)%2) == 0){
                    p = boardState[i][j];
                    if(p != null  ) isBlack = !IsWhitePiece(p);

                    if(isBlack)
                        x++;
                        isBlack = false;
                }
            }
        }
        return x;
    }
    public int countWhitePieces(){
        int x = 0;
        bool isWhite = false;
        Piece p;
         for( int i = 0 ; i<10; i++){
            for( int j= 0; j<10; j++){
                if(((i+j)%2) == 0){
                    p = boardState[i][j];
                    if(p != null  ) isWhite = IsWhitePiece(p);

                    if(isWhite)
                        x++;
                        isWhite = false;
                }
            }
        }
        return x;
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
