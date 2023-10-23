using UnityEngine;

public class PressurePlate : MonoBehaviour, Mechanism, InteractableObject
{
    public bool Activated { get; set; }

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
