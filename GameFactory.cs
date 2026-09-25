using System;
using System.Collections.Generic;

public sealed class GameSession
{
    public Rules Rules { get; }
    public List<Board> Boards { get; }
    public Player[] Players { get; }
    public int CurrentPlayerIndex { get; set; }
    public Result Outcome { get; set; } = Result.NotYet;

    public GameSession(Rules rules, List<Board> boards, Player[] players)
    {
        Rules = rules;
        Boards = boards;
        Players = players;
    }
}

// REMOVED: IRulePort, TeamRulesPort, ITeamRulesFactory, TeamRulesFactory, GameOutcome enum
// REMOVED: IPlayerPort, DelegatingPlayerPort, ITeamPlayerFactory

public sealed class GameFactory
{
    public GameSession Create(int gameChoice, int boardSize, bool computerOpponent)
    {
        Rules rules = gameChoice switch
        {
            1 => new NumericalTicTacToeRules(),
            2 => new NotaktoRules(),
            3 => new GomokuRules(),
            _ => throw new ArgumentOutOfRangeException(nameof(gameChoice))
        };

        // Only NumericalTicTacToeRules reads this value (CustomBoard == true);
        // Notakto/Gomoku ignore it internally.
        rules.RulesSetup(rules.CustomBoard ? boardSize : 0);

        Player[] players = new Player[]
        {
            new HumanPlayer(1, rules),
            computerOpponent ? new AIPlayer(2, rules) : new HumanPlayer(2, rules)
        };

        return new GameSession(rules, rules.BoardList, players);
    }
}