using System.Collections.Generic;
using System.Drawing;

public class NotaktoRules : Rules
{
    private const int BoardSize = 3;
    private const int BoardCount = 3;

    public NotaktoRules()
    {
        boardList = BoardFactory();
    }

    public override List<Board> BoardFactory()
    {
        List<Board> newBoardList = new List<Board>();
        for (int i = 0; i < BoardCount; i++)
        {
            newBoardList.Add(new Board(BoardSize, CreatePieceSet()));
        }
        return newBoardList;
    }

    //All pieces are the same symbol, just enough X to fill one board
    public override List<Piece> CreatePieceSet()
    {
        List<Piece> pieces = new List<Piece>();
        for (int i = 0; i < BoardSize * BoardSize; i++)
        {
            pieces.Add(new Piece(1, "X"));
        }
        return pieces;
    }


    public override Result CheckWin(Move move)
    {
        Board board = boardList[move.BoardNumber];
        Point space = move.Position;

        if (!board.IsLive) return Result.NotYet; //dead boards can't be re-checked

        bool boardDied = LineFilled(board.GetRow(space))
            || LineFilled(board.GetColumn(space))
            || (space.X == space.Y && LineFilled(board.GetNWDiagonal()))
            || (space.X + space.Y == board.BoardSize + 1 && LineFilled(board.GetNEDiagonal()));

        if (boardDied) board.IsLive = false;

        //Only Loss for the mover once ALL boards are dead
        //GameLoop checks boardList.All(b => !b.IsLive) to decide the real outcome
        return Result.NotYet;
    }

    private bool LineFilled(Piece[] line)
    {
        foreach (Piece piece in line)
        {
            if (piece.Value == 0) return false;
        }
        return true;
    }

    public override List<Piece> AvailablePieces(int player)
    {
        return new List<Piece>(); //not applicable, Notakto pieces aren't owned per player
    }
}