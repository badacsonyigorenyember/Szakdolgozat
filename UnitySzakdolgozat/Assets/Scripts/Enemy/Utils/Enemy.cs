using System;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public Room StartRoom;
    public Transform Player;
    public EnemyType Type;

    protected virtual void Start() {
        Player = GameManager.Player.transform;
        GameManager.AddEnemy(this);
    }

    protected abstract void Init();

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            GameManager.Respawn();
        }
    }

    public abstract void Stop();
    public abstract void Resume();
}