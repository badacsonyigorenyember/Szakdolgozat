using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;

public class MovingEnemy : Enemy
{
    public EnemyState State;
    protected NavMeshAgent Agent;
    protected Room ActualRoom;
    protected GameObject Target;
    protected EnemyState ChasingState;
    
    protected virtual void Update() {
        if (Agent.isStopped) {
            return;
        }

        if (State == ChasingState) {
            gameObject.GetComponent<MeshRenderer>().material = (Material) Resources.Load("Materials/EnemyFollowing");
        }
        else {
            gameObject.GetComponent<MeshRenderer>().material = (Material) Resources.Load("Materials/EnemyBase");
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
    
    protected void Move(Vector3 position) {
        State = EnemyState.Moving;
        Agent.SetDestination(position);
    }
}
