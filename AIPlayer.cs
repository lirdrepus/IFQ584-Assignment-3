using System;
using System.Linq;
using System.Collections.Generic;

public class AIPlayer : Player
{
    public AIPlayer(int playerNumber, Rules rules) : base(playerNumber, rules) { }

    public override Move PlayerTurn(List<Board> boardList)
    {
        return FindWin(boardList) ?? RandomMove(boardList);
    }

    // Your Board.SetPiece commits immediately — there's no "would this
    // work?" query — so trialing a move here means actually placing it,
    // asking Rules what happened, then calling board.RemovePiece to put
    // it right back before trying the next candidate.
    public Move? FindWin(List<Board> boardList)
    {
        Move? safeFallback = null;

        for (int boardIndex = 0; boardIndex < boardList.Count; boardIndex++)
        {
            var board = boardList[boardIndex];
            if (!board.IsLive) continue;

            var distinctValues = Rules.GetEligiblePieces(board, PlayerNumber)
                                       .Select(p => p.Value)
                                       .Distinct();

            foreach (var value in distinctValues)
            {
                // Snapshot available spaces once — GetAvaliableSpaces() would
                // otherwise shrink out from under us as we place trial pieces.
                foreach (var space in board.GetAvaliableSpaces().ToList())
                {
                    board.SetPiece(value, space);
                    bool wasLive = board.IsLive; // Rules may flip this (e.g. a misere game) — remember it

                    var candidate = new Move(value, space, this, boardIndex);
                    var outcome = Rules.EvaluateMove(candidate, board);

                    if (outcome == MoveOutcome.MoverWins)
                        return candidate; // keep this placement as-is — it's the move we're taking, don't revert it

                    // Not decisive — undo the trial before evaluating the next candidate.
                    board.RemovePiece(space);
                    board.IsLive = wasLive;

                    if (safeFallback == null && Rules.IsFavorableOutcome(outcome))
                        safeFallback = candidate; // remember the first acceptable move as a fallback
                }
            }
        }

        if (safeFallback != null)
        {
            // The fallback candidate WAS reverted above (we didn't know yet
            // it would be the one we keep) — re-commit it for real now.
            boardList[safeFallback.BoardIndex].SetPiece(safeFallback.PieceValue, safeFallback.Position);
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

        var eligible = Rules.GetEligiblePieces(board, PlayerNumber);
        var value = eligible[random.Next(eligible.Count)].Value;

        board.SetPiece(value, space);
        return new Move(value, space, this, boardIndex);
    }
}
