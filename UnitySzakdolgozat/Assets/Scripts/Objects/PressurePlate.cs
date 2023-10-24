using UnityEngine;

public class PressurePlate : MonoBehaviour, IMechanism
{
    private Task task;
    private Vector3 downPosition, upPosition;
    private bool pressureStopped;
    public float speed;

    public bool Activated { get; set; }
    
    public void AddTask(Task t) {
        task = t;
    }

    private void Start() {
        downPosition = upPosition = transform.position;
        downPosition.y = 0f;    
    }

    public void Action() {
        task.MechanismActivated(Activated);
    }

    private void Update() {
        if (pressureStopped && transform.position.y < upPosition.y) {
            transform.Translate(Vector3.up * (speed * Time.deltaTime));
        }
    }

    private void OnCollisionEnter() {
        pressureStopped = false;
        Activated = true;
        Action();
    }

    private void OnCollisionStay(Collision collisionInfo) {
        if (collisionInfo.impulse.y != 0 && transform.position.y > downPosition.y) {
            transform.Translate(Vector3.down * (speed * Time.deltaTime));
        }
    }

    private void OnCollisionExit() {
        pressureStopped = true;
        Activated = false;
        Action();
    }
    
}
