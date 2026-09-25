using System;
using System.Linq;

// REMOVED: IGameHistory, IGameUi interface

public sealed class GameController
{
    private readonly GameFactory factory;
    private readonly ConsoleUI ui;
    private GameSession? session;

    // REMOVED: SaveLoadHandler and IGameHistory params
    public GameController(GameFactory factory, ConsoleUI ui)
    {
        this.factory = factory;
        this.ui = ui;
    }

    public void Run()
    {
        session = StartNewGame();

        while (session.Outcome == Result.NotYet)
        {
            ShowBoards();
            PlayTurn();
        }

        ShowBoards();
        AnnounceOutcome(session.Outcome, session.CurrentPlayerIndex);
        Console.WriteLine($"Game over: {session.Outcome}");
    }

    private GameSession StartNewGame()
    {
        // TODO: confirm exact prompt wording/flow ¡ª ConsoleUI only
        // exposes PromptInteger(prompt) right now, not the SelectGame()/
        // SelectBoardSizeIfRequired()/SelectComputerMode() methods Kevin's
        // original IGameUi assumed. Using PromptInteger directly as a stopgap.
        int gameChoice = ui.PromptInteger("Choose a game: 1) Numerical TicTacToe 2) Notakto 3) Gomoku");
        int boardSize = gameChoice == 1
            ? ui.PromptInteger("Enter board size:")
            : 0; // ignored by Notakto/Gomoku
        bool computerOpponent = ui.PromptInteger("1) Human vs Human  2) Human vs Computer") == 2;

        return factory.Create(gameChoice, boardSize, computerOpponent);
    }

    private void ShowBoards()
    {
        RenderEngine.DrawAll(session!.Boards);
    }

    private void PlayTurn()
    {
        Player player = session!.Players[session.CurrentPlayerIndex];
        Move move = player.PlayerTurn(session.Boards);

        HistoryEngine.Instance.RecordMove(move.MyPiece, move.MyPlayer, move.BoardNumber, move.MovePosition);

        Result outcome = session.Rules.CheckWin(move);

        // Notakto-specific: CheckWin only ever flips IsLive on the board it just
        // checked and returns NotYet - the "misere" ending (all boards dead) can
        // only be detected by looking across every board, not from one CheckWin
        // call. This aggregation has to live here, not in NotaktoRules, since
        // NotaktoRules.CheckWin only ever sees one board at a time.
        if (outcome == Result.NotYet && session.Boards.All(b => !b.IsLive))
        {
            outcome = Result.Loss; // the player who just moved loses (misere rule)
        }

        if (outcome != Result.NotYet)
        {
            AnnounceOutcome(outcome, session.CurrentPlayerIndex);
            session.Outcome = outcome;
            return;
        }

        if (NoMovesRemain())
        {
            session.Outcome = Result.Draw;
            Console.WriteLine("It's a draw!");
            return;
        }

        session.CurrentPlayerIndex = (session.CurrentPlayerIndex + 1) % session.Players.Length;
    }

    // Result is expressed relative to the player who just moved (Win = mover
    // wins, Loss = mover loses - this is what lets NotaktoRules flip the
    // semantics polymorphically without Game knowing it's Notakto). This method
    // is where that gets translated into a message naming the actual winner.
    private void AnnounceOutcome(Result outcome, int moverIndex)
    {
        int moverPlayerNumber = session!.Players[moverIndex].PlayerNumber;
        int otherIndex = (moverIndex + 1) % session.Players.Length;
        int otherPlayerNumber = session.Players[otherIndex].PlayerNumber;

        if (outcome == Result.Win)
            Console.WriteLine($"Player {moverPlayerNumber} wins!");
        else if (outcome == Result.Loss)
            Console.WriteLine($"Player {otherPlayerNumber} wins! (Player {moverPlayerNumber} made the losing move)");
    }

    private bool NoMovesRemain()
    {
        return session!.Boards.All(board => board.GetAvaliableSpaces().Count == 0);
    }

    // REMOVED: HandleCommand() switch for SAVE/LOAD/UNDO/REDO/HELP/QUIT
}