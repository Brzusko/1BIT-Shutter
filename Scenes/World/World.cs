using Godot;
using Godot.Collections;
using System;
using bit_shuter.Scenes.Interfaces;

public class World : Node2D, IMap
{
    public void Setup(Dictionary<string, object> worldData) {
        GD.Print(worldData);
    }

    public void Destroy() => Free();
}
