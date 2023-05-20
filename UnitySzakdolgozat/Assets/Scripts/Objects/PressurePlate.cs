using System;
using UnityEngine;

public class PressurePlate : MonoBehaviour, Task, InteractableObject
{
    public bool Activated { get; set; }

    public bool IsStationary { get; set; } = true;

    
    public void Action() {
        
    }

    private void OnTriggerEnter(Collider other) {
        Activated = true;
        Debug.Log(name + " activated");
    }

    private void OnTriggerExit(Collider other) {
        Activated = false;
        Debug.Log(name + " deactivated");
    }
}
