using System;
using System.Drawing;
public class HistoryEngine {
    private List<Move> moveHistory;
    private List<Move> redoHistory;

    private static HistoryEngine? instance; //Singleton pointer.

    public  static HistoryEngine Instance {get{return instance;}}

    public void RecordMove(Piece piece,Player player, int boardNumber,Point position){
        Move newMove = MoveFactory(piece,player,boardNumber,position);
        moveHistory.Add(newMove);
        redoHistory.Clear(); //Anything in redo is outdated now so is flushed
    }


    private Move MoveFactory(Piece piece, Player player, int boardNumber,Point position){
        Move newMove = new Move(piece,player,boardNumber,position);
        return newMove;
    }


    //TODO: Needs reference to boardList
    public bool Undo(){ //Performs an Undo on given list of boards.
        Move lastMove = moveHistory[^1];
        //TODO: Throw exception here if no move found
        Board board = boardList[lastMove.BoardNumber - 1];//? Depends How boardlist is implemented
        board.RemovePiece(lastMove.MyPiece);
        //TODO: Check if piece was successfully removed??
        moveHistory.Remove(lastMove);
        redoHistory.Add(lastMove);
        //^^ There is also a world where we index thru but I think this is more consistent.
        //TODO: Return undo successful or something
        return true;

    }

    public bool Redo(){
        Move redoMove = redoHistory[^1];
        Board board = boardList[lastMove.BoardNumber - 1];
        board.SetPiece(redoMove.piece);
        redoHistory.Remove(redoMove);
        moveHistory.Add(redoMove);
        return true;
    }

    //From here, a load from save method could be created that imports all saved moves
    //Into redoHistory and then loops through a .Count, redoing all the taken moves.
}