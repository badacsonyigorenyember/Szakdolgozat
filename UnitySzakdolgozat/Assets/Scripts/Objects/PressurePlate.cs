using UnityEngine;

public class PressurePlate : MonoBehaviour, IMechanism, InteractableObject
{
    private Task task;
    
    public bool Activated { get; set; }
    public void AddTask(Task task) {
        this.task = task;
    }

    public bool IsStationary { get; set; } = true;

    
    public void Action() {
        
    }

    private void OnTriggerEnter(Collider other) {
        Activated = true;
    }

    private void OnTriggerExit(Collider other) {
        Activated = false;
    }
}
