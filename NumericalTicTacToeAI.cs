using System.Drawing;
public class NumericalTicTacToeAI{ //Object that contains the set of hueristics and operations for the AI
    private Board board;
    private NumericalTicTacToe rules;
    private RenderEngine renderEngine;
    public NumericalTicTacToeAI(Board board, NumericalTicTacToe rules, RenderEngine renderEngine){
        this.board = board;
        this.rules = rules;
        this.renderEngine = renderEngine;}
    public bool AITurn(List<int> avaliablePieces){ //Method that performs computer's turn
        List<Point> avaliableSpaces = board.GetAvaliableSpaces(); //Get all free spaces
        bool winFound = FindWin(avaliablePieces,avaliableSpaces); //Looking for a win first
        if(winFound){
            return true;} //Tell TTT computer found a win
        RandomMove(avaliablePieces,avaliableSpaces); //Random move if no win found
        return false;} //Tell TTT computer did not find a win
    private bool FindWin(List<int> pieces, List<Point> spaces){ //Checks every given space and piece to find a win
        foreach(Point space in spaces){
            foreach(int piece in pieces){
                board.SetPiece(piece,space); //Placing piece on board
                bool possibleWin = rules.CheckWin(space); //Checking if there are any wins using that piece
                if(possibleWin){
                    DisplayMove(piece,space);//If a win is found the computer confirms the move and ends turn to win
                    return true;}
                else{
                    board.RemovePiece(piece,space);}}} //If no wins, we remove the piece
        return false;} //If all spaces fail to find win, we can return and place a random piece
    private void RandomMove(List<int> pieces, List<Point> spaces){ //Method to allow computer to take random move
        Random rng = new Random();                                 //Random moves can only be ones that can't win, 
        int randomPiece = pieces[rng.Next(0,pieces.Count)];        //so winning is not checked.
        Point randomSpace = spaces[rng.Next(0,spaces.Count)];
        board.SetPiece(randomPiece,randomSpace); //After obtaining random move and space, set piece there.
        DisplayMove(randomPiece,randomSpace);}
    private void DisplayMove(int piece,Point space){ //Method for displaying the computer's turn to player
        renderEngine.DrawBoard();
        Console.WriteLine($"It is the computer's turn.");
        Console.WriteLine($"Computer has placed piece {piece} on space {space.X},{space.Y}.");}
}