using System;
using System.Collections.Generic;

//Strategy pattern base: each game owns its own boardList, piece creation, and win-checking logic
//so Game/GameLoop never needs to know which game is being played.
public abstract class Rules : RulesInterface
{
    protected List<Board> boardList;
    public List<Board> BoardList => boardList;

    public abstract string GameName { get; }
    public abstract string GameDescription { get; }

    public abstract void RulesSetup(int boardSize = 0);
    public abstract Result CheckWin(Move move);
    public abstract List<Piece> AvailablePieces(int player);
    public abstract List<Piece> CreatePieceSet();
    public abstract List<Board> BoardFactory();
    public virtual bool CustomBoard => false; //override to true only where board size is user-prompted
}