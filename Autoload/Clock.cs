using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Clock : Node
{
    public ulong Time {
        get => _time;
        private set => _time = value;
    }
    private float _rtt = 0.0f;
    private float _deltaLatenacy = 0.0f;
    private float _reminder = 0.0f;
    private List<float> _latenacyList = new List<float>();
    private int _timer_iteration = 0;

    private ulong _time = 0;
    private readonly int _timer_max_iterations = 5;
    private Timer _timer;

    private bool _isSynced = false;

    private void CreateTimer() {
        _timer = new Timer();
        _timer.WaitTime = 1;
        _timer.Connect("timeout", this, "SendPing");
        AddChild(_timer);
        _timer.Start();
    }
    
    private void DestroyTimer() {
        _timer.Disconnect("timeout", this, "SendPing");
        _timer.QueueFree();
        _timer = null;
    }

    private void RestartClock() {
        DestroyTimer();
        SetPhysicsProcess(false);
        _latenacyList.Clear();
        _rtt = 0;
        _timer_iteration = 0;
        _time = 0;
        _reminder = 0;
        _deltaLatenacy = 0;
        _isSynced = false;
    }

    public void SendPing() {
        RpcId(1, "RecivePing", OS.GetSystemTimeMsecs());
        _timer_iteration++;
    }

    [Remote]
    public void RecivePong(ulong serverTime, ulong clientTime) {
        if(!_isSynced) {
            _time = serverTime + (OS.GetSystemTimeMsecs() - clientTime) / 2;
            _rtt = (OS.GetSystemTimeMsecs() - clientTime) / 2;
            GetNode<Network>("/root/Network").ClockSyncFinished();
            _isSynced = true;
        }

        var letanecy = (OS.GetSystemTimeMsecs() - clientTime) / 2;
        _latenacyList.Add(letanecy);

        if(_latenacyList.Count >= _timer_max_iterations) {
            var sum = 0.0f;
            _latenacyList.Sort();
            var midPoint = _latenacyList[(int)(_latenacyList.Count / 2)];
            foreach(var current_letanecy in _latenacyList.Reverse<float>()) {
                if (current_letanecy >= (2 * midPoint) && current_letanecy >= 50) _latenacyList.Remove(current_letanecy);
                else sum += current_letanecy;
            }
            _deltaLatenacy = (sum / _latenacyList.Count) - _rtt;
            _rtt = (int)letanecy / _latenacyList.Count;
            _latenacyList.Clear();
        }
    }
    public void StartClockSyncing() {
        SendPing();
        CreateTimer();
        SetPhysicsProcess(true);
    }
    public override void _Ready() {
        SetPhysicsProcess(false);
        GetNode<Network>("/root/Network").Connect("DisconnectedFromServer", this, nameof(RestartClock));
    }

    public void OnDisconnect() => RestartClock();

    public override void _PhysicsProcess(float delta)
    {
        if (_time == 0) return;

        _time += (ulong)((delta * 1000) + _deltaLatenacy);
        _reminder += (delta * 100) - (int)(delta * 1000);

        _deltaLatenacy -= _deltaLatenacy;

        if(_reminder >= 1.0) {
            _time += 1;
            _reminder -= 1;
        }
    }
}
