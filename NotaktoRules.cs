using System.Collections.Generic;
using System.Drawing;

public class NotaktoRules : Rules
{
    public override List<Board> BoardFactory()
    {
        List<Board> newBoardList = new List<Board>();
        for (int i = 0; i < 3; i++) //Notakto always uses exactly 3 boards
        {
            newBoardList.Add(new Board(3, CreatePieceSet())); //TODO: confirm Board ctor, does each board need its own piece pool, or one shared piece since supply is unlimited?
        }
        return newBoardList;
    }

    public NotaktoRules()
    {
        boardList = BoardFactory();
    }

    public override List<Piece> CreatePieceSet()
    {
        //All pieces are the same symbol, not a depleting pool like NTTT.
        //TODO: confirm with Sean how Board.SetPiece should work for an unlimited-supply piece rather than one drawn from a finite list.
        return new List<Piece> { new Piece(1, "X") };
    }

    public override Result CheckWin(int boardNumber, Point space)
    {
        Board board = boardList[boardNumber];
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