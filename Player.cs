using System.Collections.Generic;

// This is the class that makes Human-vs-Human and Human-vs-Computer the
// SAME code from GameLoop's point of view — GameLoop only ever calls
// player.PlayerTurn(boardList) and never checks which concrete type it is.
// Rules is on the base class (not just AIPlayer) because your Board keeps
// one shared pool of unplaced pieces rather than giving each player their
// own — so even HumanPlayer needs to ask Rules "which of these are
// actually mine to play?" before it can validate console input.
public abstract class Player
{
    public int PlayerNumber { get; }
    protected Rules Rules { get; }

    protected Player(int playerNumber, Rules rules)
    {
        PlayerNumber = playerNumber;
        Rules = rules;
    }

    public abstract Move PlayerTurn(List<Board> boardList);
}
