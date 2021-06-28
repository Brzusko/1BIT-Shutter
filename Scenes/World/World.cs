using Godot;
using Godot.Collections;
using System;
using bit_shuter.Scenes.Interfaces;

public class World : Node2D, IMap
{
    private Dictionary<string, object> _currentGameWorld;
    public void Setup(Dictionary<string, object> worldData) {
        var players = (Godot.Collections.Array)worldData["P"]; 
        GetNode<Players>("Players").CreatePlayers(players);
        GetNode<Network>("/root/Network").GameSceneLoaded();
    }

    public void ProcessStates(float delta) {
        //GetNode<Players>("Players").ProcessPlayers((Godot.Collections.Array)_currentGameWorld["P"]);
    }

    public override void _PhysicsProcess(float delta) => ProcessStates(delta);

    public void ReciveGameState(Dictionary<string, object> newGameWorld) => _currentGameWorld = newGameWorld;
    public void Destroy() => Free();
}
