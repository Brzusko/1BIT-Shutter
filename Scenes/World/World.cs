using Godot;
using Godot.Collections;
using System;
using bit_shuter.Scenes.Interfaces;

public class World : Node2D, IMap
{
    private System.Collections.Generic.Stack<Dictionary<string, object>> _worldStates = new System.Collections.Generic.Stack<Dictionary<string, object>>();
    private System.Collections.Generic.Stack<ulong> _worldTimestamps= new System.Collections.Generic.Stack<ulong>();
    private readonly float _renderConstant = 100.0f;
	private Dictionary<string, object> _currentGameWorld;
    private Clock _clock;
	public void Setup(Dictionary<string, object> worldData) {
		var players = (Godot.Collections.Array)worldData["P"]; 
		GetNode<Players>("Players").CreatePlayers(players);
		GetNode<Network>("/root/Network").GameSceneLoaded();
        _clock = GetNode<Clock>("/root/Clock");
	}

	public void ProcessStates(float delta) {
		if(_currentGameWorld == null) return;
		GetNode<Players>("Players").ProcessPlayers((Godot.Collections.Array)_currentGameWorld["P"]);
	}

	public override void _PhysicsProcess(float delta) {
        if (_worldStates.Count < 2) return;
        var renderTime = _clock.Time - _renderConstant;
        var nearestWorldState = _worldStates.Pop();
        var pastWorldState = _worldStates.Pop();
        var t1 = _worldTimestamps.Pop();
        var t2 = _worldTimestamps.Pop();
        var interpolationFactor = (renderTime - t2) / (t2 - t1);
        GD.Print(interpolationFactor);
        
    }
	[Remote]
	public void ReciveGameState(Dictionary<string, object> newGameWorld, ulong timeStamp) {
        _worldTimestamps.Push(timeStamp);
        _worldStates.Push(newGameWorld);
    }
	public void Destroy() => Free();
}
