using System;
using UnityEngine;

public class PressurePlate : MonoBehaviour, IMechanism, InteractableObject
{
    private Task task;
    public float movementTime;
    private float timer = 0;
    private Vector3 downPosition, upPosition, currentPosition;

    public bool Activated { get; set; }
    
    public void AddTask(Task task) {
        this.task = task;
    }

    public bool IsStationary { get; set; } = true;


    private void Start() {
        downPosition = upPosition = currentPosition = transform.position;
        downPosition.y = 0f;    
    }

    public void Action() {
        task.MechanismActivated();
    }

    private void Update() {
        if (timer > 0) {
            timer -= Time.deltaTime;
            float distance = Math.Abs(timer - movementTime) / movementTime;
            
            if (Activated) {
                 transform.position = Vector3.Lerp(currentPosition, downPosition, distance);
            }
            else {
                transform.position = Vector3.Lerp(currentPosition, upPosition, distance);
            }
        }
    }


    private void OnTriggerEnter(Collider other) {
        Activated = true;
        timer = movementTime - timer;
        currentPosition = transform.position;
        Action();
    }

    private void OnTriggerExit(Collider other) {
        Activated = false;
        timer = movementTime - timer;
        currentPosition = transform.position;
        Action();
    }
}
