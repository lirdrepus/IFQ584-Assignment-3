# Kevin: save / load / undo / redo / help notes

Australian English. Written against `main` at `d0619f5` (bootable console loop).

## Requirement anchors (local archives)

From `archive/assessments-2026-08-10/03-assignment-2-object-oriented-design.md` (Assignment 2 Requirements, carried into A3):

- "A game can be saved and restored from any state of play, which is stored in a save file."
- "During a game, human players can undo and redo any number of turns"
- "The program should provide a simple in-game help menu system to guide users with the available commands."
- "Your program begins by presenting the user with an option to load an existing game from a save or initiate a fresh game."

Assignment 3 fulfilment rubric (`archive/assessments-2026-08-10/04-assignment-3-object-oriented-software-development-project.md`) expects an extensible framework with all three games, undo/redo, save/load, AI behaviour, game modes, usability, and design patterns. Team milestone text in that brief also lists designing save/load and undo/redo ahead of implementation.

Slack #assignment-3 context used for authorship: Terry (lirdrepus) put the bootable GameController/Factory/SaveLoadHandler on main without save/undo behaviour wired; Kevin takes save/load/undo; Naveed said exclude Sean; Sean UI stubs were empty, so they were filled in-place.

## What was wired

- **`GameStateMapper.cs`** implements `IGameStateMapper` (`Capture` / `Restore`). Restore mirrors `GameFactory`: rules from `GameName`, `RulesSetup`, overlay cells/`IsLive`, players from `PlayerType`, then `CurrentPlayerIndex` / `Outcome`. History stacks are restored best-effort via `HistoryEngine.ReplaceHistory`.
- **`HistoryEngine`**: `BoardList` is assigned on new game and after load. Added `AppliedMoves` / `RedoMoves`, `ClearHistory`, `ReplaceHistory`. Undo revives `IsLive` on the affected board (Notakto).
- **`GameController`**: takes `SaveLoadHandler`, owns HELP/SAVE/LOAD/UNDO/REDO. Default file: `save.json`. Undo/redo also adjust `CurrentPlayerIndex` and clear a finished `Outcome` on undo. Terry's play loop (turn, CheckWin, Notakto aggregation, draw) is kept.
- **`UI/ConsoleUI` + `UI/UICommands`**: Sean's command strategy filled in-place (HELP/SAVE/LOAD/UNDO/REDO registered and implemented). `Bind(GameController)` supplies the session/controller refs. Case-insensitive command matching.
- **`HumanPlayer`**: every `ReadLine` goes through command interception so typing SAVE/UNDO/HELP mid-turn works without a second menu. LOAD mid-turn throws `SessionReplacedException` so the turn aborts cleanly against the new session. Cecil prompt/validation logic is unchanged.
- **`Program.cs`**: constructs `GameStateMapper` -> `SaveLoadHandler` -> `GameController`.

## Sean UI note (exclusion / in-place fill)

Sean owns `UI/ConsoleUI.cs` and `UI/UICommands.cs`. Those files were **not replaced wholesale**; they were filled in-place so the bootable loop keeps working. A later UI rewrite can keep the same command names and call into `GameController`'s public HELP/SAVE/LOAD/UNDO/REDO helpers (or re-bind the strategy dictionary the same way). Prefer not to duplicate a second controller-side command switch unless a rewrite removes the strategy pattern.

## Limitations

- **No .NET SDK on this Mac** (`dotnet` not found) when this was written. Code matches current APIs but was not compiled here.
- **Redo + Notakto `IsLive`**: `HistoryEngine.Redo` only `SetPiece`s; it does not re-run `Rules.CheckWin`, so a redone killing move may leave the board live until a later real move evaluates wins.
- **Saved move `RenderValue`**: `SavedMoveState` has no render field; restore infers from the board cell or `PieceValue.ToString()`.
- **LOAD during setup**: sets a pending session; enter any number at the next setup prompt to adopt it (avoids fighting `PromptInteger`). This is a workable path, but not a dedicated "new game vs load" first screen from the Assignment 2 brief.
- **AI turns**: commands are only intercepted on human input prompts, not during an AI move.
- **QUIT**: not registered as a UI command (process exit still ends the run).
- **Terry's playable loop** (factory -> turn -> `CheckWin` / Notakto aggregation / draw) is unchanged aside from history `BoardList` assignment and the LOAD abort path.

## How to test (once `dotnet` is available)

```bash
cd /Users/kevinnaderian/Desktop/QUTMASTER/IFQ584/assignment-3
dotnet build
dotnet run
```

1. Start Numerical Tic-Tac-Toe, human vs human, size 3.
2. Make a few moves; type `HELP`, `SAVE`, `UNDO`, `REDO` at a move prompt.
3. `SAVE`, quit the process, `dotnet run` again, type `LOAD` at setup (then any number) or during a turn; confirm boards/turn resume.
4. Repeat a quick Notakto undo of a board-killing move and confirm the board is live again.
5. Confirm Terry's normal win/draw paths still work without using commands.
