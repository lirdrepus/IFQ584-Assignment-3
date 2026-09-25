// Program.cs
// REMOVED: the original top-level statements (Console.Clear(); NumericalTicTacToe ttt = ...)
// — that was the old A1 entry point, replaced by GameController now that Rules/
// GameFactory exist. Also removes the class-name clash with Kevin's
// `public static class Program { public static void Run(...) }`.

var factory = new GameFactory();
var ui = new ConsoleUI();
var controller = new GameController(factory, ui);
controller.Run();