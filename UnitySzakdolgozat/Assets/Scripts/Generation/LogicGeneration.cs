using System.Collections.Generic;
using UnityEngine;

public class LogicGeneration : MonoBehaviour
{
    private static Dictionary<string, GameObject> prefabs;
    private static List<Room> availableRooms;
    private static Room playerRoom;
    private static Room endRoom;
    private static List<Task> tasks;


    public static bool CreateGameMechanic(List<Room> rooms, int leverCount) {
        ObjectGeneration.LoadPrefabs();
        tasks = new List<Task>();
        availableRooms = rooms;
        
        //tutorial
        if (!Tutorial())
            return false;
        
        // 1 task
        endRoom = ObjectGeneration.GenerateEndRoom(availableRooms);

        for (int i = 0; i < leverCount; i++) {
            ObjectGeneration.GenerateLevers(availableRooms, endRoom);
        }
        
        //ObjectGeneration.GeneratePressurePlate(availableRooms);
        
        SpawnEnemy();

        return true;

    }
    
    private static bool Tutorial() {
        
        playerRoom = ObjectGeneration.StartRoom(availableRooms);

        return true;
    }

    private static void SpawnEnemy() { 
        ObjectGeneration.SpawnEnemyInRoom(availableRooms);
    }

}