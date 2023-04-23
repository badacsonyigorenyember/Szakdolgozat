using System.Collections.Generic;
using System.Linq;
using GenerationUtils;
using GeometryUtils;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public List<Room> rooms;
    public int roomCount;
    private int actualRooms;
    public int mapSize;
    public int maxRoomSize;
    
    public static bool[,] map;

    public Transform player;
    
    public static Vector3 playerPos;

    public GameObject lever;


    private void Start() {
        rooms = new List<Room>();
        actualRooms = 0;
        float startTime = Time.realtimeSinceStartup;
        
        GenerateRooms();
        
        List<Edge> edges = Delaunay.FinalEdges(rooms);
        Delaunay.PathFinding(edges, rooms, map);
        MeshGeneration.GenerateWalls(map, 3);

        PlacePlayer();

        GenerateLever();
        
        float endTime = Time.realtimeSinceStartup;
        Debug.Log(endTime - startTime + " seconds");

}

    public void GenerateRooms() {
        int attempts = 0;
        while (actualRooms < roomCount) {
            if (attempts == 5000) {
                mapSize += mapSize / 2;
                attempts = 0;
            }
            
            Room room = new Room(mapSize, maxRoomSize);

            if (!rooms.Any(r => r.Overlaps(room))) {
                rooms.Add(room);
                actualRooms++;
                attempts = 0;
            }
            else
                attempts++;

        }
        
        map = new bool[(mapSize + maxRoomSize) * 2 + 1, (mapSize + maxRoomSize) * 2 + 1];

        MeshGeneration.GenerateRooms(rooms, gameObject);
        
        gameObject.transform.position += new Vector3(mapSize, 0, mapSize);

        foreach (var room in rooms) {
            room.Position += new Vector2(mapSize, mapSize);
            room.ClaimArea(map);
        }
        
    }

    private void PlacePlayer() {
        Vector2 position = rooms[Random.Range(0, roomCount - 1)].center;
        player.position = new Vector3(position.x, 0.5f, position.y);
        Debug.Log(position + " " + player.position);
    }

    void GenerateLever() {
        Room room = rooms[Random.Range(0, rooms.Count)];
        
    }
}
