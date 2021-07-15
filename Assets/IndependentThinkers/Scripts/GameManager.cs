using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class GameManager : NetworkBehaviour
{
    public enum GameState {
        WAITING_FOR_CONNECTIONS,
        GAMEPLAY,
        RESTARTING,
    }
    
    private int _joinedPlayers = 0;

    private readonly int _playersCountToStartGame = 2;

    private GameState _currentGameState = GameState.WAITING_FOR_CONNECTIONS;

    [SerializeField]
    private Clock _clock;

    private void ChangeEventsHandlersOnState(bool connectEvents)
    {
        var netManager = ShuterNetworkManager.singleton as ShuterNetworkManager;
        if(connectEvents)
            netManager.PlayersCountChanged += OnPlayerCountChange;
        else
            netManager.PlayersCountChanged -= OnPlayerCountChange;
    }
    
    [Server]
    public override void OnStartServer() => ChangeEventsHandlersOnState(true);

    [Server]
    public override void OnStopServer() => ChangeEventsHandlersOnState(false);

    [Server]
    public virtual void OnPlayerCountChange(object sender, ShuterNetworkManager.PlayersCounter args)
    {
        _joinedPlayers = args.CurrentPlayers;
        Debug.Log(_joinedPlayers);
        if(_joinedPlayers >= 2 && _currentGameState == GameState.WAITING_FOR_CONNECTIONS)
        {
            var connections = NetworkServer.connections;
            foreach(var conn in connections)
            {
                var playerController = conn.Value.identity.GetComponent<PlayerController>();
                playerController.EnableMovement(conn.Value);
                Debug.Log("Enabling movement");
            }
            _currentGameState = GameState.GAMEPLAY;
        }
    }

}
