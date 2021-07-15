using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class PlayerData : NetworkBehaviour
{
    [SerializeField]
    private Transform handTransform;
    public Vector3 PickupPoint { get => handTransform.position; }
}
