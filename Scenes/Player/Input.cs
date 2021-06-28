using Godot;
using System;

public class Input : Node2D
{
    private Vector2 _velocity;
    public override void _PhysicsProcess(float delta)
    {
        if(!GetParent<Player>().IsLocal) return;
        _velocity = Vector2.Zero;

        if(Godot.Input.IsActionPressed("ui_up"))
            _velocity += Vector2.Up;
        
        if(Godot.Input.IsActionPressed("ui_down"))
            _velocity += Vector2.Down;
        
        if(Godot.Input.IsActionPressed("ui_left"))
            _velocity += Vector2.Left;
        
        if(Godot.Input.IsActionPressed("ui_right"))
            _velocity += Vector2.Right;

        _velocity = _velocity.Normalized();
        GetParent<Player>().SendInput(_velocity);
    }
}
