using System;
using System.Collections.Generic;

//Strategy pattern base: each game owns its own boardList, piece creation, and win-checking logic
//so Game/GameLoop never needs to know which game is being played.
public abstract class Rules : RulesInterface
{
    protected List<Board> boardList;

    public abstract Result CheckWin(int boardNumber, Point space);
    public abstract List<Piece> AvailablePieces(int player);
    public abstract List<Piece> CreatePieceSet();
    public abstract List<Board> BoardFactory();
}