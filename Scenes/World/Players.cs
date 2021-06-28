using Godot;
using Godot.Collections;
using bit_shuter.Scenes.Interfaces;

public class Players : Node2D
{
    [Export]
    private PackedScene _playerScene;
    public void CreatePlayers(Array players) {
        foreach(Dictionary player in players) {
            var playerInstance = _playerScene.Instance() as Player;
            AddChild(playerInstance);
            playerInstance.Build(player);
            playerInstance.UpdateStartingPosition();
        }
    }
}
