using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;

public class MovingEnemy : Enemy
{
    protected EnemyState State;
    protected NavMeshAgent Agent;
    protected Room ActualRoom;

    protected virtual void Update() {
        if (Agent.isStopped) {
            return;
        }
    }

    protected override void Init() {
        ActualRoom = StartRoom;
        Agent = GetComponent<NavMeshAgent>();
        Agent.Warp(new Vector3(StartRoom.area.center.x, 0, StartRoom.area.center.y));
        State = EnemyState.Idle;
    }

    public override void Stop() {
        Agent.isStopped = true;
    }

    public override void Resume() {
        Agent.isStopped = false;
    }

    public void Respawn() {
        Init();
    }
}
