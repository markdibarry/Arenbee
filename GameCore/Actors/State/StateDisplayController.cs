using GameCore.Extensions;
using GameCore.GUI;
using Godot;

namespace GameCore.Actors;

public class StateDisplayController
{
    private Label _airStateDisplay;
    private Label _moveStateDisplay;
    private Label _actionStateDisplay;
    private Label _healthStateDisplay;

    public void CreateStateDisplay(ActorBase actor)
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
        stateDisplay.Position = new Vector2(actor.BodySprite.Position.x, (frameSize.y / 2 * -1) - 10 + actor.BodySprite.Position.y);
    }

    public void Update(StateControllerBase stateController)
    {
        _moveStateDisplay.Text = stateController.MoveStateMachine.State.GetType().Name;
        _airStateDisplay.Text = stateController.AirStateMachine.State.GetType().Name;
        _actionStateDisplay.Text = stateController.ActionStateMachine.State.GetType().Name;
        _healthStateDisplay.Text = stateController.HealthStateMachine.State.GetType().Name;
    }
}
