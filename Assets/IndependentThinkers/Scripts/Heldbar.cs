using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Heldbar : MonoBehaviour
{
    private readonly Quaternion _defaultRotation = Quaternion.Euler(0, 0, 90.0f);
    [SerializeField]
    private float _offset = 0.65f;

    [SerializeField]
    private Canvas _canvas;
    [SerializeField]
    private Transform _fill;
    // Update is called once per frame

    void Update()
    {
        transform.position = transform.parent.transform.position + (Vector3.right * _offset);
        transform.rotation = _defaultRotation;
    }

    public void RegisterEvents(PlayerAreaDetection hand)
    {
        _canvas.enabled = false;
        if(hand == null) return;
        hand.HeldUpdate += OnHeldUpdate;
        hand.HeldStarted += OnHeldStart;
        hand.HeldReleased += OnHeldStop;
    }

    public void DisableHeldBar()
    {
        _canvas.enabled = false;
        enabled = false;
    }
    protected virtual void OnHeldStart(object sender, EventArgs args)
    {
        Debug.Log("started");
        _canvas.enabled = true;
        _fill.localScale = new Vector3(0, 1, 0);
    }

    protected virtual void OnHeldStop(object sender, EventArgs args)
    {
        _fill.localScale = new Vector3(0, 1, 0);
        _canvas.enabled = false;
    }
    protected virtual void OnHeldUpdate(object sender, PlayerAreaDetection.HeldEvArgs arg)
    {
        var stepCounts = arg.TotalWaitTime / arg.StepTreshold;
        var scaleStep = 1.0f / stepCounts;

        if(_fill.localScale.x == 0)
            _fill.localScale += new Vector3(scaleStep, 0, 0);
        _fill.localScale += new Vector3(scaleStep, 0, 0);

    }
}
