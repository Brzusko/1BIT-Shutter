using Godot;
using System;
using System.Collections.Generic;
using bit_shuter.Scenes.UI.Interfaces;

public class UIHandler : CanvasLayer
{
    private const string _startingScene = "MainMenu";
    private Network _network;
    private Dictionary<string, IUI> _uiScenes;

    private IUI _currentUI;
    public override void _Ready()
    {
        _network = GetNode<Network>("/root/Network");
        _network.Connect("RequestUIChange", this, nameof(OnUIChangeRequest));
        _uiScenes = new Dictionary<string, IUI>{
            { nameof(MainMenu), GetNode<IUI>("MainMenu") },
            { nameof(Lobby), GetNode<IUI>("Lobby")}
        };
        _currentUI = _uiScenes[_startingScene];
    }

    public void OnUIChangeRequest(string className) {
        if (!_uiScenes.ContainsKey(className)) return;
        _currentUI?.Disactive();
        _currentUI = _uiScenes[className];
        _currentUI?.Active();
    }
}
