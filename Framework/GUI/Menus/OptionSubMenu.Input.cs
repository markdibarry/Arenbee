using Arenbee.Framework.Enums;
using Arenbee.Framework.Extensions;

namespace Arenbee.Framework.GUI
{
    public partial class OptionSubMenu : SubMenu
    {
        private Direction _currentDirection;
        private float _rapidScrollTimer;
        private bool _rapidScrollTimerEnabled;
        private readonly float _rapidScrollDelay = 0.4f;
        private readonly float _rapidScrollInterval = 0.05f;

        public override void _Process(float delta)
        {
            if (this.IsToolDebugMode() || !IsActive) return;
            base._Process(delta);

            var newDirection = Direction.None;

            if (MenuInput.Up.IsActionPressed)
                newDirection = Direction.Up;
            else if (MenuInput.Down.IsActionPressed)
                newDirection = Direction.Down;
            else if (MenuInput.Left.IsActionPressed)
                newDirection = Direction.Left;
            else if (MenuInput.Right.IsActionPressed)
                newDirection = Direction.Right;
            else if (MenuInput.Enter.IsActionJustPressed)
                CurrentContainer.SelectItem();

            HandleRapidScroll(delta, newDirection);
        }

        private void HandleRapidScroll(float delta, Direction newDirection)
        {
            if (newDirection == _currentDirection)
            {
                if (_rapidScrollTimerEnabled)
                {
                    if (_rapidScrollTimer > 0)
                    {
                        _rapidScrollTimer -= delta;
                    }
                    else
                    {
                        _rapidScrollTimer = _rapidScrollInterval;
                        ScrollDirection(_currentDirection);
                    }
                }
            }
            else
            {
                _currentDirection = newDirection;
                if (newDirection != Direction.None)
                {
                    _rapidScrollTimerEnabled = true;
                    _rapidScrollTimer = _rapidScrollDelay;
                    ScrollDirection(_currentDirection);
                }
                else
                {
                    _rapidScrollTimerEnabled = false;
                }
            }
        }

        private void ScrollDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    CurrentContainer?.FocusUp();
                    break;
                case Direction.Down:
                    CurrentContainer?.FocusDown();
                    break;
                case Direction.Left:
                    CurrentContainer?.FocusLeft();
                    break;
                case Direction.Right:
                    CurrentContainer?.FocusRight();
                    break;
            }
        }
    }
}
