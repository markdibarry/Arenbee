using Arenbee.Framework.Enums;
using Arenbee.Framework.Input;

namespace Arenbee.Framework.GUI
{
    public partial class OptionSubMenu : SubMenu
    {
        private Direction _currentDirection;
        private float _rapidScrollTimer;
        private bool _rapidScrollTimerEnabled;
        private readonly float _rapidScrollDelay = 0.4f;
        private readonly float _rapidScrollInterval = 0.05f;

        public override void HandleInput(GUIInputHandler menuInput, float delta)
        {
            base.HandleInput(menuInput, delta);

            if (CurrentContainer == null)
                return;
            if (menuInput.Enter.IsActionJustPressed)
            {
                CurrentContainer.SelectItem();
                return;
            }

            var newDirection = Direction.None;

            if (menuInput.Up.IsActionPressed)
                newDirection = Direction.Up;
            else if (menuInput.Down.IsActionPressed)
                newDirection = Direction.Down;
            else if (menuInput.Left.IsActionPressed)
                newDirection = Direction.Left;
            else if (menuInput.Right.IsActionPressed)
                newDirection = Direction.Right;

            HandleRapidScroll(delta, newDirection);
        }

        private void HandleRapidScroll(float delta, Direction newDirection)
        {
            if (newDirection == _currentDirection)
            {
                if (!_rapidScrollTimerEnabled)
                    return;
                if (_rapidScrollTimer > 0)
                {
                    _rapidScrollTimer -= delta;
                    return;
                }

                _rapidScrollTimer = _rapidScrollInterval;
                CurrentContainer.FocusDirection(_currentDirection);
                return;
            }

            _currentDirection = newDirection;
            if (newDirection == Direction.None)
            {
                _rapidScrollTimerEnabled = false;
                return;
            }

            _rapidScrollTimerEnabled = true;
            _rapidScrollTimer = _rapidScrollDelay;
            CurrentContainer.FocusDirection(_currentDirection);
        }
    }
}
