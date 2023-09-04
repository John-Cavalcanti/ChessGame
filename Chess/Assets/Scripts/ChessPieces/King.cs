using UnityEngine;
using System.Collections.Generic;

public class King : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        //Right 
        if (currentX + 1 < tileCountX)
        {
            //Right
            if (board[currentX + 1, currentY] == null)
                r.Add(new Vector2Int(currentX + 1, currentY));
            else if (board[currentX + 1, currentY].team != team)
                r.Add(new Vector2Int(currentX + 1, currentY));

            // Top Right
            if (currentY + 1 < tileCountY)
            {
                if (board[currentX + 1, currentY + 1] == null)
                    r.Add(new Vector2Int(currentX + 1, currentY + 1));
                else if (board[currentX + 1, currentY + 1].team != team)
                    r.Add(new Vector2Int(currentX + 1, currentY+ 1));
            }

            // Bottom Right
            if (currentY - 1 >= 0)
            {
                if (board[currentX + 1, currentY - 1] == null)
                    r.Add(new Vector2Int(currentX + 1, currentY - 1));
                else if (board[currentX + 1, currentY - 1].team != team)
                    r.Add(new Vector2Int(currentX + 1, currentY - 1));
            }
        }


        // Left
        if (currentX - 1 >= 0)
        {
            //left
            if (board[currentX - 1, currentY] == null)
                r.Add(new Vector2Int(currentX - 1, currentY));
            else if (board[currentX - 1, currentY].team != team)
                r.Add(new Vector2Int(currentX - 1, currentY));

            // Top Left
            if (currentY + 1 < tileCountY)
            {
                if (board[currentX - 1, currentY + 1] == null)
                    r.Add(new Vector2Int(currentX - 1, currentY + 1));
                else if (board[currentX - 1, currentY + 1].team != team)
                    r.Add(new Vector2Int(currentX - 1, currentY + 1));
            }

            // Bottom Left
            if (currentY - 1 >= 0)
            {
                if (board[currentX - 1, currentY - 1] == null)
                    r.Add(new Vector2Int(currentX - 1, currentY - 1));
                else if (board[currentX - 1, currentY - 1].team != team)
                    r.Add(new Vector2Int(currentX - 1, currentY - 1));
            }
        }

        // Up
        if (currentY + 1 < tileCountY)
            if ((board[currentX, currentY + 1] == null || board[currentX, currentY + 1].team != team))
                r.Add(new Vector2Int(currentX, currentY + 1));

        // Down
        if (currentY - 1 >= 0)
            if ((board[currentX, currentY - 1] == null || board[currentX, currentY - 1].team != team))
                r.Add(new Vector2Int(currentX, currentY - 1));

        return r;
    }

    // MUDANÇA
    // Função para o movimento especial do Rei
    // Retorno será usado no Script ChessBoard a partir da linha 380
    public override SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves)
    {

        SpecialMove r = SpecialMove.None;

        // verifica se as peças Rei e as duas Torres já realizaram algum movimento, para os dois lados
        var kingMove = moveList.Find(m => m[0].x == 3 && m[0].y == ((team == 1) ? 0 : 7));
        var leftRook = moveList.Find(m => m[0].x == 0 && m[0].y == ((team == 1) ? 0 : 7));
        var rightRook = moveList.Find(m => m[0].x == 7 && m[0].y == ((team == 1) ? 0 : 7));

        // Caso ainda não tenha feito um movimento 
        if (kingMove == null && currentX == 3)
        {
            // Time Branco
            if (team == 1)
            {
                // leftRook
                // Caso a torre não tenha movido e ainda esteja na casa inicial
                if (leftRook == null)
                    if (board[0, 0].type == ChessPieceType.Rook)
                        // Verifica se as casas entre o rei e a torre estejam vazias
                        if (board[0, 0].team == 1)
                            if (board[2, 0] == null)
                                if (board[1, 0] == null)
                                {
                                    // adiciona o novo opção de movimento na lista e retorna que houve e movimento especial
                                    availableMoves.Add(new Vector2Int(1, 0));
                                    r = SpecialMove.Castling;
                                }
                //rightRook
                // Caso a torre não tenha movido e ainda esteja na casa inicial
                if (rightRook == null)
                    if (board[7, 0].type == ChessPieceType.Rook)
                        // Verifica se as casas entre o rei e a torre estejam vazias
                        // Esse lado do tabuleiro possui uma casa para verificar a mais por causa da Rainha
                        if (board[7, 0].team == 1)
                            if (board[6, 0] == null)
                                if (board[5, 0] == null)
                                    if (board[4, 0] == null)
                                    {
                                        // adiciona o novo opção de movimento na lista e retorna que houve e movimento especial
                                        availableMoves.Add(new Vector2Int(5, 0));
                                        r = SpecialMove.Castling;
                                    }
            }
            // Time Preto
            else
            {
                // leftRook
                // Caso a torre não tenha movido e ainda esteja na casa inicial
                if (leftRook == null)
                    if (board[0, 7].type == ChessPieceType.Rook)
                        if (board[0, 7].team == 0)
                            // Verifica se as casas entre o rei e a torre estejam vazias
                            if (board[2, 7] == null)
                                if (board[1, 7] == null)
                                {
                                    // adiciona o novo opção de movimento na lista e retorna que houve e movimento especial
                                    availableMoves.Add(new Vector2Int(1, 7));
                                    r = SpecialMove.Castling;
                                }
                //rightRook
                // Caso a torre não tenha movido e ainda esteja na casa inicial
                if (rightRook == null)
                    if (board[7, 7].type == ChessPieceType.Rook)
                        if (board[7, 7].team == 0)
                            // Verifica se as casas entre o rei e a torre estejam vazias
                            // Esse lado do tabuleiro possui uma casa para verificar a mais por causa da Rainha
                            if (board[6, 7] == null)
                                if (board[5, 7] == null)
                                    if (board[4, 7] == null)
                                    {
                                        // adiciona o novo opção de movimento na lista e retorna que houve e movimento especial
                                        availableMoves.Add(new Vector2Int(5, 7));
                                        r = SpecialMove.Castling;
                                    }
            }
        }


        return r;
    }

}
