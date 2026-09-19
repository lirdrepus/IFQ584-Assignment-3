//This entire script is just a theory on the shape of rules

using System;

using System.Drawing;

public abstract class Rules : RulesInterface {
    public List<Board> boardList;

}

public interface RulesInterface{
    bool CheckWin();
    List<Piece> AvaliablePieces();
    List<Piece> CreatePieceSet();
    List<Board> BoardFactory();

}


public class RulesNumericalTTT : Rules {

    int goal;
    public RulesNumericalTTT(){
        this.boardList = BoardFactory();
    }

    public List<Board> BoardFactory(){
        List<Board> newBoardList = null;
        //HERE WE WOULD PROMPT BOARD SIZE, I JUST DO a 3 FOR NOW
        int boardSize = 3;
        List<Piece> pieces = CreatePieceSet(boardSize);
        Board newBoard = new Board(boardSize,pieces);
        newBoardList.Add(newBoard);
        return newBoardList;
    }

    public List<Piece> CreatePieceSet(int n){
        List<Piece> pieces = new List<Piece>();
        for(int x = 1; x < n; x++){
            Piece newPiece = new Piece(x,$"{x}");
            pieces.Add(newPiece);}
        return pieces;
    }
    public bool CheckWin(Point space){ //Method that calculates wins using the current turn's space
        Board board = boardList[0];
        Piece[] row = board.GetRow(space);//Checking Row
        bool rowWon = WinSum(row);
        if(rowWon){
            return true;}
        Piece[] column = board.GetColumn(space);//Checking Column
        bool columnWon = WinSum(column);
        if(columnWon){
            return true;}
        if(space.X == space.Y){ //If these are equal, the space is on a diagonal line for Num.TTT Purposes.
            Piece[] NWDiagonal = board.GetNWDiagonal();
            bool NWWin = WinSum(NWDiagonal);
            if(NWWin){
                return true;}}
        if((space.X + space.Y) == (board.BoardSize + 1)){ // If these are equal, space is on the diagonal line starting at NW
            Piece[] NEDiagonal = board.GetNEDiagonal();
            bool NEWin = WinSum(NEDiagonal);
            if(NEWin == true){
                return true;}}
        return false;} //If method makes it this far, the game has not been won.

    private bool WinSum(Piece[] line){ //Helper method that tallies a given line and checks against goal
    if(Array.TrueForAll(line, piece => piece.Value != 0) == false){
        return false;} // We do not sum incomplete lines
    int total = 0;
    foreach(Piece piece in line){
        total = total + piece.Value;}
    if(total == goal){
        return true;}
    return false;}

    private List<int> AvaliablePieces(int player){ //Method that returns avaliable pieces of given player number
        Board board = boardList[0]; //TODO: ABOVE ARG SHOULD BE PLAYER...
        List<Piece> pieces = board.Pieces; //Get list of pieces from board
        List<int> avaliablePieces = new List<int>() ;
        for(int x = 1; x < pieces.Count; x++ ){ // Let's calculate if a piece is owned by the player
            Piece currentPiece = pieces[x];
            if(currentPiece.Value != 0 && System.Int32.IsOddInteger(player) == System.Int32.IsOddInteger(currentPiece.Value)){}
            else if(currentPiece.Value != 0){ // We and make sure the piece is not on the board and is owned by the player using above.
                avaliablePieces.Add(currentPiece.Value);}}
        return avaliablePieces;}
}