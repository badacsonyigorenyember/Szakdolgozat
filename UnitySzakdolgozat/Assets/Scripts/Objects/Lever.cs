using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour, Task, InteractableObject
{
    public List<Door> doors;

    public bool Activated { get; set; }

    public bool IsStationary { get; set; } = true;
    
    public void Action() {
        Debug.LogError(doors[0].tasks.Count);
        Activated = true;
        Debug.Log(name + " activated");
        doors.ForEach(d => d.Action());
    }

    
}
