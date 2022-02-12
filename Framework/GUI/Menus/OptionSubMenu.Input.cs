using Arenbee.Framework.Enums;
using Arenbee.Framework.Game;
using Godot;

namespace Arenbee.Framework.GUI
{
    public partial class OptionSubMenu : SubMenu
    {
        private Direction _currentDirection;
        private float _rapidScrollTimer;
        private bool _rapidScrollTimerEnabled;
        private readonly float _rapidScrollDelay = 0.4f;
        private readonly float _rapidScrollInterval = 0.05f;

        public override void _PhysicsProcess(float delta)
        {
            if (Engine.IsEditorHint() || !IsActive) return;
            base._PhysicsProcess(delta);

            var menuInput = GameRoot.MenuInput;
            var newDirection = Direction.None;

            if (menuInput.Up.IsActionPressed)
            {
                newDirection = Direction.Up;
            }
            else if (menuInput.Down.IsActionPressed)
            {
                newDirection = Direction.Down;
            }
            else if (menuInput.Left.IsActionPressed)
            {
                newDirection = Direction.Left;
            }
            else if (menuInput.Right.IsActionPressed)
            {
                newDirection = Direction.Right;
            }
            else if (menuInput.Enter.IsActionJustPressed)
            {
                _currentContainer.SelectItem();
            }

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
                    _currentContainer.FocusUp();
                    break;
                case Direction.Down:
                    _currentContainer.FocusDown();
                    break;
                case Direction.Left:
                    _currentContainer.FocusLeft();
                    break;
                case Direction.Right:
                    _currentContainer.FocusRight();
                    break;
            }
        }
    }
}
