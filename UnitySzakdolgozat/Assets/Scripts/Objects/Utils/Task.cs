using System.Collections.Generic;
using System.Linq;

public class Task
{
    private readonly List<IMechanism> mechanisms;
    private readonly List<Door> doors;
    private bool finishedTask;

    public Task(Room doorRoom) {
        doors = doorRoom.doors;
        mechanisms = new List<IMechanism>();
    }

    public void AddMechanism(IMechanism mechanism) {
        mechanisms.Add(mechanism);
        mechanism.AddTask(this);
    }

    public void MechanismActivated(bool activated) {
        var gm = GameManager.instance;
        gm.TaskCompleted(activated);

        if (mechanisms.Any(m => !m.Activated)) {
            if (finishedTask) {
                ActivateDoors(false);
            }
        }
        else {
            ActivateDoors(true);
            finishedTask = true;
        }

    }

    private void ActivateDoors(bool open) {
        foreach (var door in doors) {
            door.Action(open);
        }
    }
    
    
}
