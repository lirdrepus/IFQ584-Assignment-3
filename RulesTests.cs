//Testing file
using System;
using System.Drawing;

public static class RulesTests
{
	public static void RunAll()
	{
		TestNumericalTicTacToeWin();
		TestNumericalTicTacToeNotYet();
		TestNotaktoBoardDies();
		TestGomokuFiveInARow();
		TestGomokuFourInARowNotWin();
		Console.WriteLine("All tests completed.");
	}

	private static void Assert(bool condition, string testName)
	{
		Console.WriteLine(condition ? $"PASS: {testName}" : $"FAIL: {testName}");
	}

	private static void TestNumericalTicTacToeWin()
	{
		var rules = new NumericalTicTacToeRules();
		rules.RulesSetup(3); // 3x3, goal = 3*(9+1)/2 = 15
		var board = rules.BoardList[0];

		// Fill the first row with numbers that sum to 15: 4 + 5 + 6 = 15
		board.SetPiece(4, new Point(1, 1));
		board.SetPiece(5, new Point(1, 2));
		board.SetPiece(6, new Point(1, 3));

		var move = new Move(new Piece(6, "6"), null!, 0, new Point(1, 3));
		Result result = rules.CheckWin(move);

		Assert(result == Result.Win, "NumericalTicTacToe: row sums to 15 -> Win");
	}

	private static void TestNumericalTicTacToeNotYet()
	{
		var rules = new NumericalTicTacToeRules();
		rules.RulesSetup(3);
		var board = rules.BoardList[0];

		board.SetPiece(1, new Point(1, 1)); // put one number, but not enough to sum to 15

		var move = new Move(new Piece(1, "1"), null!, 0, new Point(1, 1));
		Result result = rules.CheckWin(move);

		Assert(result == Result.NotYet, "NumericalTicTacToe: incomplete line -> NotYet");
	}

	private static void TestNotaktoBoardDies()
	{
		var rules = new NotaktoRules();
		rules.RulesSetup();
		var board = rules.BoardList[0];

		board.SetPiece(1, new Point(1, 1));
		board.SetPiece(1, new Point(1, 2));
		board.SetPiece(1, new Point(1, 3)); // Fill a row with X's

		var move = new Move(new Piece(1, "X"), null!, 0, new Point(1, 3));
		rules.CheckWin(move);

		Assert(board.IsLive == false, "Notakto: filled row -> board.IsLive becomes false");
	}

	private static void TestGomokuFiveInARow()
	{
		var rules = new GomokuRules();
		rules.RulesSetup();
		var board = rules.BoardList[0];

		for (int col = 1; col <= 5; col++)
		{
			board.SetPiece(0, new Point(1, col)); // put 5 pieces in a row for player 0
		}

		var move = new Move(new Piece(0, "X"), null!, 0, new Point(1, 5));
		Result result = rules.CheckWin(move);

		Assert(result == Result.Win, "Gomoku: five in a row horizontally -> Win");
	}

	private static void TestGomokuFourInARowNotWin()
	{
		var rules = new GomokuRules();
		rules.RulesSetup();
		var board = rules.BoardList[0];

		for (int col = 1; col <= 4; col++)
		{
			board.SetPiece(0, new Point(1, col)); // put 4 pieces in a row for player 0
		}

		var move = new Move(new Piece(0, "X"), null!, 0, new Point(1, 4));
		Result result = rules.CheckWin(move);

		Assert(result == Result.NotYet, "Gomoku: only four in a row -> NotYet");
	}
}