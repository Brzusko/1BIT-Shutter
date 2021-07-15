using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PointManager : NetworkBehaviour
{
    private static PointManager _instance;
    public static PointManager Singleton {
        get 
        {
            return _instance;
        }

        private set 
        {
            if(_instance != null) return;
            _instance = value;
        }
    }

    [SerializeField]
    private List<Transform> _spawnPoints;

    public override void OnStartServer()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }

        base.OnStartServer();

        _instance = this;
        DontDestroyOnLoad(this);
    }

    public Vector3 GetStartingPosByPlayersCount(int count) => _spawnPoints[count - 1].position;

    public Vector3 FindSafeSpawn(Vector3 playerOnePos, Vector3 playerTwoPos)
    {
        Vector3 safeSpawn = Vector3.zero;
        foreach(var spawnPoint in _spawnPoints)
        {
            var distanceVector = spawnPoint.position - playerTwoPos;
            if(safeSpawn.magnitude < distanceVector.magnitude)
                safeSpawn = spawnPoint.position;
        }
        return safeSpawn;
    }

    public void RespawnPlayer(NetworkIdentity toRespawn, NetworkIdentity enemy)
    {
        var safeSpawn = FindSafeSpawn(toRespawn.transform.position, enemy.transform.position);
        toRespawn.GetComponent<PlayerAreaDetection>().ReciveNewPos(toRespawn.connectionToClient, safeSpawn);
    }
}
