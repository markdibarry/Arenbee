﻿using Godot;

namespace GameCore.Input;

public abstract class InputHandler
{
    protected InputHandler()
    {
        Up = new InputAction(this);
        Down = new InputAction(this);
        Left = new InputAction(this);
        Right = new InputAction(this);
    }

    public InputAction Up { get; protected set; }
    public InputAction Down { get; protected set; }
    public InputAction Left { get; protected set; }
    public InputAction Right { get; protected set; }
    public bool UserInputDisabled { get; set; }

    public Vector2 GetLeftAxis()
    {
        Vector2 vector;
        vector.X = Right.ActionStrength - Left.ActionStrength;
        vector.Y = Down.ActionStrength - Up.ActionStrength;
        return vector.Normalized();
    }

    public void SetLeftAxis(Vector2 newVector)
    {
        Down.ActionStrength = Mathf.Max(newVector.Y, 0);
        Up.ActionStrength = Mathf.Min(newVector.Y, 0) * -1;
        Right.ActionStrength = Mathf.Max(newVector.X, 0);
        Left.ActionStrength = Mathf.Min(newVector.X, 0) * -1;
    }

    public virtual void Update()
    {
        Up.ClearOneTimeActions();
        Down.ClearOneTimeActions();
        Left.ClearOneTimeActions();
        Right.ClearOneTimeActions();
    }
}
