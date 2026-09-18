using System;

using System.Drawing;
public class Board {
    private int boardSize; //Board size will always be a single int ex. a boardsize 3 makes a 3x3 board
    private int maxSpace; //The last space on the board, is always boardSize*boardSize
    private int[,] boardState; //Multidimensional Array of what pieces are on the board, this also doubles for tracking all board positions
    private int[] pieces; //Array of pieces that are NOT on the board, these are moved to boardstate
    private bool isLive; //Wether the board is still live for multi-board games such as Notakto
    public int BoardSize {get {return boardSize;}}
    public int[] Pieces {get {return pieces;}}
    public int MaxSpace {get {return maxSpace;}}
    public bool IsLive {get {return isLive;} set{isLive = value;}}
    public Board(int n){ // Constructor for board, n is entered size of board
        boardSize = n; 
        maxSpace = n * n; // Maxspace is calculated
        boardState = new int[boardSize,boardSize];
        pieces = new int[maxSpace + 1]; //Number of pieces is always equal to maxSpace, we add 1 padding
        for(int x = 1; x < pieces.Length; x++){
            pieces[x] = x;}} //Piece is put into piece array, place is the same as value
    public void SetPiece(int number, Point space){ //Generic method for setting a piece on the board
        int selectedPiece = pieces[number];
        Point translatedSpace = LocalToBoard(space);
        boardState[translatedSpace.X,translatedSpace.Y] = selectedPiece; //Piece is placed onto the board
        pieces[number] = 0;} //Taking the piece out of the avaliable pool
    public void RemovePiece(int number, Point space){ //Generic method for removing a piece from board
        Point translatedSpace = LocalToBoard(space);
        boardState[translatedSpace.X,translatedSpace.Y] = 0;
        pieces[number] = number;}
    private static Point LocalToBoard(Point space){ //Translation layer: Converts logical XY space to matrice YX space
        Point translatedSpace = new Point(space.Y - 1,space.X - 1); //We also take away the padding added by user inputs
        return translatedSpace;}
    public void CheckSpace(Point space){ //Method for checking a space is valid and not taken by another piece
        if(space.X == 0 && space.Y == 0){
            throw new PointZeroException();} // 0x0 is not a space the player can access.
        Point translatedSpace = LocalToBoard(space);
        if(boardState[translatedSpace.X,translatedSpace.Y] != 0){
            throw new SpaceTakenException();}} //Need player to try again if they pick a space that has a piece.
    public int[] GetRow(Point space){ //Returns row array of given space.
        Point translatedSpace = LocalToBoard(space);
        int[] rowArray = new int[boardSize];
        for(int x = 0; x < boardSize; x++ ){
            rowArray[x] = boardState[x,translatedSpace.Y];}
        return rowArray;}
    public int[] GetColumn(Point space){ //Returns row array of given space.
        Point translatedSpace = LocalToBoard(space);
        int[] columnArray = new int[boardSize];
        for(int y = 0; y < boardSize; y++ ){
            columnArray[y] = boardState[translatedSpace.X,y];}
        return columnArray;}
    public int[] GetNWDiagonal(){ //Makes Array of the NW diagonal line starting on 1,1
        int[] diagonalNW = new int[boardSize];
        for(int x = 0; x < boardSize; x++){
            Point space = new Point(x,x);
            diagonalNW[x] = boardState[space.Y,space.X];}
        return diagonalNW;}
    public int[] GetNEDiagonal(){ //Makes Array of the NE diagonal line starting on boardsize,boardsize
        int[] diagonalNE = new int[boardSize];
        for(int x = 0; x < boardSize; x++){
            Point space = new Point((boardSize - 1) - x,x); //Formula for finding each diagonal space starting NE
            diagonalNE[x] = boardState[space.Y,space.X];}
        return diagonalNE;}
    public List<Point> GetAvaliableSpaces(){ //Method that returns spaces that do not contain a piece
        List<Point> avaliableSpaces = new List<Point>();
        for(int y = 0; y < boardSize;y++){
            for(int x = 0; x < boardSize; x++){
                int currentSpace = boardState[y,x];
                if(currentSpace == 0){
                    Point avaliable = new Point(x + 1,y + 1); //Translated to local space.
                    avaliableSpaces.Add(avaliable);}}}
        return avaliableSpaces;}
    public int GetPiece(Point space){ // Obtains the piece on listed space (or 0 if no piece)
        Point translatedSpace = LocalToBoard(space);
        int findPiece = boardState[translatedSpace.X,translatedSpace.Y];
        return findPiece;}}