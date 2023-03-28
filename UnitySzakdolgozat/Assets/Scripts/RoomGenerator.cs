using System.Collections.Generic;
using System.Linq;
using GenerationUtils;
using GeometryUtils;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public List<Room> rooms;
    public Material material;
    public int roomCount;
    private int actualRooms;
    public int mapSize;
    private GameObject level;
    private bool[,] map;
    
    private void Start() {
        rooms = new List<Room>();
        actualRooms = 0;
        map = new bool[mapSize, mapSize]; 
        
        level = new GameObject();
        level.AddComponent<MeshCollider>();
        level.AddComponent<MeshFilter>();
        level.AddComponent<MeshRenderer>().material = material;
        int attempts = 0;
        
        while (actualRooms < roomCount) {
            Debug.Log(actualRooms);
            if (attempts >= 100) {
                mapSize *= (int)1.25f;
                attempts = 0;
            }
                
            Room room = new Room(mapSize, 20);

            if (!rooms.Any(r => r.Overlaps(room))) {
                rooms.Add(room);
                actualRooms++;
                attempts = 0;
            }
            else
                attempts++;

        }
        GenerateRooms();
        List<Edge> edges = Delaunay.FinalPaths(rooms);
        foreach (var edge in edges) {
            Delaunay.PathFinding(edge, map);
        }
    }

    public void GenerateRooms() {
        Mesh mesh = MeshGeneration.GenerateRooms(rooms, map);
        
        level.GetComponent<MeshFilter>().mesh = mesh;
        level.GetComponent<MeshCollider>().sharedMesh = mesh;
        
    }

    public bool[,] GetMap() {
        return map;
    }
    
    
    
}
