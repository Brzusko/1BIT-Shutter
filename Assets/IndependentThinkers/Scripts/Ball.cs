using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Ball : NetworkBehaviour
{
    public NetworkIdentity NetID { get => _netID; }
    [SyncVar]
    protected NetworkIdentity _netID;

    protected Rigidbody2D _rg;

    [Server]
    public void SetOwner(NetworkIdentity id) => _netID = id;

    [Command]
    public void CMDPickUP() => NetworkServer.Destroy(this.gameObject);

    public void Shoot(Vector3 direction, float force)
    {
        _rg.AddForce(new Vector2(direction.x, direction.y) * force);
    }

    [Client]
    public void PickUP()
    {
        if(!hasAuthority) return;
        CMDPickUP();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        _rg = GetComponent<Rigidbody2D>();
    }

    [ServerCallback]
    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag != "Wall") return;
        _rg.velocity = _rg.velocity * 0.7f;
    }
}
