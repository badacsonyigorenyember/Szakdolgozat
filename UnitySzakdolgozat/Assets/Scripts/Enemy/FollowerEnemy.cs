public class FollowerEnemy : MovingEnemy
{
    protected override void Start() {
        base.Start();
        Init();
        State = EnemyState.Chasing;
        ChasingState = EnemyState.Moving;
    }

    protected override void Update() {
        base.Update();
        
        Move(Player.position);
    }
}

