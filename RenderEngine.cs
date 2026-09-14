using System;
using System.Drawing;
public class RenderEngine { //Draws the board to the screen so that the user can parse what is happening
    private Board board;
    private string blankSpace = ""; //Property that contains padding for blank spaces
    public RenderEngine(Board board){
        this.board = board;
        for( int x = 0; x < System.Convert.ToString(board.MaxSpace).Length; x++){
            blankSpace = blankSpace + ".";}}
    public void DrawBoard() {
        Console.Clear();
        for(int x = 1; x <= board.BoardSize; x++){ //Loop over each row and draw rows to screen
            string line = System.Convert.ToString(x) + ") "; // Label row
            for(int y = 1; y <= board.BoardSize; y++){// Loop over each space
                Point space = new Point(x,y);
                string spaceRender = "";
                int pieceNumber = board.GetPiece(space);
                spaceRender = pieceNumber.ToString($"D{blankSpace.Length}");
                if(pieceNumber == 0){
                    spaceRender = blankSpace;} //If there is no piece, we catch and insert a default space
                line = line + ($"  {spaceRender}  ");}
            Console.WriteLine(line);}}
}