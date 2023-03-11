using System.Collections.Generic;
using System.Linq;
using GeometryUtils;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public List<Room> rooms;
    public Material material;
    public int roomCount;
    private int actualRooms;
    public int mapSize;
    private GameObject map;
    
    private void Start() {
        rooms = new List<Room>();
        actualRooms = 0;
        map = new GameObject();
        map.AddComponent<MeshCollider>();
        map.AddComponent<MeshFilter>();
        map.AddComponent<MeshRenderer>().material = material;
        
        while (actualRooms < roomCount) {
            Room room = new Room(new Vector2Int(Random.Range(-mapSize, mapSize), Random.Range(-mapSize, mapSize)), 20);
            
            bool overlaps = rooms.Any(r => r.rect.Overlaps(room.rect));
            
            if (!overlaps) {
                rooms.Add(room);
                actualRooms++;
            }
            
        }
        Generate();
        
        Delaunay.FinalPaths(rooms);
        
    }

    public void Generate() {
        Mesh mesh = new Mesh();
        
        List<Vector3> verticies = new List<Vector3>();
        List<int> triangles = new List<int>();
        
        foreach (var room in rooms) {
            Vector3 bottomLeft = new Vector3(room.rect.position.x - .5f, 0, room.rect.position.y - .5f);
            Vector3 topLeft = new Vector3(room.rect.position.x - .5f, 0, room.rect.position.y + room.rect.height + .5f);
            Vector3 bottomRigth = new Vector3(room.rect.position.x + room.rect.width + .5f, 0, room.rect.position.y - .5f);
            Vector3 topRight = new Vector3(room.rect.position.x + room.rect.width + .5f, 0, room.rect.position.y + room.rect.height + .5f);

            Vector3[] v = {bottomLeft, topLeft, topRight, bottomLeft, topRight, bottomRigth};
            for (int i = 0; i < 6; i++) {
                verticies.Add(v[i]);
                triangles.Add(triangles.Count);
            }
        }

        

        mesh.vertices = verticies.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        map.GetComponent<MeshFilter>().mesh = mesh;
        map.GetComponent<MeshCollider>().sharedMesh = mesh;
        
    }
    
    
    
}
