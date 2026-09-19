using System.Collections.Generic;
using System.Drawing;

public class GomokuRules : Rules
{
	private const int WinLength = 5;
	private readonly int boardSize;

	private static readonly (int rowStep, int colStep)[] Directions =
	{
		(0, 1), (1, 0), (1, 1), (1, -1)
	};

	public GomokuRules(int boardSize)
	{
		this.boardSize = boardSize;
		boardList = BoardFactory();
	}

	public override List<Board> BoardFactory()
	{
		List<Board> newBoardList = new List<Board>();
		newBoardList.Add(new Board(boardSize, CreatePieceSet()));
		return newBoardList;
	}

	public override List<Piece> CreatePieceSet()
	{
		//Two unlimited-supply piece types, not a depleting pool.
		//TODO: same question as Notakto, confirm shape with Sean
		return new List<Piece> { new Piece(1, "X"), new Piece(2, "O") };
	}

	public override Result CheckWin(int boardNumber, Point space)
	{
		Board board = boardList[0];
		Piece placed = board.GetPiece(space);

		foreach (var (rowStep, colStep) in Directions)
		{
			int count = 1;
			count += CountDirection(board, space, rowStep, colStep, placed.Value);
			count += CountDirection(board, space, -rowStep, -colStep, placed.Value);
			if (count >= WinLength) return Result.Win;
		}
		return Result.NotYet;
	}

	private int CountDirection(Board board, Point from, int rowStep, int colStep, int pieceValue)
	{
		int count = 0;
		int row = from.X + rowStep;
		int col = from.Y + colStep;
		while (row >= 1 && row <= board.BoardSize && col >= 1 && col <= board.BoardSize)
		{
			Point space = new Point(row, col);
			if (board.GetPiece(space).Value != pieceValue) break;
			count++;
			row += rowStep;
			col += colStep;
		}
		return count;
	}

	public override List<Piece> AvailablePieces(int player)
	{
		return new List<Piece>(); //not applicable, from Sean's message
	}
}