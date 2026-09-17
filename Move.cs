using System;
using System.Drawing;
public class Move {
    private Piece myPiece; //Move's piece
    private Player myPlayer; //Player who performed this move
    private int boardNumber; //Number of board in board list move was performed on
    private Point position; //Position/Coordiante of move

    public Piece MyPiece {get {return myPiece}};
    public Player MyPlayer {get {return MyPlayer}};
    public int BoardNumber {get {return boardNumber}};
    public Point MovePosition {get {return movePosition}};

    public Move(Piece piece,Player player, int boardNumber,Point position){
        myPiece = piece;                     //Constructor for move object
        myPlayer = player;
        this.boardNumber = boardNumber;
        this.position = position;
    }
}