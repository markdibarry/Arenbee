using GameCore.Actors;
using GameCore.Extensions;
using GameCore.GUI;
using Godot;

namespace Arenbee.Actors;

public class StateDisplayController
{
    private Label _airStateDisplay = null!;
    private Label _moveStateDisplay = null!;
    private Label _actionStateDisplay = null!;
    private Label _healthStateDisplay = null!;

    public void CreateStateDisplay(AActorBody actor)
    {
        var stateDisplay = GDEx.Instantiate<Control>(StateDisplay.GetScenePath());
        _airStateDisplay = stateDisplay.GetNode<Label>("AirState");
        _moveStateDisplay = stateDisplay.GetNode<Label>("MoveState");
        _actionStateDisplay = stateDisplay.GetNode<Label>("ActionState");
        _healthStateDisplay = stateDisplay.GetNode<Label>("HealthState");
        _moveStateDisplay.Text = string.Empty;
        _airStateDisplay.Text = string.Empty;
        _actionStateDisplay.Text = string.Empty;
        _healthStateDisplay.Text = string.Empty;
        actor.AddChild(stateDisplay);
        Vector2 frameSize = actor.BodySprite.GetFrameSize();
        stateDisplay.Position = new Vector2(actor.BodySprite.Position.X, frameSize.Y / 2 * -1 - 10 + actor.BodySprite.Position.Y);
    }

    public void Update(StateController stateController)
    {
        _moveStateDisplay.Text = stateController.MoveStateMachine.State.GetType().Name;
        _airStateDisplay.Text = stateController.AirStateMachine.State.GetType().Name;
        _actionStateDisplay.Text = stateController.BaseActionStateMachine.State.GetType().Name;
        _healthStateDisplay.Text = stateController.HealthStateMachine.State.GetType().Name;
    }
}
