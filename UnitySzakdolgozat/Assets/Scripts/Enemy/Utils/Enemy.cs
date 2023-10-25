using System;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public Room StartRoom;
    public Transform Player;

    protected virtual void Start() {
        Player = GameManager.Player.transform;
        GameManager.AddEnemy(this);
        Debug.Log("Enemy");
    }

    protected abstract void Init(); 

    protected void PlayerCatch() {
        if (Vector3.Distance(Player.transform.position, transform.position) < 0.5f) {
            GameManager.Respawn();
        }
    }

    public abstract void Stop();
    public abstract void Resume();
}