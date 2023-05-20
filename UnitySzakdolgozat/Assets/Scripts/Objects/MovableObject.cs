using UnityEngine;

public class MovableObject : MonoBehaviour, InteractableObject
{
    public bool IsStationary { get; set; } = false;
    
    public void Action() {
        Debug.Log("asd");
    }
    
}
