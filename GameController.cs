using System;
using System.IO;
using System.Linq;

// Terry play loop kept; Kevin wires save/load/undo/help (Slack #assignment-3).

public sealed class GameController
{
    public const string DefaultSavePath = "save.json";

    private readonly GameFactory factory;
    private readonly ConsoleUI ui;
    private readonly SaveLoadHandler saveLoad;
    private GameSession? session;
    private GameSession? pendingLoaded;
    private bool inPlayLoop;

    public GameController(GameFactory factory, ConsoleUI ui, SaveLoadHandler saveLoad)
    {
        this.factory = factory;
        this.ui = ui;
        this.saveLoad = saveLoad;
        ui.Bind(this);
    }

    public GameSession? Session => session;

    public void Run()
    {
        inPlayLoop = false;
        session = PromptStartMenu();
        inPlayLoop = true;

        while (session.Outcome == Result.NotYet)
        {
            try
            {
                ShowBoards();
                PlayTurn();
            }
            catch (SessionReplacedException)
            {
                // Mid-turn LOAD: continue loop with the loaded session.
            }
        }

        ShowBoards();
        AnnounceOutcome(session.Outcome, session.CurrentPlayerIndex);
        Console.WriteLine($"Game over: {session.Outcome}");
    }

    // Assignment 2: begin with load-or-fresh before setup prompts.
    private GameSession PromptStartMenu()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=== Start Menu ===");
            Console.WriteLine("1) New game");
            Console.WriteLine("2) Load game");
            Console.WriteLine("(Type 1/NEW, 2/LOAD, HELP, or QUIT)");
            Console.Write("Choice: ");
            string? line = Console.ReadLine();

            if (ui.TryHandleCommand(line))
            {
                if (pendingLoaded != null)
                    return ConsumePendingLoaded();
                continue;
            }

            string key = (line ?? "").Trim().ToUpperInvariant();
            if (key == "1" || key == "NEW")
                return StartNewGame();

            if (key == "2" || key == "LOAD")
            {
                if (TryLoadAtStart())
                    return ConsumePendingLoaded();
                continue;
            }

            Console.WriteLine("Please choose 1 (New game) or 2 (Load game).");
        }
    }

    // Load via SaveLoadHandler; missing/invalid save stays on the start menu.
    private bool TryLoadAtStart(string path = DefaultSavePath)
    {
        try
        {
            LoadGame(path);
            return pendingLoaded != null;
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"No save file found at '{path}'. Choose New game or try Load again.");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not load save: {ex.Message}");
            return false;
        }
    }

    private GameSession StartNewGame()
    {
        int gameChoice = ui.PromptInteger("Choose a game: 1) Numerical TicTacToe 2) Notakto 3) Gomoku");
        if (pendingLoaded != null) return ConsumePendingLoaded();

        int boardSize = gameChoice == 1
            ? ui.PromptInteger("Enter board size:")
            : 0;
        if (pendingLoaded != null) return ConsumePendingLoaded();

        bool computerOpponent = ui.PromptInteger("1) Human vs Human  2) Human vs Computer") == 2;
        if (pendingLoaded != null) return ConsumePendingLoaded();

        GameSession created = factory.Create(gameChoice, boardSize, computerOpponent);
        HistoryEngine.Instance.ClearHistory();
        HistoryEngine.Instance.BoardList = created.Boards;
        return created;
    }

    private GameSession ConsumePendingLoaded()
    {
        GameSession loaded = pendingLoaded!;
        pendingLoaded = null;
        HistoryEngine.Instance.BoardList = loaded.Boards;
        return loaded;
    }

    public void ShowBoards()
    {
        if (session == null) return;
        RenderEngine.DrawAll(session.Boards);
    }

    private void PlayTurn()
    {
        Player player = session!.Players[session.CurrentPlayerIndex];
        Move move = player.PlayerTurn(session.Boards);

        HistoryEngine.Instance.RecordMove(move.MyPiece, move.MyPlayer, move.BoardNumber, move.MovePosition);

        Result outcome = session.Rules.CheckWin(move);

        // Notakto: aggregate dead boards here (CheckWin sees one board only; Terry).
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

    // Outcome is relative to the mover (Notakto flips Win/Loss); name the real winner here (Terry).
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

    public void ShowHelp()
    {
        if (session != null)
        {
            Console.WriteLine($"--- {session.Rules.GameName} ---");
            Console.WriteLine(session.Rules.GameDescription);
        }
        else
        {
            Console.WriteLine("No game in progress yet. Choose a game to see its rules.");
        }

        Console.WriteLine("Commands: HELP, SAVE, LOAD, UNDO, REDO, QUIT");
        Console.WriteLine($"SAVE/LOAD use '{DefaultSavePath}' in the working directory.");
    }

    public void SaveGame(string path = DefaultSavePath)
    {
        if (session == null)
        {
            Console.WriteLine("Nothing to save. Start or load a game first.");
            return;
        }

        saveLoad.Save(path, session);
        Console.WriteLine($"Game saved to {path}.");
    }

    public void LoadGame(string path = DefaultSavePath)
    {
        GameSession loaded = saveLoad.Load(path);
        HistoryEngine.Instance.BoardList = loaded.Boards;
        Console.WriteLine($"Game loaded from {path}.");

        if (!inPlayLoop)
        {
            pendingLoaded = loaded;
            // Start menu consumes pending immediately; mid-setup prompts still check pendingLoaded.
            return;
        }

        session = loaded;
        ShowBoards();
        throw new SessionReplacedException();
    }

    public void UndoMove()
    {
        if (session == null)
        {
            Console.WriteLine("No active game.");
            return;
        }

        if (!HistoryEngine.Instance.Undo())
        {
            Console.WriteLine("There is no move to undo.");
            return;
        }

        // Undo returns the turn to the mover after PlayTurn advanced the index.
        session.CurrentPlayerIndex =
            (session.CurrentPlayerIndex - 1 + session.Players.Length) % session.Players.Length;
        session.Outcome = Result.NotYet;
        Console.WriteLine("Move undone.");
        ShowBoards();
    }

    public void RedoMove()
    {
        if (session == null)
        {
            Console.WriteLine("No active game.");
            return;
        }

        if (!HistoryEngine.Instance.Redo())
        {
            Console.WriteLine("There is no move to redo.");
            return;
        }

        session.CurrentPlayerIndex =
            (session.CurrentPlayerIndex + 1) % session.Players.Length;
        Console.WriteLine("Move redone.");
        ShowBoards();
    }

    // Exit the process cleanly from any prompt (setup, start menu, or mid-turn).
    public void QuitGame()
    {
        Console.WriteLine("Goodbye.");
        Environment.Exit(0);
    }
}
