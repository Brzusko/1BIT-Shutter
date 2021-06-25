using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System;
using bit_shuter.Scenes.UI.Interfaces;
public class Lobby : Control, IUI
{
	private List<string> _players = new List<string>();
	private Button _readyBTN;
	private VBoxContainer _playersContainer;
	private Label _gameStateInfo;
	private Network _network;
    private bool _isPlayerReady = false;

	[Export]
	private NodePath _readyBTNPath;
	[Export]
	private NodePath _playersContainerPath;
	[Export]
	private NodePath _gameStateInfoPath;
	public void Active() {
		_network = GetNode<Network>("/root/Network");
		_network.Connect("LobbyStateChanged", this, nameof(OnLobbyStateChanged));
        _readyBTN.Connect("pressed", this, nameof(OnReadyBTNClick));
		_network.LobbyLoaded();
		Show();
	}

	public void Disactive() {
		_network = null;
		Hide();
	}

	public override void _Ready()
	{
		_readyBTN = GetNode<Button>(_readyBTNPath);
		_playersContainer = GetNode<VBoxContainer>(_playersContainerPath);
		_gameStateInfo = GetNode<Label>(_gameStateInfoPath);
	}

    public void OnReadyBTNClick() {
        _isPlayerReady = !_isPlayerReady;
        _network.SendReadyState(_isPlayerReady);
    }
	public void OnLobbyStateChanged(Godot.Collections.Dictionary<string, object> newState) {
		GD.Print("Recived state");
		var timerStarted = (bool)newState["ts"];
		var players = (Godot.Collections.Array)newState["c"];

		if (!timerStarted) {
			_gameStateInfo.Text = "Waiting for connections";
		} 
		else {
			var timeLeft = newState["t"];
			_gameStateInfo.Text = $"Game is starting in {(int)timeLeft}";
		}

		foreach(var player in players) {
			
			var playerAsDict = (Godot.Collections.Dictionary)player;
            var playerName = (string)playerAsDict["ClientName"];
            var playerState = (int)playerAsDict["State"];
            var playerDisplayState = playerState == 3 ? "Not Ready" : "Ready";

			if (_players.FindIndex(pName => pName == playerName) == -1) {
                var label = new Label();
                label.Name = playerName;
                label.Text = $"{playerDisplayState} - {playerName}";
                _playersContainer.AddChild(label);
                _players.Add(playerName);
			} else {
                var label = _playersContainer.GetNode<Label>(playerName);
                label.Name = playerName;
                label.Text = $"{playerDisplayState} - {playerName}";
            }
		}

        var playersToRemove = new Stack<string>();
        foreach(var player in _players) {
            bool playerExist = false;
            foreach(var _player in players) {
                var _platerAsDict = (Godot.Collections.Dictionary)_player;
                var playerName = (string)_platerAsDict["ClientName"];
                if (player == playerName) {
                    playerExist = true;
                    break;
                }
            }
            if (!playerExist) {
                playersToRemove.Push(player);
            }
        }

        while(playersToRemove.Count != 0) {
            var playerToRemove = playersToRemove.Pop();
            _players.Remove(playerToRemove);
            var label = _playersContainer.GetNode<Label>(playerToRemove);
            label.Free();
        }

	}
	
}
