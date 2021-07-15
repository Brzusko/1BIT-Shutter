using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private float _playerSpeed = 5.0f;
    private Vector3 _lastestInput;
    private Camera _camera;
    private bool _canProcessInput = false;
    private Vector3 _latestMousePosition;
    public override void OnStartLocalPlayer()
    {
        _camera = Camera.main;
    }
    private void FixedUpdate() {
        if (!isLocalPlayer || !_canProcessInput) return;
        var v = Input.GetAxis("Vertical");
        var h = Input.GetAxis("Horizontal");
        var velocity = new Vector3(h, v, 0);
        
        transform.position = Vector3.Lerp(transform.position, transform.position + ((velocity * _playerSpeed) * Time.fixedDeltaTime), 0.5f);
        _camera.transform.position = new Vector3(transform.position.x, transform.position.y, _camera.transform.position.z);

        _latestMousePosition = Input.mousePosition - _camera.WorldToScreenPoint(transform.position);
        _latestMousePosition.z = 0;

        var angle = Mathf.Atan2(_latestMousePosition.y, _latestMousePosition.x) * Mathf.Rad2Deg + -90;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);  
    } 

    [TargetRpc]
    public void EnableMovement(NetworkConnection conn) => _canProcessInput = true;

    [TargetRpc]
    public void DisableMovement(NetworkConnection conn) => _canProcessInput = false;
} 
