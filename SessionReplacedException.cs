using System;

// LOAD replaced the session mid-turn; PlayTurn aborts without recording on old boards.
public sealed class SessionReplacedException : Exception
{
    public SessionReplacedException() : base("Game session was replaced by LOAD.") { }
}
