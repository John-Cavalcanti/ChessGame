using UnityEngine;
using System.Collections.Generic;

public class Pawn : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        int direction = (team == 1) ? 1 : -1;

        // One in front
        if (board[currentX, currentY + direction] == null)
            r.Add(new Vector2Int(currentX, currentY +  direction));

        // Two in front
        if (board[currentX, currentY + direction] == null)
        {
            if (team == 1 && currentY == 1 && board[currentX, currentY + (direction * 2)] == null)
                r.Add(new Vector2Int(currentX, currentY + (direction * 2)));
            if (team == 0 && currentY == 6 && board[currentX, currentY + (direction * 2)] == null)
                r.Add(new Vector2Int(currentX, currentY + (direction * 2)));
        }

        // Kill move
        if (currentX != tileCountX - 1)
            if (board[currentX + 1, currentY + direction] != null && board[currentX + 1, currentY + direction].team != team)
                r.Add(new Vector2Int(currentX + 1, currentY + direction));
        if (currentX != 0)
            if (board[currentX - 1, currentY + direction] != null && board[currentX - 1, currentY + direction].team != team)
                r.Add(new Vector2Int(currentX - 1, currentY + direction));


        return r;
    }

    
    public override SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves)
    {
        int direction = (team == 1) ? 1 : -1;

        // MUDAN�A
        // Verifica��o se o pe�o chegou na ultima casa do tabuleiro para ambos os lados
        // Retorno ser� usado no Script ChessBoard a partir da linha 380
        if ((team == 1 && currentY == 6) || (team == 0 && currentY == 1))
            return SpecialMove.Promotion;

        // En Passsant
        if(moveList.Count > 0)
        {
            Vector2Int[] lastMove = moveList[moveList.Count - 1];
            if(board[lastMove[1].x, lastMove[1].y].type == ChessPieceType.Pawn) // if the last piece moved was a pawn
            {
                if(Mathf.Abs(lastMove[0].y - lastMove[1].y) == 2) // if the last move was a +2 in either direction
                {
                    if(board[lastMove[1].x, lastMove[1].y].team != team) // if the move was from the other team
                    {
                        if ( lastMove[1].y == currentY) // if both pawns are on the same Y
                        {
                            if(lastMove[1].x == currentX - 1) // Landed left
                            {
                                availableMoves.Add(new Vector2Int(currentX - 1, currentY + direction));
                                return SpecialMove.EnPassant;
                            }
                            if(lastMove[1].x == currentX + 1) // Landed right
                            {
                                availableMoves.Add(new Vector2Int(currentX + 1, currentY + direction));
                                return SpecialMove.EnPassant;
                            }
                        }
                    }
                }
            }
        }

        return SpecialMove.None;
    }
}
