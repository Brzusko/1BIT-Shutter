using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Mirror;
public class PlayerAreaDetection : NetworkBehaviour
{
    public event EventHandler<HeldEvArgs> HeldUpdate;
    public event EventHandler HeldStarted;
    public event EventHandler HeldReleased;
    // AKA HAND
    [SerializeField] private Transform _hand;
    [SerializeField] public GameObject _ball;
    [SerializeField] private GameObject _ballPrefab;
    [SerializeField] private GameObject _ballNetworkedPrefab;
    [SerializeField] private float _maximumHeldTime = 3.5f;
    [SerializeField] private float _eventStepTreshold = 0.5f;
    [SerializeField] private Heldbar _heldbar;
    [SerializeField] private float _baseForceMultipler = 100f;

    private float _heldTime = 0.0f;
    private Coroutine _forceCorountine = null;
    private bool _isMouseBTNClicked = false;

    [SyncVar(hook = nameof(OnCurrentBallChange))]
    public NetworkIdentity _currentBall;
    
    [ClientCallback]
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag != "Ball") return;
        var ball = other.GetComponent<Ball>();
        if (ball.NetID.netId != netId)
        {
            CMDRespawn(ball.NetID);
            return;
        }

        ball.PickUP();
        Pickup();
    }

    private void Update() {
        if(!hasAuthority) return;
        if(_currentBall != null) return;
        if(Input.GetMouseButtonDown(0) && _forceCorountine == null) 
            _forceCorountine = StartCoroutine(CalculateForce());
        if(Input.GetMouseButtonUp(0))
            _isMouseBTNClicked = false;
    }
    private void OnCurrentBallChange(NetworkIdentity oldBall, NetworkIdentity newBall)
    {
        if(newBall == null)
        {
            var newBallInHand = Instantiate(_ballPrefab, _hand.transform.position, Quaternion.identity);
            newBallInHand.transform.parent = _hand;
            newBallInHand.transform.localScale = new Vector3(0.3f, 0.3f, 1.0f);
            _ball = newBallInHand;
        }
        else
        {
            Destroy(_ball);
            _ball = null;
        }
    }

    [Command]
    public void SpawnBall(Vector3 startingPos, Quaternion rot, Vector3 direction, float force)
    {
        var obj = Instantiate(_ballNetworkedPrefab, startingPos, rot);
        var ball = obj.GetComponent<Ball>();
        ball.SetOwner(netIdentity);
        NetworkServer.Spawn(obj, connectionToClient);
        SetCurrentBall(ball.netIdentity);
        ball.Shoot(transform.up, force);
    }

    [Command]
    public void Pickup() => _currentBall = null;

    [Command]
    public void CMDRespawn(NetworkIdentity enemyPlayer) => PointManager.Singleton.RespawnPlayer(netIdentity, enemyPlayer);

    [Server]
    public void SetCurrentBall(NetworkIdentity id) =>_currentBall = id;

    public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {
        if (initialState)
        {
            writer.WriteNetworkIdentity(_currentBall);
        }
        return base.OnSerialize(writer, initialState);
    }

    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        if (initialState)
        {
            _currentBall = reader.ReadNetworkIdentity();
        }
        base.OnDeserialize(reader, initialState);
    }

    private void InvokeStep(float currentStep) => HeldUpdate?.Invoke(this, new HeldEvArgs {
        HasBeenReleased = false,
        JustStarted = false,
        TotalWaitTime = _maximumHeldTime,
        StepTreshold = _eventStepTreshold,
        CurrentStep = currentStep
    });

    public override void OnStartClient()
    {
        base.OnStartClient();
        if(!hasAuthority) 
        {
            _heldbar.DisableHeldBar();
            return;
        }
        _heldbar.RegisterEvents(this);
    }
    private IEnumerator CalculateForce()
    {
        HeldStarted?.Invoke(this, null);

        _isMouseBTNClicked = true;
        float currentStep = 0;

        while(_isMouseBTNClicked && _heldTime <= _maximumHeldTime)
        {
            var delta = Time.deltaTime;
            _heldTime += delta;
            currentStep += delta;

            if(currentStep >= _eventStepTreshold)
            {
                InvokeStep(_heldTime);
                currentStep = 0.0f;
            }

            yield return null;
        }
        
        SpawnBall(transform.position + transform.up * 1.5f, Quaternion.identity, transform.up, _heldTime * _baseForceMultipler);
        _heldTime = 0;
        _forceCorountine = null;
        HeldReleased?.Invoke(this, null);

    }

    public class HeldEvArgs : EventArgs {
        public bool HasBeenReleased {get; set;}
        public bool JustStarted {get; set;}
        public float TotalWaitTime {get; set;}
        public float StepTreshold {get; set;}
        public float CurrentStep {get; set;}
    }

    [TargetRpc]
    public void ReciveNewPos(NetworkConnection who, Vector3 newPos)
    {
        if(who.identity != netIdentity) return;
        this.transform.position = newPos;
    }
}
