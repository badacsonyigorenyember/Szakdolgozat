using UnityEngine;

public class Button : MonoBehaviour, IMechanism, InteractableObject
{
    public Task task;

    public bool Activated { get; set; }

    public void AddTask(Task task) {
        this.task = task;
    }

    public bool IsStationary { get; set; } = true;
    
    
    public void Action() {
        if (!Activated) {
            Activated = true;
            task.MechanismActivated(Activated);
        }
        
    }



}
