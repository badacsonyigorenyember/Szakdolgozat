using System.Collections.Generic;
using UnityEngine;

public class LogicGeneration : MonoBehaviour
{
    private static Dictionary<string, GameObject> prefabs;
    private static List<Room> availableRooms;
    private static Room playerRoom;
    private static Room endRoom;
    public static Dictionary<string, Task> tasks;


    public static bool CreateGameMechanic(List<Room> rooms, int taskCount) {
        ObjectGeneration.LoadPrefabs();
        tasks = new Dictionary<string, Task>();
        availableRooms = rooms;
        
        //tutorial
        Tutorial();
        
        //endroom
        EndRoom();

        for (int i = 0; i < taskCount; i++) {
            Button button = ObjectGeneration.GenerateButton(FindRandomRoom(false));
            CreateTask("End", button);
        }
        
        
        
        SpawnEnemy();
        

        return true;

    }

    private static void Tutorial() {
        Room room = FindRandomRoom(true);
        if (room == null) return;

        ObjectGeneration.StartRoom(room);

        GameManager.SetStarterRoom(room);

        Task task = new Task(room);
        Button button = ObjectGeneration.GenerateButton(room);
        task.AddMechanism(button);
        PressurePlate plate = ObjectGeneration.GeneratePressurePlate(room);
        task.AddMechanism(plate);
        tasks["Start"] = task;
        
        
    }

    private static void EndRoom() {
        Room room = FindNotArticularRoom(true);
        if (room == null) return;
        
        ObjectGeneration.GenerateEndRoom(room);
        
        Task task = new Task(room);
        tasks["End"] = task;
    }

    private static void SpawnEnemy() {
        Room room = FindRandomRoom(false);
        ObjectGeneration.SpawnEnemyInRoom(room);
    }

    private static Room FindNotArticularRoom(bool toDelete) {
        if (availableRooms.Count == 0) {
            Debug.LogError("No rooms available to get a room");
            return null;
        }

        Room room = DFS.FindNotArticularPoint(availableRooms);
        
        if (toDelete) availableRooms.Remove(room);
        
        return room;
    }

    private static Room FindRandomRoom(bool toDelete) {
        if (availableRooms.Count == 0) return null;

        int pos = Random.Range(0, availableRooms.Count);
        Room room = availableRooms[pos];
        
        if(toDelete) availableRooms.RemoveAt(pos);
        
        return room;
    }

    public static void CreateTask(string name, IMechanism mechanism) {
        Task task = tasks[name];
        task.AddMechanism(mechanism);
        mechanism.AddTask(task);
        
    }
}