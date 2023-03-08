using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] private int mapSize;
    public List<Room> rooms;
    public Material material;
    public int roomCount;
    private int actualRooms;
    public int range;
    
    private void Start() {
        rooms = new List<Room>();
        actualRooms = 0;
        while (actualRooms < roomCount) {
            Room a = new Room(new Vector2Int(Random.Range(-range, range), Random.Range(-range, range)), 10);
            Generate(a);
        }
        Delaunay.CalculateDelaunay(rooms);
    }
    
    public void Generate(Room room) {
        foreach (var r in rooms) {
            if (room.rect.Overlaps(r.rect)) return;
        }
        rooms.Add(room);
        actualRooms++;
        Mesh mesh = new Mesh();
        List<Vector3> verticies = new List<Vector3>();
        List<int> triangles = new List<int>();

        Vector3 bottomLeft = new Vector3(room.rect.x - .5f, 0, room.rect.y - .5f);
        Vector3 topLeft = new Vector3(room.rect.x - .5f, 0, room.rect.y + room.rect.height + .5f);
        Vector3 bottomRigth = new Vector3(room.rect.x + room.rect.width + .5f, 0, room.rect.y - .5f);
        Vector3 topRight = new Vector3(room.rect.x + room.rect.width + .5f, 0, room.rect.y + room.rect.height + .5f);
        
        Vector3[] v = {bottomLeft, topLeft, topRight, bottomLeft, topRight, bottomRigth};
        for (int i = 0; i < 6; i++) {
            verticies.Add(v[i]);
            triangles.Add(triangles.Count);
        }
        mesh = new Mesh
        {
            vertices = verticies.ToArray(),
            triangles = triangles.ToArray()
        };
        mesh.RecalculateNormals();
        
        
        GameObject a = new GameObject() {
            transform = {position = new Vector3(room.rect.x, 0, room.rect.y)}
        };
        a.AddComponent<MeshFilter>().mesh = mesh;
        a.AddComponent<MeshRenderer>().material = material;
    }
    
    
    
}
