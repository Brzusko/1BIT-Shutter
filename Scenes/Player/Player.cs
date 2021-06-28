using Godot;
using Godot.Collections;
using System;
using bit_shuter.Scenes.Interfaces;

public class Player : KinematicBody2D, ISynchronized
{
	public bool IsLocal { get => _isLocalPlayer; }
	private Network _network;
	private Dictionary _currentstate;
	private bool _isLocalPlayer = false;

	public void ProcessState(Dictionary state) {

	}

	public void Build(Dictionary data) {
		_network = GetNode<Network>("/root/Network");
		_currentstate = data;
		var playerName = (string)data["n"];
		var rotation = (float)data["r"];
		var look = (bool)data["l"];
		_isLocalPlayer = _network.UserName == playerName;
		Name = playerName;
		GetNode<Camera2D>("Camera2D").Current = _isLocalPlayer;
	}

	public void SendInput(Vector2 velocity) => RpcId(1, "GetPlayerInput", velocity, Vector2.Zero, false);
	public void UpdateStartingPosition() {
		var pos = (Vector2)_currentstate["p"];
		GlobalPosition = pos;
	}
}
