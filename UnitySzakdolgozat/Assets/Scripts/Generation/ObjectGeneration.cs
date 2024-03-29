using System.Collections.Generic;
using UnityEngine;

public class ObjectGeneration : MonoBehaviour
{
    private static Dictionary<string, GameObject> prefabs;

    public static void LoadPrefabs() {
        prefabs = new Dictionary<string, GameObject>();

        GameObject[] loads = Resources.LoadAll<GameObject>("Prefabs");

        foreach (var prefab in loads) {
            prefabs.Add(prefab.name, prefab);
        }
    }

    public static void StartRoom(Room room) {
        if (prefabs.TryGetValue("Player", out GameObject playerObj)) {
            Vector2 pos = room.area.center;
            Transform player = Instantiate(playerObj, new Vector3(pos.x, 0, pos.y), Quaternion.identity).transform;
            player.name = "Player";
            player.GetComponent<PlayerController>().starterRoom = room;
            
            GameManager.SetPlayer(player.GetComponent<PlayerController>());
            
        }
        else {
            return;
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

    public static void SpawnEnemyInRoom(Room room, EnemyType type) {
        GameObject enemyPrefab;
        
        switch (type) {
            case EnemyType.Roamer:
                if (prefabs.TryGetValue("RoamerEnemy", out enemyPrefab)) {
                    Vector2 pos = room.area.center;
                    
                    GameObject enemyObj = Instantiate(enemyPrefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
                    
                    enemyObj.name = "RoamerEnemy";
                    enemyObj.GetComponent<Enemy>().StartRoom = room;
                }

                break;
            
            case EnemyType.Spike:
                if (prefabs.TryGetValue("Spike", out enemyPrefab)) {
                    foreach (var entrance in room.entrancePositions) {
                        Vector3 pos = new Vector3(entrance.x, -0.5f, entrance.y);
                        GameObject enemyObj = Instantiate(enemyPrefab, pos, Quaternion.identity);
                        
                        enemyObj.name = "Spike";
                        SpikeEnemy enemy = enemyObj.GetComponent<SpikeEnemy>();
                        
                        enemy.StartRoom = room;
                        enemy.SetBasePosition(pos);
                    }
                    
                }
                
                break;
            
            case EnemyType.Follower:
                if (prefabs.TryGetValue("FollowerEnemy", out enemyPrefab)) {
                    Vector2 pos = room.area.center;
                    
                    GameObject enemyObj = Instantiate(enemyPrefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
                    
                    enemyObj.name = "FollowerEnemy";
                    enemyObj.GetComponent<Enemy>().StartRoom = room;
                }
                break;
        }
        
    }

    public static Button GenerateButton(Room room) {
        Vector2 buttonPos = room.GetRandomWallPosition();

        Button button;
        
        if (prefabs.TryGetValue("Button", out GameObject buttonPrefab)) {
            Quaternion buttonRotation = Quaternion.Euler(0,0,0);
            switch (buttonPos) {
                case var v when (int) v.x == room.area.xMin:
                    buttonRotation = Quaternion.Euler(0, -90, 0);
                    break;
                case var v when (int) v.x == room.area.xMax:
                    buttonRotation = Quaternion.Euler(0, 90, 0);
                    break;
                case var v when (int) v.y == room.area.yMin:
                    buttonRotation = Quaternion.Euler(0, 180, 0);
                    break;

            }
            
            GameObject buttonObj = Instantiate(buttonPrefab, new Vector3(buttonPos.x, 0.75f, buttonPos.y), buttonRotation);
            buttonObj.tag = "Map";

            button = buttonObj.GetComponent<Button>();
        }
        else {
            Debug.LogError("button prefab not found");
            return null;
        }
        
        return button;

    }

    private static void GenerateDoors(Room room) {
        room.locked = true;
        
        if (prefabs.TryGetValue("Door", out GameObject doorPrefab)) {
            foreach (var position in room.entrancePositions) {
                Quaternion doorRotation = Quaternion.Euler(0,0,0);
                
                switch (position) {
                    case var v when (int) v.x == room.area.xMin:
                        doorRotation = Quaternion.Euler(0, -90, 0);
                        break;
                    case var v when (int) v.x == room.area.xMax:
                        doorRotation = Quaternion.Euler(0, 90, 0);
                        break;
                    case var v when (int) v.y == room.area.yMin:
                        doorRotation = Quaternion.Euler(0, 180, 0);
                        break;

                }
                
                GameObject doorObj = Instantiate(doorPrefab, new Vector3(position.x, 0, position.y), doorRotation);
                doorObj.name = "Door";

                Door door = doorObj.GetComponent<Door>();
                room.doors.Add(door);
                door.SetRoom(room);
                
                MapGeneration.BuildNavMesh();

            }
            
        } else {
            Debug.LogError("Door prefab not found");
        }
    }

    public static PressurePlate GeneratePressurePlate(Room plateRoom, Room boxRoom) {
        Vector2 platePos = plateRoom.GetRandPositionInRoom(true);

        PressurePlate plate;

        if (prefabs.TryGetValue("Pressure Plate", out GameObject platePrefab)) {
            GameObject plateObj = Instantiate(platePrefab, new Vector3(platePos.x, 0.05f, platePos.y), Quaternion.identity);
            plateObj.name = "Pressure Plate";

            plate = plateObj.GetComponent<PressurePlate>();
            
        } else {
            Debug.LogError("Pressure plate prefab not found");
            return null;
        }
        
        if (prefabs.TryGetValue("Box", out GameObject boxPrefab)) {
            Vector2 boxPos = boxRoom.area.center;
            GameObject boxObj = Instantiate(boxPrefab, new Vector3(boxPos.x + 1, 1, boxPos.y + 1), Quaternion.identity);
            boxObj.name = "Box";
            boxObj.tag = "Map";
        }
        else {
            Debug.LogError("Box prefab not found");
            return null;
        }

        return plate;
    }
    
}
