using Godot;
using Godot.Collections;
using System;
using bit_shuter.Scenes.Interfaces;

public class World : Node2D, IMap
{
    public void Setup(Dictionary<string, object> worldData) {
        var players = (Godot.Collections.Array)worldData["P"];
        GetNode<Players>("Players").CreatePlayers(players);
    }

    public void Destroy() => Free();
}
