using UnityEngine;

public class Button : MonoBehaviour, IMechanism, IInteractableObject
{
    private Task task;

    public bool Activated { get; set; }
    public bool IsStationary { get; } = true;


    public void AddTask(Task t) {
        task = t;
    }

    public void Action() {
        if (Activated == false) {
            Activated = true;
            task.MechanismActivated(Activated);
        }
        
    }

}
