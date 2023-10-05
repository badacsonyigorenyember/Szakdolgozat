using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour, Task, InteractableObject
{
    public List<Door> doors;

    public bool Activated { get; set; }

    public bool IsStationary { get; set; } = true;
    
    public void Action() {
        if (!Activated) {
            Activated = true;
            doors.ForEach(d => d.Action());
            GameManager gm = GameManager.instance;
            gm.TaskCompleted();
        }
        
    }
    

    
}
