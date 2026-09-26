
public interface UICommand {
    void Execute();
}

// Sean UICommand stubs filled in-place (Slack #assignment-3 / Naveed; Sean exited).
public class HelpCommand : UICommand {
    private readonly GameController controller;
    public HelpCommand(GameController controller) { this.controller = controller; }
    public void Execute() => controller.ShowHelp();
}

public class SaveCommand : UICommand {
    private readonly GameController controller;
    public SaveCommand(GameController controller) { this.controller = controller; }
    public void Execute() => controller.SaveGame();
}

public class LoadCommand : UICommand {
    private readonly GameController controller;
    public LoadCommand(GameController controller) { this.controller = controller; }
    public void Execute() => controller.LoadGame();
}

public class UndoCommand : UICommand {
    private readonly GameController controller;
    public UndoCommand(GameController controller) { this.controller = controller; }
    public void Execute() => controller.UndoMove();
}

public class RedoCommand : UICommand {
    private readonly GameController controller;
    public RedoCommand(GameController controller) { this.controller = controller; }
    public void Execute() => controller.RedoMove();
}
