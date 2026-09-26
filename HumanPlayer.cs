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
        Console.WriteLine("(Type HELP, SAVE, LOAD, UNDO or REDO at any prompt.)");

        int boardIndex = boardList.Count == 1 ? 0 : PromptForBoard(boardList);
        Board board = boardList[boardIndex];

        Point space = PromptForSpace(board);
        int value = PromptForValue(board);

        board.SetPiece(value, space);
        var piece = new Piece(value, value.ToString());
        return new Move(piece, this, boardIndex, space);
    }

    // Cecil prompts kept; route UI commands so LOAD can abort via SessionReplacedException.
    private static string? ReadLineAllowingCommands()
    {
        while (true)
        {
            string? line = Console.ReadLine();
            if (ConsoleUI.TryHandleActiveCommand(line))
                continue;
            return line;
        }
    }

    private int PromptForBoard(List<Board> boardList)
    {
        while (true)
        {
            Console.Write($"Choose a board (1-{boardList.Count}): ");
            if (int.TryParse(ReadLineAllowingCommands(), out int input)
                && input >= 1 && input <= boardList.Count
                && boardList[input - 1].IsLive)
                return input - 1;
            Console.WriteLine("That board isn't in play. Try again.");
        }
    }

    private Point PromptForSpace(Board board)
    {
        while (true)
        {
            Console.Write("Enter column and row (e.g. \"1 2\"): ");
            var parts = (ReadLineAllowingCommands() ?? "").Split(' ');

            if (parts.Length == 2 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
            {
                var space = new Point(x, y);
                try
                {
                    board.CheckSpace(space);
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
                    Console.WriteLine("That space is off the board. Try again.");
                }
            }
            else
            {
                Console.WriteLine("Enter two numbers separated by a space.");
            }
        }
    }

    // Notakto/Gomoku: infer piece from pool when AvailablePieces is empty (Cecil).
    private int PromptForValue(Board board)
    {
        var eligible = Rules.AvailablePieces(PlayerNumber);

        if (eligible.Count == 0)
        {
            var distinctValues = board.Pieces.Select(p => p.Value).Distinct().OrderBy(v => v).ToList();
            return distinctValues.Count == 1
                ? distinctValues[0]
                : distinctValues[(PlayerNumber - 1) % distinctValues.Count];
        }

        while (true)
        {
            Console.Write($"Choose a number ({string.Join(", ", eligible.Select(p => p.Value))}): ");
            if (int.TryParse(ReadLineAllowingCommands(), out int value) && eligible.Any(p => p.Value == value))
                return value;
            Console.WriteLine("That number isn't yours to play. Try again.");
        }
    }
}
