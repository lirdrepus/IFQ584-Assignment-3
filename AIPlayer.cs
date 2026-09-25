using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

public class AIPlayer : Player
{
    public AIPlayer(int playerNumber, Rules rules) : base(playerNumber, rules) { }

    public override Move PlayerTurn(List<Board> boardList)
    {
        return FindWin(boardList) ?? RandomMove(boardList);
    }

    // Notakto/Gomoku don't assign pieces per player (AvailablePieces returns
    // empty for them) - infer from the pool instead: one distinct value =
    // shared piece (Notakto's X); two = alternate by player number (Gomoku's X/O).
    // Same logic as HumanPlayer.PromptForValue(), kept in sync deliberately.
    private List<int> EligibleValues(Board board)
    {
        var eligible = Rules.AvailablePieces(PlayerNumber);
        if (eligible.Count > 0)
            return eligible.Select(p => p.Value).Distinct().ToList();

        var distinctValues = board.Pieces.Select(p => p.Value).Distinct().OrderBy(v => v).ToList();
        if (distinctValues.Count == 0) return distinctValues; // board's piece pool is empty

        return distinctValues.Count == 1
            ? new List<int> { distinctValues[0] }
            : new List<int> { distinctValues[(PlayerNumber - 1) % distinctValues.Count] };
    }

    public Move? FindWin(List<Board> boardList)
    {
        Move? safeFallback = null;

        for (int boardIndex = 0; boardIndex < boardList.Count; boardIndex++)
        {
            var board = boardList[boardIndex];
            if (!board.IsLive) continue;

            var distinctValues = EligibleValues(board); // Rules.AvailablePieces(...).Select(...)

            foreach (var value in distinctValues)
            {
                foreach (var space in board.GetAvaliableSpaces().ToList())
                {
                    board.SetPiece(value, space);
                    bool wasLive = board.IsLive;

                    var piece = new Piece(value, value.ToString());
                    var candidate = new Move(piece, this, boardIndex, space);
                    Result outcome = Rules.CheckWin(candidate);

                    if (outcome == Result.Win)
                        return candidate;

                    board.RemovePiece(space);
                    board.IsLive = wasLive;

                    if (safeFallback == null && outcome != Result.Loss)
                        safeFallback = candidate;
                }
            }
        }

        if (safeFallback != null)
        {
            boardList[safeFallback.BoardNumber].SetPiece(safeFallback.MyPiece.Value, safeFallback.MovePosition);
        }

        return safeFallback;
    }

    public Move RandomMove(List<Board> boardList)
    {
        var random = new Random();

        var liveBoardIndices = Enumerable.Range(0, boardList.Count)
            .Where(i => boardList[i].IsLive && boardList[i].GetAvaliableSpaces().Count > 0)
            .ToList();

        int boardIndex = liveBoardIndices[random.Next(liveBoardIndices.Count)];
        var board = boardList[boardIndex];

        var spaces = board.GetAvaliableSpaces();
        var space = spaces[random.Next(spaces.Count)];

        var eligibleValues = EligibleValues(board); // Rules.AvailablePieces(...)
        var value = eligibleValues[random.Next(eligibleValues.Count)];

        board.SetPiece(value, space);
        var piece = new Piece(value, value.ToString());
        return new Move(piece, this, boardIndex, space);
    }
}