using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Mirror;

public class Clock : NetworkBehaviour
{
    [SyncVar(hook = nameof(UpdateText))]
    private int _seconds = 1;
    [SyncVar(hook = nameof(UpdateText))]
    private int _minutes = 10;

    [SerializeField]
    private Canvas _inGameUI;

    [SerializeField]
    private TMP_Text _timerText;

    private Coroutine _timerCoroutine;
    public override void OnStartServer()
    {        
        if(isServerOnly)
        {
            Destroy(_inGameUI);
        }
    }

    public void StartClock() {
        if(_timerCoroutine != null) return;
        _timerCoroutine = StartCoroutine(Timer());
    }

    public void UpdateText(int oldVal = 0, int newVal = 0)
    {
        Debug.Log($"{oldVal}, {newVal}");
        _timerText.text = $"{_minutes} : {_seconds}";
    }
    public override void OnStartClient() => UpdateText();

    public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {
        if(initialState)
        {
            writer.WriteInt(_seconds);
            writer.WriteInt(_minutes);
        }
        return base.OnSerialize(writer, initialState);
    }

    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        if(initialState)
        {
            _seconds = reader.ReadInt();
            _minutes = reader.ReadInt();
        }
        base.OnDeserialize(reader, initialState);
    }

    [Server]
    private IEnumerator Timer()
    {
        while(_minutes > 0)
        {
            _seconds--;
            if(_seconds <= 0)
            {
                _seconds = 60;
                _minutes--;
            }
            yield return new WaitForSeconds(1.0f);
        }
        // do something when minutes comes to 0
    }
}
