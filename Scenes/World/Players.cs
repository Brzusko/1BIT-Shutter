using Godot;
using Godot.Collections;
using bit_shuter.Scenes.Interfaces;

public class Players : Node2D
{
    [Export] 
    private PackedScene _playerScene;
    private System.Collections.Generic.Dictionary<string, Player> _activePlayers = new System.Collections.Generic.Dictionary<string, Player>();
    public void CreatePlayers(Array players) {
        foreach(Dictionary player in players) {
            var playerInstance = _playerScene.Instance() as Player;
            _activePlayers.Add((string)player["n"], playerInstance);
            AddChild(playerInstance);
            playerInstance.Build(player);
            playerInstance.UpdateStartingPosition();
        }
    }

    public void ProcessPlayers(Array players) {
        foreach(var player in players) {
            var playerAsDict = (Dictionary)player;
            var playerName = (string)playerAsDict["n"];
            GD.Print("Muving");
            if (!_activePlayers.ContainsKey(playerName)) continue;
            var currentPlayer = _activePlayers[playerName];
            currentPlayer.GlobalPosition = (Vector2)playerAsDict["p"];
        }       
    } 
}
