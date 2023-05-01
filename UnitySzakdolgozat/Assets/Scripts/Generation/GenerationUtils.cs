using System.Collections.Generic;
using UnityEngine;

public class GenerationUtils : MonoBehaviour
{
    private static Dictionary<string, GameObject> prefabs;
    private static List<Room> availableRooms;
    private static Room playerRoom;
    private Room endRoom;


    public static bool CreateGameMechanic(List<Room> rooms) {
        LoadPrefabs();
        availableRooms = rooms;

        if (!PlacePlayer())
            return false;
        
        //tutorial
        SetDoorsToLever(GenerateLever(playerRoom), GenerateDoors(playerRoom));
        
        // 1 task
        if(!GenerateEndRoom())
            return false;

        return true;

    }

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
    
    static bool PlacePlayer() {
        if (availableRooms.Count == 0) {
            Debug.LogError("No rooms available to generate lever");
            return false;
        }
        
        Room room = availableRooms[Random.Range(0, availableRooms.Count)];
        availableRooms.Remove(room);

        if (prefabs.TryGetValue("Player", out GameObject playerObj)) {
            GameManager.player = Instantiate(playerObj, new Vector3(room.center.x, 0, room.center.y), Quaternion.identity).transform;
            GameManager.player.name = "Player";
        }
        else {
            return false;
        }

            playerRoom = room;
        return true;
    }
    
    static GameObject GenerateLever(Room room = null) {
        if (availableRooms.Count == 0) {
            Debug.LogError("No rooms available to generate lever");
            return null;
        }

        room = room ?? availableRooms[Random.Range(0, availableRooms.Count)];
        //availableRooms.Remove(room);
        
        List<Vector2> wallPositions = room.GetWallPositions();
        Vector2 leverPos = wallPositions[Random.Range(0, wallPositions.Count)];
        
        GameObject lever = null;

        if (prefabs.TryGetValue("Lever", out GameObject leverPrefab)) {
            Vector2 directionToCenter = room.center - leverPos;
            Quaternion leverRotation;
            if(Mathf.Abs(directionToCenter.x) > Mathf.Abs(directionToCenter.y)) 
                leverRotation = Quaternion.Euler(0,directionToCenter.x > 0 ? -90 : 90,0);
            else 
                leverRotation = Quaternion.Euler(0,directionToCenter.y > 0 ? 180 : 0,0);
            
            lever = Instantiate(leverPrefab, new Vector3(leverPos.x, 1, leverPos.y), leverRotation);
            lever.tag = "Map";
        }
        else 
            Debug.LogError("Lever prefab not found");
        
        return lever;
    }

    static List<GameObject> GenerateDoors(Room room) {
        room.locked = true;
        List<GameObject> doors = new List<GameObject>();
        foreach (var position in room.DoorsLocations()) {
            if (prefabs.TryGetValue("Door", out GameObject leverPrefab)) {
                Vector2 directionToCenter = room.center - position;
                Quaternion doorRotation;
                if(Mathf.Abs(directionToCenter.x) > Mathf.Abs(directionToCenter.y)) 
                    doorRotation = Quaternion.Euler(0,directionToCenter.x > 0 ? -90 : 90,0);
                else 
                    doorRotation = Quaternion.Euler(0,directionToCenter.y > 0 ? 180 : 0,0);
                
                doors.Add(Instantiate(leverPrefab, new Vector3(position.x, 0, position.y), doorRotation));
            } else {
                Debug.LogError("Door prefab not found");
                return null;
            }
        }
        
        doors.ForEach(d => d.tag = "Map");

        return doors;
    }

    static void SetDoorsToLever(GameObject lever, List<GameObject> doors) {
        lever.TryGetComponent(out Lever component);
        component.doors = doors;

    }
    
    static bool GenerateEndRoom() {
        if (availableRooms.Count == 0) {
            Debug.LogError("No rooms available to generate lever");
            return false;
        }

        Room room = DFS.FindNotArticularPoint(availableRooms);
        if (room == null) 
            return false;
        
        availableRooms.Remove(room);
        
        SetDoorsToLever(GenerateLever(), GenerateDoors(room));

        if (prefabs.TryGetValue("End", out GameObject leverPrefab)) {
            Instantiate(leverPrefab, new Vector3(room.center.x, 1, room.center.y), Quaternion.identity).tag = "Map";
        } else {
            Debug.LogError("End prefab not found");
        }

        return true; 
        
        //endRoom = room;
    }
}