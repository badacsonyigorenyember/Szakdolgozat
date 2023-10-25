using UnityEngine;
public class SpikeEnemy : StationaryEnemy
{    
    [SerializeField]
    private float MovingRange;
    [SerializeField]
    private float MoveTime;
    [SerializeField]
    private float WaitTime;

     

    private Vector3 EndPosition;
    private float Timer;
     
    private SpikeState State;
    private SpikeState PreviousState;

     
    protected override void Start() {
        base.Start();
        EndPosition = BasePosition + Vector3.up * MovingRange;
        IsMoving = true;
    }

    private void Update() {
        if (IsMoving) {
            switch (State) {
                case SpikeState.GoingUp: {
                    if (Timer < MoveTime / 10) {
                        Timer += Time.deltaTime;
                    
                        transform.position = Vector3.LerpUnclamped(BasePosition, EndPosition, Timer / (MoveTime / 10));
                    }
                    else {
                        Timer = 0;
                        State = SpikeState.Waiting;
                        PreviousState = SpikeState.GoingUp;
                    }

                    break;
                }

                case SpikeState.Waiting: {
                    if (Timer < WaitTime) {
                        Timer += Time.deltaTime;
                    }
                    else {
                        Timer = 0;
                        State = PreviousState == SpikeState.GoingUp ? SpikeState.GoingDown : SpikeState.GoingUp;
                    }
                    
                    break;
                }

                case SpikeState.GoingDown: {
                    if (Timer < MoveTime) {
                        Timer += Time.deltaTime;
                    
                        transform.position = Vector3.LerpUnclamped(EndPosition, BasePosition, Timer / MoveTime);
                    }
                    else {
                        Timer = 0;
                        State = SpikeState.Waiting;
                        PreviousState = SpikeState.GoingDown;
                    }
                    
                    break;
                }
            }
        }
    }
     
    private enum SpikeState
    {
        GoingUp,
        GoingDown,
        Waiting
    }
     
}
 
 
