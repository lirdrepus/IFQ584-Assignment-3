using System.Drawing;

// Sean ConsoleUI kept; Bind fills empty command stubs (Slack #assignment-3).
public class ConsoleUI {

    // Sean command strategy map (filled in Bind).
    private Dictionary<string, UICommand> CommandStrategies = new();
    private GameController? controller;

    // Active UI so HumanPlayer can route SAVE/UNDO without a UI ref.
    public static ConsoleUI? Active { get; private set; }

    public ConsoleUI()
    {
        Active = this;
    }

    public void Bind(GameController gameController)
    {
        controller = gameController;
        CommandStrategies = new Dictionary<string, UICommand>
        {
            ["HELP"] = new HelpCommand(gameController),
            ["SAVE"] = new SaveCommand(gameController),
            ["LOAD"] = new LoadCommand(gameController),
            ["UNDO"] = new UndoCommand(gameController),
            ["REDO"] = new RedoCommand(gameController),
            ["QUIT"] = new QuitCommand(gameController)
        };
    }

    // Prompt the player for an integer.
    public int PromptInteger(string prompt){
        bool incomplete = true;
        int chosenNumber = 0;
        Console.WriteLine(prompt);
        while (incomplete){
            try{
                string input = PlayerInput();
                chosenNumber = System.Convert.ToInt32(input);}
            catch{
                Console.WriteLine("Invalid input detected. Please follow the instructions and try again.");
                continue;}
            incomplete = false;}
        return chosenNumber;}

    public bool TryHandleCommand(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        string key = input.Trim().ToUpperInvariant();
        if (CommandStrategies.TryGetValue(key, out UICommand? command))
        {
            command.Execute();
            return true;
        }
        return false;
    }

    public static bool TryHandleActiveCommand(string? input)
    {
        return Active != null && Active.TryHandleCommand(input);
    }

    // Read input; run a UI command if matched, else return the value.
    private string PlayerInput(){
        bool incomplete = true;
        string input = "";
        while (incomplete){
            Console.Write("Input:");
            input = Console.ReadLine() ?? "";
            if (TryHandleCommand(input))
            {
                continue;
            }
            incomplete = false;
        }
        return input;
    }
}
