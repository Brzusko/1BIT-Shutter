using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerLook : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnPlayerLookChange))]
    private bool _playerLook = false;

    [SerializeField]
    private Sprite _playerOtherLook;

    public void ChangeLook(bool look) => _playerLook = look;
    public void OnPlayerLookChange(bool old, bool newLook)
    {
        if(newLook)
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = _playerOtherLook;
            spriteRenderer.flipX = true;
        }
    }
}
