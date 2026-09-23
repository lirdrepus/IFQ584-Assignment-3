using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

public class HumanPlayer : Player
{
    public HumanPlayer(int playerNumber, Rules rules) : base(playerNumber, rules) { }

    public override Move PlayerTurn(List<Board> boardList)
    {
        return PromptInput(boardList);
    }

    public Move PromptInput(List<Board> boardList)
    {
        Console.WriteLine($"Player {PlayerNumber}'s turn.");

        int boardIndex = boardList.Count == 1 ? 0 : PromptForBoard(boardList);
        Board board = boardList[boardIndex];

        Point space = PromptForSpace(board);
        int value = PromptForValue(board);

        board.SetPiece(value, space); // your Board commits immediately — there's no separate "confirm" step
        return new Move(value, space, this, boardIndex);
    }

    private int PromptForBoard(List<Board> boardList)
    {
        while (true)
        {
            Console.Write("Choose a board: ");
            if (int.TryParse(Console.ReadLine(), out int index)
                && index >= 0 && index < boardList.Count
                && boardList[index].IsLive)
                return index;
            Console.WriteLine("That board isn't in play. Try again.");
        }
    }

    private Point PromptForSpace(Board board)
    {
        while (true)
        {
            Console.Write("Enter column and row (e.g. \"1 2\"): ");
            var parts = (Console.ReadLine() ?? "").Split(' ');

            if (parts.Length == 2 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
            {
                var space = new Point(x, y);
                try
                {
                    board.CheckSpace(space); // throws PointZeroException / SpaceTakenException
                    return space;
                }
                catch (PointZeroException)
                {
                    Console.WriteLine("0,0 isn't a space on the board. Try again.");
                }
                catch (SpaceTakenException)
                {
                    Console.WriteLine("That space is already taken. Try again.");
                }
                catch (IndexOutOfRangeException)
                {
                    // CheckSpace only guards (0,0) and occupied spaces — anything
                    // outside the board (e.g. x = 10 on a 3x3) surfaces here instead.
                    Console.WriteLine("That space is off the board. Try again.");
                }
            }
            else
            {
                Console.WriteLine("Enter two numbers separated by a space.");
            }
        }
    }

    private int PromptForValue(Board board)
    {
        var eligible = Rules.GetEligiblePieces(board, PlayerNumber);
        while (true)
        {
            Console.Write($"Choose a number ({string.Join(", ", eligible.Select(p => p.Value))}): ");
            if (int.TryParse(Console.ReadLine(), out int value) && eligible.Any(p => p.Value == value))
                return value;
            Console.WriteLine("That number isn't yours to play. Try again.");
        }
    }
}
