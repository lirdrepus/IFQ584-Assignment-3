// Boot GameController with SaveLoadHandler + GameStateMapper (Kevin / Slack #assignment-3).

var factory = new GameFactory();
var ui = new ConsoleUI();
var saveLoad = new SaveLoadHandler(new GameStateMapper());
var controller = new GameController(factory, ui, saveLoad);
controller.Run();
