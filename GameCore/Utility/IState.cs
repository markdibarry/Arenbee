﻿namespace GameCore.Utility;

public interface IState
{
    void Enter();
    void Update(double delta);
    void Exit();
    bool TrySwitch(IStateMachine stateMachine);
}
