using System;

using System.Drawing;
public class Board {
    private int boardSize; //Board size will always be a single int ex. a boardsize 3 makes a 3x3 board
    private int maxSpace; //The last space on the board, is always boardSize*boardSize
    private Piece[,] boardState; //Multidimensional Array of what pieces are on the board, this also doubles for tracking all board positions
    private List<Piece> pieces; //Array of pieces that are NOT on the board, these are moved to boardstate
    private bool isLive; //Wether the board is still live for multi-board games such as Notakto
    public int BoardSize {get {return boardSize;}}
    public List<Piece> Pieces {get {return pieces;}}
    public int MaxSpace {get {return maxSpace;}}
    public bool IsLive {get {return isLive;} set{isLive = value;}}
    public Board(int n, List<Piece> pieces){ // Constructor for board, n is entered size of board
        boardSize = n; 
        maxSpace = n * n; // Maxspace is calculated
        boardState = new Piece[boardSize,boardSize];
        this.pieces = pieces;
        isLive = true;
    }
    public void SetPiece(int number, Point space){ //Generic method for setting a piece on the board
        Piece selectedPiece = pieces.Find(delegate(Piece p){
            return p.Value == number;
        })!;
        Point translatedSpace = LocalToBoard(space);
        boardState[translatedSpace.X,translatedSpace.Y] = selectedPiece; //Piece is placed onto the board
        pieces.Remove(selectedPiece);} //Taking the piece out of the avaliable pool
    public void RemovePiece(Point position){ //Generic method for removing a piece from board
        Point translatedSpace = LocalToBoard(position);
        Piece pieceOnSpace = boardState[translatedSpace.X,translatedSpace.Y];
        boardState[translatedSpace.X,translatedSpace.Y] = null!;
        pieces.Add(pieceOnSpace);}
    private static Point LocalToBoard(Point space){ //Translation layer: Converts logical XY space to matrice YX space
        Point translatedSpace = new Point(space.Y - 1,space.X - 1); //We also take away the padding added by user inputs
        return translatedSpace;}
    public void CheckSpace(Point space){ //Method for checking a space is valid and not taken by another piece
        if(space.X == 0 && space.Y == 0){
            throw new PointZeroException();} // 0x0 is not a space the player can access.
        Point translatedSpace = LocalToBoard(space);
        if(boardState[translatedSpace.X,translatedSpace.Y] != null){
            throw new SpaceTakenException();}} //Need player to try again if they pick a space that has a piece.
    public Piece[] GetRow(Point space){ //Returns row array of given space.
        Point translatedSpace = LocalToBoard(space);
        Piece[] rowArray = new Piece[boardSize];
        for(int x = 0; x < boardSize; x++ ){
            rowArray[x] = boardState[x,translatedSpace.Y];}
        return rowArray;}
    public Piece[] GetColumn(Point space){ //Returns row array of given space.
        Point translatedSpace = LocalToBoard(space);
        Piece[] columnArray = new Piece[boardSize];
        for(int y = 0; y < boardSize; y++ ){
            columnArray[y] = boardState[translatedSpace.X,y];}
        return columnArray;}
    public Piece[] GetNWDiagonal(){ //Makes Array of the NW diagonal line starting on 1,1
        Piece[] diagonalNW = new Piece[boardSize];
        for(int x = 0; x < boardSize; x++){
            Point space = new Point(x,x);
            diagonalNW[x] = boardState[space.Y,space.X];}
        return diagonalNW;}
    public Piece[] GetNEDiagonal(){ //Makes Array of the NE diagonal line starting on boardsize,boardsize
        Piece[] diagonalNE = new Piece[boardSize];
        for(int x = 0; x < boardSize; x++){
            Point space = new Point((boardSize - 1) - x,x); //Formula for finding each diagonal space starting NE
            diagonalNE[x] = boardState[space.Y,space.X];}
        return diagonalNE;}
    public List<Point> GetAvaliableSpaces(){ //Method that returns spaces that do not contain a piece
        List<Point> avaliableSpaces = new List<Point>();
        for(int y = 0; y < boardSize;y++){
            for(int x = 0; x < boardSize; x++){
                Piece currentSpace = boardState[y,x];
                if(currentSpace == null){
                    Point avaliable = new Point(x + 1,y + 1); //Translated to local space.
                    avaliableSpaces.Add(avaliable);}}}
        return avaliableSpaces;}
    public Piece GetPiece(Point space){ // Obtains the piece on listed space (or 0 if no piece)
        Point translatedSpace = LocalToBoard(space);
        Piece findPiece = boardState[translatedSpace.X,translatedSpace.Y];
        return findPiece;}}