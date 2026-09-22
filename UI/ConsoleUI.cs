using System.Drawing;

public class ConsoleUI {
    
    public int PromptInteger(string prompt){ //Allows for caller to give a prompt to player
        bool incomplete = true;                   //The argument allows a "prompt" to be displayed to the player
        int chosenNumber = 0;
        Console.WriteLine(prompt);
        while (incomplete){
            try{
                Console.WriteLine(prompt);
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


        private string PlayerInput(string prompt){ //Allows for player to input commands or seek help.
            bool incomplete = true;
            while (incomplete){
                Console.Write("Input:");
                string? input = Console.ReadLine();
                if(input == "HELP"){ //TODO: This should all be a strategy
                    DisplayHelpMenu(); // THESE SHOULD BE COMMANDS
                    continue;
                }
                if (input == "SAVE"){
                    InitiateSave();
                    continue;
                }
                if (input == "LOAD"){
                    InitiateLoad();
                    continue;
                }
                return input;
            }
            

        }
    }



