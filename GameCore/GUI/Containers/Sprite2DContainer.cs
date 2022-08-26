using Godot;

namespace GameCore.GUI
{
    [Tool]
    public partial class Sprite2DContainer : Container
    {
        public Sprite2D Sprite2D { get; set; }

        public override void _Ready()
        {
            SetNewChild();
            ChildEnteredTree += OnChildEntered;
            ChildExitingTree += OnChildExiting;
        }

        private void SetNewChild()
        {
            var childCount = GetChildCount();
            if (childCount > 0)
            {
                if (Sprite2D == null)
                    Sprite2D = GetChildOrNull<Sprite2D>(0);
                if (Sprite2D == null || childCount > 1)
                    GD.PrintErr("Sprite2DContainer should have one child and it must be of type Sprite2D.");
                else
                    Sprite2D.ItemRectChanged += UpdateSize;
            }
            UpdateSize();
        }

        private Vector2 GetNewSize()
        {
            if (Sprite2D?.Visible != true)
                return Vector2.Zero;
            float v = 0;
            float h = 0;
            Rect2 rect = Sprite2D.GetRect();
            var spritePos = Sprite2D.Position + rect.Size;
            if (spritePos.x > h)
                h = spritePos.x;
            if (spritePos.y > v)
                v = spritePos.y;
            return new Vector2(h, v);
        }

        private void OnChildEntered(Node node)
        {
            SetNewChild();
        }

        private void OnChildExiting(Node node)
        {
            if (Sprite2D == node)
            {
                Sprite2D.ItemRectChanged -= UpdateSize;
                Sprite2D = null;
            }
            SetNewChild();
        }

        private void UpdateSize()
        {
            CustomMinimumSize = GetNewSize();
        }
    }
}