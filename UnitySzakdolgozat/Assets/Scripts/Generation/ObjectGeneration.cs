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
            Debug.LogError("No rooms available to generate lever");
            return null;
        }
        
        room = room ?? availableRooms[Random.Range(0, availableRooms.Count)];
        
        if(remove)
            availableRooms.Remove(room);
        
        return room;
    }
    
    public static Room StartRoom(List<Room> availableRooms) {
        Room room = SelectRoom(availableRooms);
        
        GenerateLeversAndDoors(availableRooms, room, room);

        if (prefabs.TryGetValue("Player", out GameObject playerObj)) {
            Vector2 pos = room.area.center;
            Transform player = Instantiate(playerObj, new Vector3(pos.x, 0, pos.y), Quaternion.identity).transform;
            player.name = "Player";
            GameManager.player = player.GetComponent<PlayerController>();
        }
        else {
            return null;
        }
        
        if (prefabs.TryGetValue("Box", out GameObject box)) {
            Vector2 pos = room.area.center;
            Transform player = Instantiate(box, new Vector3(pos.x + 1, 1, pos.y + 1), Quaternion.identity).transform;
            box.name = "Box";
        }
        else {
            return null;
        }

        return room;
    }

    public static void SpawnEnemyInRoom(List<Room> availableRooms) {
        Room room = SelectRoom(availableRooms);
        
        if (prefabs.TryGetValue("Enemy", out GameObject enemyObj)) {
            Vector2 pos = room.area.center;
            Transform enemy = Instantiate(enemyObj, new Vector3(pos.x, 0, pos.y), Quaternion.identity).transform;
            enemy.name = "Enemy";
            enemy.GetComponent<Enemy>().SetActualRoom(room);
            
        }
    }
    
    public static Room GenerateEndRoom(List<Room> availableRooms) {
        if (availableRooms.Count == 0) {
            Debug.LogError("No rooms available to generate lever");
            return null;
        }

        Room room = DFS.FindNotArticularPoint(availableRooms);
        availableRooms.Remove(room);

        GenerateLeversAndDoors(availableRooms, room);
        
        if (room == null) 
            return null;
        
        if (prefabs.TryGetValue("End", out GameObject endPrefab)) {
            Vector2 pos = room.area.center;
            Instantiate(endPrefab, new Vector3(pos.x, 1, pos.y), Quaternion.identity).tag = "Map";
        } else {
            Debug.LogError("End prefab not found");
        }

        return room; 
        
    }

    public static void GenerateLeversAndDoors(List<Room> availableRooms, Room doorRoom, Room leverRoom = null) {
        GenerateDoors(doorRoom);
        GenerateLevers(availableRooms, doorRoom, leverRoom);
    }
    
    public static Lever GenerateLevers(List<Room> availableRooms, Room opensThisRoom, Room room = null) { 
        room = SelectRoom(availableRooms, false, room);
        
        List<Vector2> wallPositions = room.GetWallPositions();
        Vector2 leverPos = wallPositions[Random.Range(0, wallPositions.Count)];

        Lever lever;
        
        if (prefabs.TryGetValue("Lever", out GameObject leverPrefab)) {
            Quaternion leverRotation = Quaternion.Euler(0,0,0);
            switch (leverPos) {
                case var v when (int) v.x == (int) room.area.xMin:
                    leverRotation = Quaternion.Euler(0, -90, 0);
                    break;
                case var v when (int) v.x == (int) room.area.xMax:
                    leverRotation = Quaternion.Euler(0, 90, 0);
                    break;
                case var v when (int) v.y == (int) room.area.yMin:
                    leverRotation = Quaternion.Euler(0, 180, 0);
                    break;

            }
            
            GameObject leverObj = Instantiate(leverPrefab, new Vector3(leverPos.x, 1, leverPos.y), leverRotation);
            leverObj.tag = "Map";

            lever = leverObj.GetComponent<Lever>();

        }
        else {
            Debug.LogError("Lever prefab not found");
            return null;
        }

        foreach (var door in opensThisRoom.doors) {
            if (door != null) {
                door.tasks.Add(lever);
                lever.doors.Add(door);
            }
        }

        return lever;

    }

    public static List<Door> GenerateDoors(Room room) {
        room.locked = true;
        foreach (var position in room.DoorsLocations()) {
            if (prefabs.TryGetValue("Door", out GameObject doorPrefab)) {
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
                door.tasks = new List<Task>();
                room.doors.Add(door);
                door.lockedRoom = room;

            } else {
                Debug.LogError("Door prefab not found");
                return null;
            }
        }
        
        return room.doors;
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
