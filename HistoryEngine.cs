using System;
using System.Drawing;
public class HistoryEngine {
    private List<Move> moveHistory;
    private List<Move> redoHistory;

    public void RecordMove(Piece piece,Player player, int boardNumber,Point position){
        //TODO: Redo history should be flushed whenever a new move is recorded here?
        Move newMove = MoveFactory(piece,player,boardNumber,position);
        moveHistory.Add(move);
    }


    private Move MoveFactory(Piece piece,Player player, int boardNumber,Point position){
        Move newMove = new Move(piece,player,boardNumber,position);
        return newMove;
    }

    public Undo(){ //Performs an Undo on given list of boards. (Or boards should already be in this objecrt?)
        Move lastMove = moveHistory[^1];
        //TODO: Throw exception here if no move found
        Board board = boardList[lastMove.BoardNumber - 1];//? Depends How boardlist is implemented
        board.RemovePiece(lastMove.piece);
        //TODO: Check if piece was successfully removed??
        moveHistory.Remove(lastMove);
        redoHistory.Add(lastMove);
        //^^ There is also a world where we index thru but I think this is more consistent.
        //TODO: Return undo successful or something

    }

    public Redo(){
        Move redoMove = redoHistory[^1];
        Board board = boardList[lastMove.BoardNumber - 1];
        board.SetPiece(redoMove.piece);
        redoHistory.Remove(redoMove);
        undoHistory.Add(redoMove);
    }
}