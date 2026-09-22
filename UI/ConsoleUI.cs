using System.Drawing;

public class ConsoleUI {
    
    public int PromptInteger(string prompt){ //Allows for caller to give a prompt to player
        bool incomplete = true;    //The argument allows a "prompt" to be displayed to the player
        int chosenNumber = 0;
        Console.WriteLine(prompt);
        while (incomplete){
            try{
                string input = PlayerInput();
                chosenNumber = System.Convert.ToInt32(input);} //Grabbing player input and converting it to an integer
            catch{
                Console.WriteLine("Invalid input detected. Please follow the instructions and try again.");
                continue;} //If the player enters non-integer input we re-prompt them.
            incomplete = false;}
        return chosenNumber;}

        private Dictionary<string, UICommand> CommandStrategies = new(){
        ["HELP"] = new HelpCommand(), //Strategy pattern to allow user to select various commands
        ["SAVE"] = new SaveCommand(),
        ["LOAD"] = new LoadCommand()
        };
        private string PlayerInput(){ //Allows for player to input commands or seek help.
            bool incomplete = true;
            string input = "";
            while (incomplete){
                Console.Write("Input:");
                input = Console.ReadLine();
                UICommand command; //Checking Strategy to see if a command was entered.
                if(CommandStrategies.TryGetValue(input, out command!)){
                    command.Execute(); //Command is executed
                    continue; //Input resumes after command execution
                }
                incomplete = false;
            }
            return input;
            

        }
    }



