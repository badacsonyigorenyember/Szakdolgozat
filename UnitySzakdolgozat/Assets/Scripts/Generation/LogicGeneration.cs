using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

public static class LogicGeneration
{
    private static Dictionary<string, GameObject> prefabs;
    private static List<Room> availableRooms;
    private static Room playerRoom;
    private static Room endRoom;
    private static Dictionary<string, Task> tasks;


    public static void CreateGameMechanic(List<Room> rooms, int taskCount, int everyNIsPressurePlate, int enemyCount) {
        ObjectGeneration.LoadPrefabs();
        tasks = new Dictionary<string, Task>();
        availableRooms = rooms;
        
        //tutorial
        Tutorial();
        
        //endroom
        EndRoom();

        for (int i = 0; i < taskCount; i++) {
            IMechanism mechanism;
            if ((i + 1) % everyNIsPressurePlate == 0) {
                mechanism =
                    ObjectGeneration.GeneratePressurePlate(FindRandomRoom(false), FindRandomRoom(false));
            }
            else {
                mechanism = ObjectGeneration.GenerateButton(FindRandomRoom(false));
            }
            
            CreateTask("End", mechanism);
        }

        for (int i = 0; i < enemyCount; i++) {
            SpawnEnemy(i + 1);
        }

    }

    private static void Tutorial() {
        Room room = FindRandomRoom(true);
        if (room == null) return;

        ObjectGeneration.StartRoom(room);
        
        Task task = new Task(room);
        Button button = ObjectGeneration.GenerateButton(room);
        task.AddMechanism(button);
        PressurePlate plate = ObjectGeneration.GeneratePressurePlate(room, room);
        task.AddMechanism(plate);
        tasks["Start"] = task;
        
        ObjectGeneration.SpawnEnemyInRoom(room, EnemyType.Spike);

    }

    private static void EndRoom() {
        Room room = FindNotArticularRoom(true);
        if (room == null) return;
        
        ObjectGeneration.GenerateEndRoom(room);
        
        Task task = new Task(room);
        tasks["End"] = task;
    }

    private static void SpawnEnemy(int numberOfEnemies) {
        Room room = FindRandomRoom(false);
        
        
        if (numberOfEnemies % GameManager.EveryNIsSpike == 0) {
            ObjectGeneration.SpawnEnemyInRoom(room, EnemyType.Spike);
        }

        if (numberOfEnemies % GameManager.EveryNIsFollowerEnemy == 0) {
            //TODO: ObjectGenerationSpawnEnemyInRoom(room, EnemyType.Follower);
        }
        else {
            ObjectGeneration.SpawnEnemyInRoom(room, EnemyType.Roamer);
        }
        
    }

    private static Room FindNotArticularRoom(bool toDelete) {
        if (availableRooms.Count == 0) {
            Debug.LogError("No rooms available to get a room");
            return null;
        }

        Room room = DepthFirstSearch.FindNotArticularPoint(availableRooms);
        
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

    private static void CreateTask(string name, IMechanism mechanism) {
        Task task = tasks[name];
        task.AddMechanism(mechanism);
        mechanism.AddTask(task);
        
    }
}