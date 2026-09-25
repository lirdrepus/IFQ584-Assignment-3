
public class GameSetup(){ //Game setup script
    

    private Dictionary<int,Rules> RulesStrategy = new(){
        [1] = new NumericalTicTacToeRules()
    };


    public bool RulesFactory(){ //Attempts to create rules
        return true;
    }
}