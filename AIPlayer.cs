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

    public Move? FindWin(List<Board> boardList)
    {
        Move? safeFallback = null;

        for (int boardIndex = 0; boardIndex < boardList.Count; boardIndex++)
        {
            var board = boardList[boardIndex];
            if (!board.IsLive) continue;

            var distinctValues = Rules.AvailablePieces(PlayerNumber)
                                       .Select(p => p.Value)
                                       .Distinct();

            foreach (var value in distinctValues)
            {
                foreach (var space in board.GetAvaliableSpaces().ToList())
                {
                    board.SetPiece(value, space);
                    bool wasLive = board.IsLive;

                    var piece = new Piece(value, value.ToString()); // TODO: confirm renderValue source with Sean
                    var candidate = new Move(piece, this, boardIndex, space);
                    Result outcome = Rules.CheckWin(candidate);

                    if (outcome == Result.Win)
                        return candidate;

                    board.RemovePiece(space);
                    board.IsLive = wasLive;

                    // TODO: "favorable" originally meant something more specific -
                    // using "not a loss" as the safe-fallback condition for now
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

        var eligible = Rules.AvailablePieces(PlayerNumber);
        var value = eligible[random.Next(eligible.Count)].Value;

        board.SetPiece(value, space);
        var piece = new Piece(value, value.ToString());
        return new Move(piece, this, boardIndex, space);
    }
}