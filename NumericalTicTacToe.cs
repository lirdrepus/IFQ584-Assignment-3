using System.Drawing;
public class NumericalTicTacToe {
    private Board board = null!; //Reference to the board the game takes place on.
    private RenderEngine renderEngine = null!; //Reference to the render engine that draws the board for the player.
    private NumericalTicTacToeAI ai = null!; //Reference to the AI that calculates computer's turns in human vs. computer.
    private bool[] players = new bool[2]; //Array for players. Players with a value of "false" are human.
    private int goal; //The sum of a line needed to win a game of Numerical TTT
    public void StartGame(){
        Console.WriteLine("Welcome to Numerical Tic Tac Toe. Please enter a board size to play on. For example a size of 3 will result in a 3x3 board.");
        Console.WriteLine("For playability, it is reccomended you enter a number between 3 and 6.");
        PromptBoardSize();
        renderEngine = new RenderEngine(board);
        PromptGameMode();
        GameLoop();}
    private void PromptBoardSize(){
        bool promptComplete = false;
        while(promptComplete == false){
            int input = PromptNumberInput("Board size:");
            if(input <= 0){
                Console.WriteLine("Board size must be larger than zero. Please try again.");
                continue;}
            promptComplete = true;
            board = new Board(input);
            goal = board.BoardSize * ((board.BoardSize*board.BoardSize) + 1) / 2;}} //Formula for determing goal
    private void PromptGameMode(){
        bool promptComplete = false;
        while(promptComplete == false){
            Console.WriteLine("Please choose a gamemode, either human vs human or human vs computer.");
            Console.WriteLine("1: Human vs. Human    2: Human vs. Computer");
            int input = PromptNumberInput("Game mode:");
            if(input != 1 && input != 2){
                Console.WriteLine("You must choose a listed game mode. Please try again.");
                continue;}
            else if(input == 2){
                ai = new NumericalTicTacToeAI(board,this,renderEngine);
                DetermineAISide();}
            promptComplete = true;}}
    private void DetermineAISide(){
        Random rng = new Random();
        int aiSelect = rng.Next(0,2); //Basically flipping a coin to determine if ai is odds or evens
        players[aiSelect] = true;
        Console.WriteLine($"The computer is player {aiSelect + 1}");}
    private void GameLoop(){
        bool gameWon = false;
        while(gameWon == false){
            for(int x = 0; x < players.Length; x++){
                renderEngine.DrawBoard();
                if(Array.TrueForAll(board.Pieces, x => x == 0)){ //Checking to make sure game can continue and if not draw happens.
                    Console.WriteLine("There are no pieces left, game ends in a draw!");
                    return;}
                Console.WriteLine($"It is Player {x+1}'s turn.");
                if(players[x] == true){
                    gameWon = ComputerTurn(x);} //If player is not human then the computer takes the turn.
                else{
                    gameWon = PlayerTurn(x);} //Will return true if player won the game on this move.
                if(gameWon == true){
                    Console.WriteLine($"Player {x+1} has won the game! Well done.");
                    return;}
                else{
                    Console.WriteLine($"End of Player {x+1}'s turn. Press any key to continue.");}
                    Console.ReadKey();}}}
    private bool ComputerTurn(int player){ //Method that tells computer to take a turn
        List<int> avaliablePieces = AvaliablePieces(player);
        bool gameWon = ai.AITurn(avaliablePieces);
        return gameWon;}
    private bool PlayerTurn(int player){ //Method that goes through other methods that make up a human player's turn
        int chosenPiece = PlayerSelectPiece(player); //Get piece
        Point chosenSpace = PlayerSelectSpace(chosenPiece); //Get space
        board.SetPiece(chosenPiece,chosenSpace); //Put piece on space
        renderEngine.DrawBoard(); //Show player what they did
        Console.WriteLine($"You have placed {chosenPiece} on space {chosenSpace.X},{chosenSpace.Y}.");
        bool gameWon = CheckWin(chosenSpace); //Check if player won
        return gameWon;}
    private int PlayerSelectPiece(int player){ //Method for prompting player to select a piece
        List<int> avaliablePieces = AvaliablePieces(player); //Get pieces owned by player
        string piecesUI = "Your Pieces: ";
        foreach(int piece in avaliablePieces){
            piecesUI = piecesUI + $"{piece} ";} //Preparing to display owned pieces
        bool selectionIncomplete = true;
        int chosenPiece = 0;
        while (selectionIncomplete) {
            Console.WriteLine("Please enter a piece to use or type HELP for help.");
            Console.WriteLine(piecesUI);
            int input = PromptNumberInput("Piece:"); // Ask player for piece
            if(avaliablePieces.Contains(input) == false){ //Checking input is a valid piece owned by player.
                Console.WriteLine("Invalid piece chosen. Please choose one from your avaliable pieces.");
                continue;}
            chosenPiece = input;
            selectionIncomplete = false;}
        return chosenPiece;}
    private Point PlayerSelectSpace(int chosenPiece){ //Method for prompting player to select a space to use a piece on.
        bool selectionIncomplete = true;
        Point space = new Point();
        Console.WriteLine($"Please select a space to put piece {chosenPiece}.");
        while(selectionIncomplete){
            Console.WriteLine("Enter the row to use. e.g. 2 for row 2 (From the top).");
            int row = PromptNumberInput("Row:");
            space.X = row;
            Console.WriteLine("Enter the column to use. e.g. 1 for column 1 (From the left).");
            int column = PromptNumberInput("Column:");
            space.Y = column;
            try{
                board.CheckSpace(space);} // We check the space is valid and unused.
            catch (SpaceTakenException){
                Console.WriteLine("Chosen space is already taken, please try again.");
                continue;}
            catch{
                Console.WriteLine("Invalid space entered. Please try again.");
                continue;}
            selectionIncomplete = false;}
        return space;}
    private List<int> AvaliablePieces(int player){ //Method that returns avaliable pieces of given player number
        int[] pieces = board.Pieces; //Get list of pieces from board
        List<int> avaliablePieces = new List<int>() ;
        for(int x = 1; x < pieces.Length; x++ ){ // Let's calculate if a piece is owned by the player
            int currentPiece = pieces[x];
            if(currentPiece != 0 && System.Int32.IsOddInteger(player) == System.Int32.IsOddInteger(currentPiece)){}
            else if(currentPiece != 0){ // We and make sure the piece is not on the board and is owned by the player using above.
                avaliablePieces.Add(currentPiece);}}
        return avaliablePieces;}
    private int PromptNumberInput(string prompt){ //Generic method for prompting player numerical input
        bool incomplete = true;                   //The argument allows a "prompt" to be displayed to the player
        int chosenNumber = 0;
        while (incomplete){
            try{
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if(input == "HELP" && goal != 0){ //Help command tells player how to win
                    Console.WriteLine("You are playing Numerical Tic Tac Toe.");
                    Console.WriteLine($"The goal of this game is to get a row, column or diagonal line to add to {goal} when placing a piece.");
                    continue;}
                chosenNumber = System.Convert.ToInt32(input);} //Grabbing player input and converting it to an integer
            catch{
                Console.WriteLine("Invalid input detected. Please follow the instructions and try again.");
                continue;} //If the player enters non-integer input we re-prompt them.
            incomplete = false;}
        return chosenNumber;}

    public bool CheckWin(Point space){ //Method that calculates wins using the current turn's space
        int[] row = board.GetRow(space);//Checking Row
        bool rowWon = WinSum(row);
        if(rowWon){
            return true;}
        int[] column = board.GetColumn(space);//Checking Column
        bool columnWon = WinSum(column);
        if(columnWon){
            return true;}
        if(space.X == space.Y){ //If these are equal, the space is on a diagonal line for Num.TTT Purposes.
            int[] NWDiagonal = board.GetNWDiagonal();
            bool NWWin = WinSum(NWDiagonal);
            if(NWWin){
                return true;}}
        if((space.X + space.Y) == (board.BoardSize + 1)){ // If these are equal, space is on the diagonal line starting at NW
            int[] NEDiagonal = board.GetNEDiagonal();
            bool NEWin = WinSum(NEDiagonal);
            if(NEWin == true){
                return true;}}
        return false;} //If method makes it this far, the game has not been won.
    private bool WinSum(int[] line){ //Helper method that tallies a given line and checks against goal
    if(Array.TrueForAll(line, x => x != 0) == false){
        return false;} // We do not sum incomplete lines
    int total = 0;
    foreach(int num in line){
        total = total + num;}
    if(total == goal){
        return true;}
    return false;}
}