using UnityEngine;

public class StationaryEnemy : Enemy
{
    protected Vector3 BasePosition;
    protected bool IsMoving;

    protected override void Init() { }

    public override void Stop() {
        IsMoving = false;
    }

    public override void Resume() {
        IsMoving = true;
    }

    public void SetBasePosition(Vector3 position) {
        BasePosition = position;
    }
    
}
