using Godot;

namespace Arenbee.Framework.GUI
{
    [Tool]
    public partial class HandCursor : Cursor
    {
        public static new readonly string ScenePath = $"res://Framework/GUI/Cursors/{nameof(HandCursor)}.tscn";
        private readonly float _cursorTimerOut = 1.0f;
        private float _cursorTimer = 0;

        public override void HandleCursorAnimation(float delta)
        {
            base.HandleCursorAnimation(delta);
            _cursorTimer += delta;
            if (_cursorTimer > _cursorTimerOut)
            {
                if (Sprite2D.Position.x < 0)
                    Sprite2D.Position = Vector2.Zero;
                else
                    Sprite2D.Position = new Vector2(-1, 0);
                _cursorTimer = 0;
            }
        }
    }
}