using System;
using System.Drawing;
using System.Collections.Generic;

public class HistoryEngine
{
    private static HistoryEngine? instance;
    public static HistoryEngine Instance
    {
        get
        {
            instance ??= new HistoryEngine();
            return instance;
        }
    }

    private List<Move> moveHistory = new List<Move>();
    private List<Move> redoHistory = new List<Move>();

    public List<Board>? BoardList { get; set; }

    public void RecordMove(Piece piece, Player player, int boardNumber, Point position)
    {
        Move newMove = MoveFactory(piece, player, boardNumber, position);
        moveHistory.Add(newMove);
        redoHistory.Clear();
    }

    private Move MoveFactory(Piece piece, Player player, int boardNumber, Point position)
    {
        return new Move(piece, player, boardNumber, position);
    }

    public bool Undo()
    {
        if (BoardList == null || moveHistory.Count == 0) return false;
        Move lastMove = moveHistory[^1];
        Board board = BoardList[lastMove.BoardNumber];
        board.RemovePiece(lastMove.MovePosition);
        moveHistory.Remove(lastMove);
        redoHistory.Add(lastMove);
        return true;
    }

    public bool Redo()
    {
        if (BoardList == null || redoHistory.Count == 0) return false;
        Move redoMove = redoHistory[^1];
        Board board = BoardList[redoMove.BoardNumber];
        board.SetPiece(redoMove.MyPiece.Value, redoMove.MovePosition);
        redoHistory.Remove(redoMove);
        moveHistory.Add(redoMove);
        return true;
    }
}