using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

public sealed class SavedGameState
{
    public string GameName { get; set; } = "";
    public int CurrentPlayerIndex { get; set; }
    public Result Outcome { get; set; }
    public List<SavedBoardState> Boards { get; set; } = new();
    public List<SavedMoveState> AppliedMoves { get; set; } = new();
    public List<SavedMoveState> RedoMoves { get; set; } = new();
    public List<SavedPlayerState> Players { get; set; } = new();
}

public sealed class SavedBoardState
{
    public int Size { get; set; }
    public bool IsLive { get; set; }
    public List<SavedCellState> Cells { get; set; } = new();
}

public sealed class SavedCellState
{
    public int Row { get; set; }
    public int Column { get; set; }
    public int PieceValue { get; set; }
    public string RenderValue { get; set; } = "";
}

public sealed class SavedMoveState
{
    public int BoardNumber { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
    public int PieceValue { get; set; }
    public int PlayerNumber { get; set; }
}

public sealed class SavedPlayerState
{
    public int PlayerNumber { get; set; }
    public string PlayerType { get; set; } = "";
}

public interface IGameStateMapper
{
    SavedGameState Capture(GameSession session);
    GameSession Restore(SavedGameState snapshot);
}

public sealed class SaveLoadHandler
{
    private readonly IGameStateMapper mapper;

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public SaveLoadHandler(IGameStateMapper mapper)
    {
        this.mapper = mapper;
    }

    public void Save(string path, GameSession session)
    {
        SavedGameState snapshot = mapper.Capture(session);
        File.WriteAllText(path, JsonSerializer.Serialize(snapshot, Options));
    }

    public GameSession Load(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("Save file was not found.", path);

        string json = File.ReadAllText(path);
        SavedGameState snapshot = JsonSerializer.Deserialize<SavedGameState>(json, Options)
            ?? throw new InvalidDataException("Save file is empty or invalid.");

        return mapper.Restore(snapshot);
    }
}
