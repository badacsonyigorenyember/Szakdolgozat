using System.Collections.Generic;

public class Task
{
    private List<IMechanism> mechanisms;
    private List<Door> doors;

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

    public void MechanismActivated() {
        GameManager gm = GameManager.instance;
        gm.TaskCompleted();
        
        foreach (var mechanism in mechanisms) {
            if (!mechanism.Activated) {
                return;
            }
        }

        foreach (var door in doors) {
            door.Action();
        }

        
    }
    
    
}
