namespace GameCore.GUI;

public interface ITextLine
{
    TextEvent[] Events { get; }
    string Text { get; set; }
}
