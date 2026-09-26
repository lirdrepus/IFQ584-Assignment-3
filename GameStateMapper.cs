using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

// IGameStateMapper for Terry's SaveLoadHandler DTOs (A2 save/restore; Kevin).
public sealed class GameStateMapper : IGameStateMapper
{
    public SavedGameState Capture(GameSession session)
    {
        var snapshot = new SavedGameState
        {
            GameName = session.Rules.GameName,
            CurrentPlayerIndex = session.CurrentPlayerIndex,
            Outcome = session.Outcome,
            Boards = session.Boards.Select(CaptureBoard).ToList(),
            Players = session.Players.Select(p => new SavedPlayerState
            {
                PlayerNumber = p.PlayerNumber,
                PlayerType = p is AIPlayer ? nameof(AIPlayer) : nameof(HumanPlayer)
            }).ToList(),
            AppliedMoves = HistoryEngine.Instance.AppliedMoves.Select(CaptureMove).ToList(),
            RedoMoves = HistoryEngine.Instance.RedoMoves.Select(CaptureMove).ToList()
        };
        return snapshot;
    }

    public GameSession Restore(SavedGameState snapshot)
    {
        if (snapshot.Boards.Count == 0)
            throw new InvalidDataException("Save file has no boards.");

        Rules rules = CreateRules(snapshot.GameName);
        int boardSize = snapshot.Boards[0].Size;
        rules.RulesSetup(rules.CustomBoard ? boardSize : 0);

        // Overlay saved cells/IsLive onto boards from RulesSetup.
        List<Board> boards = rules.BoardList;
        if (boards.Count != snapshot.Boards.Count)
            throw new InvalidDataException(
                $"Board count mismatch for {snapshot.GameName}: expected {snapshot.Boards.Count}, got {boards.Count}.");

        for (int i = 0; i < boards.Count; i++)
            ApplyBoardState(boards[i], snapshot.Boards[i]);

        Player[] players = snapshot.Players.Select(p => CreatePlayer(p, rules)).ToArray();
        if (players.Length == 0)
            throw new InvalidDataException("Save file has no players.");

        var session = new GameSession(rules, boards, players)
        {
            CurrentPlayerIndex = snapshot.CurrentPlayerIndex,
            Outcome = snapshot.Outcome
        };

        // Best-effort history restore (see KEVIN_SAVE_UNDO_NOTES.md for Notakto limits).
        HistoryEngine.Instance.BoardList = session.Boards;
        HistoryEngine.Instance.ReplaceHistory(
            snapshot.AppliedMoves.Select(m => RestoreMove(m, session)),
            snapshot.RedoMoves.Select(m => RestoreMove(m, session)));

        return session;
    }

    private static Rules CreateRules(string gameName)
    {
        // Match concrete Rules.GameName strings.
        if (gameName == "Numerical Tic-Tac-Toe")
            return new NumericalTicTacToeRules();
        if (gameName == "Notakto")
            return new NotaktoRules();
        if (gameName == "Gomoku")
            return new GomokuRules();

        throw new InvalidDataException($"Unknown game name in save file: '{gameName}'.");
    }

    private static Player CreatePlayer(SavedPlayerState saved, Rules rules)
    {
        if (string.Equals(saved.PlayerType, nameof(AIPlayer), StringComparison.OrdinalIgnoreCase)
            || string.Equals(saved.PlayerType, "AI", StringComparison.OrdinalIgnoreCase))
        {
            return new AIPlayer(saved.PlayerNumber, rules);
        }

        return new HumanPlayer(saved.PlayerNumber, rules);
    }

    private static SavedBoardState CaptureBoard(Board board)
    {
        var saved = new SavedBoardState
        {
            Size = board.BoardSize,
            IsLive = board.IsLive
        };

        // Point: X = column, Y = row (1-based), same as HumanPlayer/Move.
        for (int row = 1; row <= board.BoardSize; row++)
        {
            for (int col = 1; col <= board.BoardSize; col++)
            {
                Piece piece = board.GetPiece(new Point(col, row));
                if (piece == null) continue;

                saved.Cells.Add(new SavedCellState
                {
                    Row = row,
                    Column = col,
                    PieceValue = piece.Value,
                    RenderValue = piece.RenderValue
                });
            }
        }

        return saved;
    }

    private static void ApplyBoardState(Board board, SavedBoardState saved)
    {
        foreach (SavedCellState cell in saved.Cells)
        {
            board.SetPiece(cell.PieceValue, new Point(cell.Column, cell.Row));
        }

        board.IsLive = saved.IsLive;
    }

    private static SavedMoveState CaptureMove(Move move)
    {
        return new SavedMoveState
        {
            BoardNumber = move.BoardNumber,
            Row = move.MovePosition.Y,
            Column = move.MovePosition.X,
            PieceValue = move.MyPiece.Value,
            PlayerNumber = move.MyPlayer.PlayerNumber
        };
    }

    private static Move RestoreMove(SavedMoveState saved, GameSession session)
    {
        Player player = session.Players.FirstOrDefault(p => p.PlayerNumber == saved.PlayerNumber)
            ?? session.Players[0];

        // SavedMoveState has no RenderValue; prefer on-board piece, else value string.
        string render = saved.PieceValue.ToString();
        Board board = session.Boards[saved.BoardNumber];
        Piece? onBoard = board.GetPiece(new Point(saved.Column, saved.Row));
        if (onBoard != null && onBoard.Value == saved.PieceValue)
            render = onBoard.RenderValue;

        var piece = new Piece(saved.PieceValue, render);
        return new Move(piece, player, saved.BoardNumber, new Point(saved.Column, saved.Row));
    }
}
