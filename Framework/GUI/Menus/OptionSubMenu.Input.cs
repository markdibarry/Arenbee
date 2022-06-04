using Arenbee.Framework.Enums;

namespace Arenbee.Framework.GUI
{
    public partial class OptionSubMenu : SubMenu
    {
        private Direction _currentDirection;
        private float _rapidScrollTimer;
        private bool _rapidScrollTimerEnabled;
        private readonly float _rapidScrollDelay = 0.4f;
        private readonly float _rapidScrollInterval = 0.05f;

        public override void HandleInput(float delta)
        {
            base.HandleInput(delta);

            if (CurrentContainer == null)
                return;
            if (MenuInput.Enter.IsActionJustPressed)
            {
                CurrentContainer.SelectItem();
                return;
            }

            var newDirection = Direction.None;

            if (MenuInput.Up.IsActionPressed)
                newDirection = Direction.Up;
            else if (MenuInput.Down.IsActionPressed)
                newDirection = Direction.Down;
            else if (MenuInput.Left.IsActionPressed)
                newDirection = Direction.Left;
            else if (MenuInput.Right.IsActionPressed)
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
