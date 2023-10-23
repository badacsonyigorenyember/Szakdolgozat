using System.Collections.Generic;
using UnityEngine;

public class ObjectGeneration : MonoBehaviour
{
    private static Dictionary<string, GameObject> prefabs;

    public static bool LoadPrefabs() {
        prefabs = new Dictionary<string, GameObject>();

        GameObject[] loads = Resources.LoadAll<GameObject>("Prefabs");

        foreach (var prefab in loads) {
            prefabs.Add(prefab.name, prefab);
        }

        if (prefabs.Count == 0)
            return false;

        return true;
    }

    private static Room SelectRoom(List<Room> availableRooms, bool remove = true, Room room = null) {
        if (availableRooms.Count == 0) {
            Debug.LogError("No rooms available to generate button");
            return null;
        }

        room = room ?? availableRooms[Random.Range(0, availableRooms.Count)];

        if(remove)
            availableRooms.Remove(room);
        
        return room;
    }
    
    public static void StartRoom(Room room) {
        if (prefabs.TryGetValue("Player", out GameObject playerObj)) {
            Vector2 pos = room.area.center;
            Transform player = Instantiate(playerObj, new Vector3(pos.x, 0, pos.y), Quaternion.identity).transform;
            player.name = "Player";
            GameManager.player = player.GetComponent<PlayerController>();
        }
        else {
            return;
        }
        
        if (prefabs.TryGetValue("Box", out GameObject boxObj)) {
            Vector2 pos = room.area.center;
            Transform box = Instantiate(boxObj, new Vector3(pos.x + 1, 1, pos.y + 1), Quaternion.identity).transform;
            box.name = "Box";
        }
        
        GenerateDoors(room);
    }
    
    public static void GenerateEndRoom(Room room) {
        if (prefabs.TryGetValue("End", out GameObject endPrefab)) {
            Vector2 pos = room.area.center;
            Instantiate(endPrefab, new Vector3(pos.x, 1, pos.y), Quaternion.identity).tag = "Map";
        } else {
            Debug.LogError("End prefab not found");
        }

        GenerateDoors(room);
    }

    public static void SpawnEnemyInRoom(Room room) {
        if (prefabs.TryGetValue("Enemy", out GameObject enemyObj)) {
            Vector2 pos = room.area.center;
            Transform enemy = Instantiate(enemyObj, new Vector3(pos.x, 0, pos.y), Quaternion.identity).transform;
            enemy.name = "Enemy";
            enemy.GetComponent<Enemy>().starterRoom = room;

        }
    }
    
   
    
    
    
    public static Button GenerateButton(Room room) {
        Vector2 buttonPos = room.GetRandomWallPosition();

        Button button;
        
        if (prefabs.TryGetValue("Button", out GameObject buttonPrefab)) {
            Quaternion buttonRotation = Quaternion.Euler(0,0,0);
            switch (buttonPos) {
                case var v when (int) v.x == (int) room.area.xMin:
                    buttonRotation = Quaternion.Euler(0, -90, 0);
                    break;
                case var v when (int) v.x == (int) room.area.xMax:
                    buttonRotation = Quaternion.Euler(0, 90, 0);
                    break;
                case var v when (int) v.y == (int) room.area.yMin:
                    buttonRotation = Quaternion.Euler(0, 180, 0);
                    break;

            }
            
            GameObject buttonObj = Instantiate(buttonPrefab, new Vector3(buttonPos.x, 1, buttonPos.y), buttonRotation);
            buttonObj.tag = "Map";

            button = buttonObj.GetComponent<Button>();
        }
        else {
            Debug.LogError("button prefab not found");
            return null;
        }
        
        return button;

    }

    public static void GenerateDoors(Room room) {
        room.locked = true;
        
        if (prefabs.TryGetValue("Door", out GameObject doorPrefab)) {
            foreach (var position in room.entrancePositions) {
                Quaternion doorRotation = Quaternion.Euler(0,0,0);
                
                switch (position) {
                    case var v when (int) v.x == (int) room.area.xMin:
                        doorRotation = Quaternion.Euler(0, -90, 0);
                        break;
                    case var v when (int) v.x == (int) room.area.xMax:
                        doorRotation = Quaternion.Euler(0, 90, 0);
                        break;
                    case var v when (int) v.y == (int) room.area.yMin:
                        doorRotation = Quaternion.Euler(0, 180, 0);
                        break;

                }
                
                GameObject doorObj = Instantiate(doorPrefab, new Vector3(position.x, 0, position.y), doorRotation);
                doorObj.name = "Door";

                Door door = doorObj.GetComponent<Door>();
                room.doors.Add(door);

            }
            
        } else {
            Debug.LogError("Door prefab not found");
        }
    }

    public static void GeneratePressurePlate(List<Room> availableRooms) {
        Room room = SelectRoom(availableRooms, false);

        Vector2 pos = room.GetRandPositionInRoom();

        if (prefabs.TryGetValue("Pressure Plate", out GameObject platePrefab)) {
            GameObject plate = Instantiate(platePrefab, new Vector3(pos.x, 0.1f, pos.y), Quaternion.identity);
            plate.name = "Pressure Plate";
            
        } else {
            Debug.LogError("Pressure plate prefab not found");
        }
    }
}
