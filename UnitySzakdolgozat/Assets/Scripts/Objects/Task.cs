using System.Collections.Generic;
using UnityEngine;

public class Task
{
    private List<IMechanism> mechanisms;
    private List<Door> doors;
    private bool finishedTask; 

    public Task() {
        mechanisms = new List<IMechanism>();
        doors = new List<Door>();
    }

    public Task(Room doorRoom) {
        doors = doorRoom.doors;
        mechanisms = new List<IMechanism>();
    }

    public void AddMechanism(IMechanism mechanism) {
        mechanisms.Add(mechanism);
        mechanism.AddTask(this);
    }

    public void MechanismActivated(bool activated) {
        GameManager gm = GameManager.instance;
        gm.TaskCompleted(activated);
        
        foreach (var mechanism in mechanisms) {
            if (!mechanism.Activated) {
                if (finishedTask) {
                    ActivateDoors(false);
                }
                return;
            }
        }

        ActivateDoors(true);
        finishedTask = true;

    }

    private void ActivateDoors(bool open) {
        foreach (var door in doors) {
            door.Action(open);
        }
    }
    
    
}
