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

    // Read-only undo/redo stacks for GameStateMapper.Capture.
    public IReadOnlyList<Move> AppliedMoves => moveHistory;
    public IReadOnlyList<Move> RedoMoves => redoHistory;

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

    public void ClearHistory()
    {
        moveHistory.Clear();
        redoHistory.Clear();
    }

    // Replace both stacks for GameStateMapper.Restore.
    public void ReplaceHistory(IEnumerable<Move> applied, IEnumerable<Move> redo)
    {
        moveHistory = new List<Move>(applied);
        redoHistory = new List<Move>(redo);
    }

    public bool Undo()
    {
        if (BoardList == null || moveHistory.Count == 0) return false;
        Move lastMove = moveHistory[^1];
        Board board = BoardList[lastMove.BoardNumber];
        board.RemovePiece(lastMove.MovePosition);
        // Notakto: undoing a killing move revives the board (harmless if already live).
        board.IsLive = true;
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
        // Redo does not re-run CheckWin (Notakto IsLive; see notes).
        redoHistory.Remove(redoMove);
        moveHistory.Add(redoMove);
        return true;
    }
}
