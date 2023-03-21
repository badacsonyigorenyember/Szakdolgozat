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
        
        while (actualRooms < roomCount) {
            Room room = new Room(new Vector2Int(Random.Range(-mapSize, mapSize), Random.Range(-mapSize, mapSize)), 20);
            
            bool overlaps = rooms.Any(r => r.rect.Overlaps(room.rect));
            
            if (!overlaps) {
                rooms.Add(room);
                actualRooms++;
            }
            
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
