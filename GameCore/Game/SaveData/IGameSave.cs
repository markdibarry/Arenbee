using System;

namespace GameCore.SaveData;

public interface IGameSave
{
    int Id { get; }
    DateTime LastModifiedUtc { get; }
}
