using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ShuterNetworkManager : NetworkManager
{
    public event EventHandler<PlayersCounter> PlayersCountChanged;
    [SerializeField] private GameObject _ball;
    private int _connectedPlayers = 0;
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        CreatePlayer(conn);
    }   

    [Server]
    private void CreatePlayer(NetworkConnection conn)
    {
         _connectedPlayers++;
        var playerLook = conn.identity.GetComponent<PlayerLook>();
        var playerhand = conn.identity.GetComponent<PlayerAreaDetection>();
        playerLook.ChangeLook(_connectedPlayers % 2 == 0);

        var ball = Instantiate(_ball);
        ball.transform.position = playerLook.transform.position;
        NetworkServer.Spawn(ball, conn);
        ball.GetComponent<Ball>().SetOwner(conn.identity);
        PlayersCountChanged?.Invoke(this, new PlayersCounter{ CurrentPlayers = _connectedPlayers });
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        _connectedPlayers--;
        PlayersCountChanged?.Invoke(this, new PlayersCounter{ CurrentPlayers = _connectedPlayers });
    }

    public class PlayersCounter : EventArgs {
        public int CurrentPlayers {get; set;}
    }
}
