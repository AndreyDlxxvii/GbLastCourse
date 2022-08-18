using System;
using UnityEngine;
using UnityEngine.Networking;
[RequireComponent(typeof(CharacterController))]
public abstract class Character : NetworkBehaviour
{
    protected Action OnUpdateAction { get; set; }
    protected abstract FireAction fireAction { get; set; }
    [SyncVar] protected Vector3 serverPosition;
    [SyncVar] protected Quaternion serverRotation;
    [SyncVar] protected int serverHealth;
    protected virtual void Initiate()
    {
        OnUpdateAction += Movement;
    }
    private void Update()
    {
        OnUpdate();
    }
    private void OnUpdate()
    {
        OnUpdateAction?.Invoke();
    }
    [Command]
    protected void CmdUpdatePosition(Vector3 position, Quaternion rotation, int health)
    {
        serverPosition = position;
        serverRotation = rotation;
        serverHealth = health;
    }

    [Command]
    protected void CmdShootByClient(RaycastHit hit)
    {
        Debug.Log(hit.collider.tag);
    }
    
    public abstract void Movement();
}