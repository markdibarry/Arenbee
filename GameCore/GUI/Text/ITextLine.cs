using System.Collections.Generic;

namespace GameCore.GUI;

public interface ITextLine
{
    List<TextEvent> Events { get; }
    string Text { get; set; }
}
