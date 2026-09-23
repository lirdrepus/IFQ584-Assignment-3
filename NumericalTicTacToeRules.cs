using System.Drawing;

public class NumericalTicTacToeRules : Rules
{
    private int boardSize;
    private int goal; //Target sum, formula n(n^2+1)/2
    public override bool CustomBoard => true;

    //boardSize comes in from GameFactory/setup, NOT prompted here
    //Rules shouldn't be doing console I/O, that's ConsoleUI's job
    /*public NumericalTicTacToeRules(int boardSize)
    {
        this.boardSize = boardSize;
        goal = boardSize * (boardSize * boardSize + 1) / 2;
        boardList = BoardFactory(); //safe: boardSize is already set above
    }*/
    public NumericalTicTacToeRules() { } // no longer builds boardList here
    public override string GameName => "Numerical Tic-Tac-Toe";
    public override string GameDescription => $"Get a row, column, or diagonal to sum to {goal} to win.";

    public override void RulesSetup(int boardSize = 0)
    {
        this.boardSize = boardSize;
        goal = boardSize * (boardSize * boardSize + 1) / 2;
        boardList = BoardFactory(); // now called after setup confirms the board size
    }

    public override List<Board> BoardFactory()
    {
        List<Board> newBoardList = new List<Board>(); //Sean's example left this null -- fixed
        Board newBoard = new Board(boardSize, CreatePieceSet());
        newBoardList.Add(newBoard);
        return newBoardList;
    }

    public override List<Piece> CreatePieceSet()
    {
        //Sean's example loop only went 1..n (missed n*n pieces) -- fixed to boardSize*boardSize
        List<Piece> pieces = new List<Piece>();
        for (int x = 1; x <= boardSize * boardSize; x++)
        {
            pieces.Add(new Piece(x, $"{x}")); //renderValue is just the number as a string
        }
        return pieces;
    }

    public override Result CheckWin(Move move)
    {
        Board board = boardList[0];
        Point space = move.MovePosition;

        if (LineWins(board.GetRow(space))) return Result.Win;
        if (LineWins(board.GetColumn(space))) return Result.Win;

        if (space.X == space.Y)
        {
            if (LineWins(board.GetNWDiagonal())) return Result.Win;
        }
        if (space.X + space.Y == board.BoardSize + 1)
        {
            if (LineWins(board.GetNEDiagonal())) return Result.Win;
        }

        return Result.NotYet;
    }

    private bool LineWins(Piece[] line)
    {
        int total = 0;
        foreach (Piece piece in line)
        {
            if (piece == null) return false;
            total += piece.Value;
        }
        return total == goal;
    }

    public override List<Piece> AvailablePieces(int player)
    {
        List<Piece> pieces = boardList[0].Pieces; //unplaced pool
        List<Piece> available = new List<Piece>();
        foreach (Piece piece in pieces)
        {
            if (piece.Value != 0 && System.Int32.IsOddInteger(player) == System.Int32.IsOddInteger(piece.Value))
            {
                available.Add(piece);
            }
        }
        return available;
    }
}