// RulesInterface.cs
using System.Collections.Generic;
using System.Drawing;

public interface RulesInterface
{
    Result CheckWin(Move move);
    List<Piece> AvailablePieces(int player);
    List<Piece> CreatePieceSet();
    List<Board> BoardFactory();
}