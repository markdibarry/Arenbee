namespace Arenbee.Framework.Input
{
    public class InputAction
    {
        public InputAction() { }
        public InputAction(string alias)
        {
            Alias = alias;
        }

        public string Alias { get; set; }
        public bool IsActionPressed
        {
            get
            {
                if (string.IsNullOrEmpty(Alias))
                    return false;
                return Godot.Input.IsActionPressed(Alias);
            }
        }

        public bool IsActionJustPressed
        {
            get
            {
                if (string.IsNullOrEmpty(Alias))
                    return false;
                return Godot.Input.IsActionJustPressed(Alias);
            }
        }

        public bool IsActionJustReleased
        {
            get
            {
                if (string.IsNullOrEmpty(Alias))
                    return false;
                return Godot.Input.IsActionJustReleased(Alias);
            }
        }
    }
}