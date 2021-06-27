using Godot;
using System;
using System.Collections.Generic;
using bit_shuter.Scenes.Interfaces;

public class Main : Node2D
{
    private Dictionary<string, string> _maps;
    private IMap _currentMap;
    private Network _network;

    public override void _Ready()
    {
        _network = GetNode<Network>("/root/Network");
        _network.Connect("RequestGameSceneChange", this, nameof(OnRequestGameSceneChange));
        _maps = new Dictionary<string, string> {
            { "World", "res://Scenes/World/World.tscn" }
        };
    }

    public void OnRequestGameSceneChange(string sceneName, Godot.Collections.Dictionary<string, object> sceneData) {
        if(!_maps.ContainsKey(sceneName)) return;
        _currentMap?.Destroy();
        var loadedScene = GD.Load<PackedScene>(_maps[sceneName]);
        var loadedSceneInstance = loadedScene.Instance();
        AddChild(loadedSceneInstance);
        var sceneAsInterface = (IMap)loadedSceneInstance;
        sceneAsInterface.Setup(sceneData);
        _currentMap = sceneAsInterface;
    }
}
