
public interface UICommand {
    void Execute();
}

//TODO: Implement commands
public class HelpCommand : UICommand {
    public void Execute(){
        return;
    }
}

public class SaveCommand : UICommand {
    public void Execute() {
        return;
    }
}

public class LoadCommand : UICommand {
    public void Execute(){
        return;
    }
}