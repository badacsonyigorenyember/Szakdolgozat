using System.Collections.Generic;
using GeometryUtils;
using UnityEngine;
using UnityEngine.AI;
using Utils;

public class MapGeneration : MonoBehaviour
{
    public static void GenerateMap(int roomCount, int mapSize, int maxRoomSize) {
        GameManager.rooms = RoomGeneration.CreateRooms(roomCount, mapSize, maxRoomSize);
        List<Room> rooms = GameManager.rooms;
        GameManager.availableRooms = rooms;
        GameManager.map = new Map((mapSize + maxRoomSize) * 2 + 1);
        rooms.ForEach(r => r.ClaimArea(GameManager.map));
        
        List<Edge> edges = Delaunay.FinalEdges(rooms);

        Delaunay.PathFinding(edges, rooms, GameManager.map);
        GenerateFloor(GameManager.rooms, mapSize);
        GenerateWalls(3, GameManager.map);

        BakeNavMesh();
    }
    
    public static void GenerateFloor(List<Room> rooms, int mapSize) {
        List<Vector3> verticies = new List<Vector3>();
        List<int> triangles = new List<int>();
            
        foreach (var room in rooms) {
            Vector2 pos = room.area.position;
            Vector3 bottomLeft = new Vector3(pos.x - .5f, 0, pos.y - .5f);
            Vector3 topLeft = new Vector3(pos.x - .5f, 0, pos.y + room.area.height + .5f);
            Vector3 bottomRigth = new Vector3(pos.x + room.area.width + .5f, 0, pos.y - .5f);
            Vector3 topRight = new Vector3(pos.x + room.area.width + .5f, 0, pos.y + room.area.height + .5f);
            
            Vector3[] v = {bottomLeft, topLeft, topRight, bottomLeft, topRight, bottomRigth};

            for (int i = 0; i < 6; i++) {
                verticies.Add(v[i]);
                triangles.Add(triangles.Count);
            }
            
            //ceiling
            Vector3 ceilingBottomLeft = new Vector3(pos.x - .5f, 3, pos.y - .5f);
            Vector3 ceilingTopLeft = new Vector3(pos.x - .5f, 3, pos.y + room.area.height + .5f);
            Vector3 ceilingBottomRigth = new Vector3(pos.x + room.area.width + .5f, 3, pos.y - .5f);
            Vector3 ceilingTopRight = new Vector3(pos.x + room.area.width + .5f, 3, pos.y + room.area.height + .5f);

            v = new[] {ceilingBottomLeft, ceilingTopRight, ceilingTopLeft, ceilingBottomLeft, ceilingBottomRigth, ceilingTopRight};

            for (int i = 0; i < 6; i++) {
                verticies.Add(v[i]);
                triangles.Add(triangles.Count);
            }
            
        }

        Mesh mesh = new Mesh()
        {
            vertices = verticies.ToArray(),
            triangles = triangles.ToArray()
        };
        GameObject corridor = GenerateCorridors(GameManager.map);
        

        Mesh floor = new Mesh();

        

        GameObject o = new GameObject()
        {
            //transform = { position = new Vector3(mapSize, 0, mapSize) },
            name = "Floor",
            tag = "Map"
        };

        o.AddComponent<MeshFilter>().mesh = mesh;
        o.layer = LayerMask.NameToLayer("Ground");
        
        CombineInstance[] combineInstances = new CombineInstance[2];
        combineInstances[0].mesh = o.GetComponent<MeshFilter>().sharedMesh;
        combineInstances[0].transform = o.transform.localToWorldMatrix;
        combineInstances[1].mesh = corridor.GetComponent<MeshFilter>().sharedMesh;
        combineInstances[1].transform = corridor.transform.localToWorldMatrix;
        
        floor.CombineMeshes(combineInstances, true, false);
        
        floor.RecalculateNormals();
        floor.RecalculateTangents();

        o.GetComponent<MeshFilter>().mesh = floor;
        o.AddComponent<MeshRenderer>().material = (Material) Resources.Load("FloorMaterial", typeof(Material));
        o.AddComponent<MeshCollider>().sharedMesh = floor;
        
        o.AddComponent<NavMeshSurface>();
        o.GetComponent<NavMeshSurface>().layerMask = LayerMask.GetMask("Ground");
        o.GetComponent<NavMeshSurface>().BuildNavMesh();

        Destroy(corridor);
        
    }
    
    public static GameObject GenerateCorridors(Map map) {
        List<Vector3> verticies = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int x = 0; x < map.Size; x++) {
            for (int y = 0; y < map.Size; y++) {
                if (map[x, y] == FieldType.Corridor) {
                    GameManager.map[x, y] = FieldType.Corridor;
                    Vector3 bottomLeft = new Vector3(x - .5f, 0, y - .5f);
                    Vector3 topLeft = new Vector3(x - .5f, 0, y + .5f);
                    Vector3 bottomRigth = new Vector3(x + .5f, 0, y - .5f);
                    Vector3 topRight = new Vector3(x + .5f, 0, y + .5f);

                    Vector3[] v = {bottomLeft, topLeft, topRight, bottomLeft, topRight, bottomRigth};
                    
                    for (int j = 0; j < 6; j++) {
                        verticies.Add(v[j]);
                        triangles.Add(triangles.Count);
                    }
                    
                    Vector3 ceilingBottomLeft = new Vector3(x - .5f, 3, y - .5f);
                    Vector3 ceilingTopLeft = new Vector3(x - .5f, 3, y + .5f);
                    Vector3 ceilingBottomRigth = new Vector3(x + .5f, 3, y - .5f);
                    Vector3 ceilingTopRight = new Vector3(x + .5f, 3, y + .5f);

                    v = new[] {ceilingBottomLeft, ceilingTopRight, ceilingTopLeft, ceilingBottomLeft, ceilingBottomRigth, ceilingTopRight};
                    
                    for (int j = 0; j < 6; j++) {
                        verticies.Add(v[j]);
                        triangles.Add(triangles.Count);
                    }
                }
                    
            }
        }
            
            
       Mesh mesh =  new Mesh()
        {
            vertices = verticies.ToArray(),
            triangles = triangles.ToArray()
        };
       
        mesh.RecalculateNormals();
        
        GameObject o = new GameObject()
        {
            name = "Corridors",
            tag = "Map"
        };
        
        o.AddComponent<MeshFilter>().mesh = mesh;

        return o;
    }

    public static void GenerateWalls(int wallHeight, Map map) {
        List<Vector3> verticies = new List<Vector3>();
        List<int> triangles = new List<int>();

        for(int x = 0; x < map.Size; x++ ){
            for (int y = 0; y < map.Size; y++) {
                if (map[x, y] != FieldType.Null) {
                    if (x > 0 && map[x - 1, y] == FieldType.Null) { //left
                        Vector3 bottomLeft = new Vector3(x - .5f, 0, y - .5f);
                        Vector3 topLeft = new Vector3(x - .5f, wallHeight, y - .5f);
                        Vector3 bottomRigth = new Vector3(x - .5f, 0, y + .5f);
                        Vector3 topRight = new Vector3(x - .5f, wallHeight, y + .5f);
                        
                        Vector3[] v = {bottomLeft, topLeft, topRight, bottomLeft, topRight, bottomRigth};
                        for (int j = 0; j < 6; j++) {
                            verticies.Add(v[j]);
                            triangles.Add(triangles.Count);
                        }
                    }

                    if (x < map.Size - 1 && map[x + 1, y] == FieldType.Null) { //right
                        Vector3 bottomLeft = new Vector3(x + .5f, 0, y + .5f);
                        Vector3 topLeft = new Vector3(x + .5f, wallHeight, y + .5f);
                        Vector3 bottomRigth = new Vector3(x + .5f, 0, y - .5f);
                        Vector3 topRight = new Vector3(x + .5f, wallHeight, y - .5f);
                        
                        Vector3[] v = {bottomLeft, topLeft, topRight, bottomLeft, topRight, bottomRigth};
                        for (int j = 0; j < 6; j++) {
                            verticies.Add(v[j]);
                            triangles.Add(triangles.Count);
                        }
                    }

                    if (y > 0 && map[x, y - 1] == FieldType.Null) { //down
                        Vector3 bottomLeft = new Vector3(x + .5f, 0, y - .5f);
                        Vector3 topLeft = new Vector3(x + .5f, wallHeight, y - .5f);
                        Vector3 bottomRigth = new Vector3(x - .5f, 0, y - .5f);
                        Vector3 topRight = new Vector3(x - .5f, wallHeight, y - .5f);
                        
                        Vector3[] v = {bottomLeft, topLeft, topRight, bottomLeft, topRight, bottomRigth};
                        for (int j = 0; j < 6; j++) {
                            verticies.Add(v[j]);
                            triangles.Add(triangles.Count);
                        }
                    }

                    if (y < map.Size - 1 && map[x, y + 1] == FieldType.Null) { //up
                        Vector3 bottomLeft = new Vector3(x - .5f, 0, y + .5f);
                        Vector3 topLeft = new Vector3(x - .5f, wallHeight, y + .5f);
                        Vector3 bottomRigth = new Vector3(x + .5f, 0, y + .5f);
                        Vector3 topRight = new Vector3(x + .5f, wallHeight, y + .5f);
                        
                        Vector3[] v = {bottomLeft, topLeft, topRight, bottomLeft, topRight, bottomRigth};
                        for (int j = 0; j < 6; j++) {
                            verticies.Add(v[j]);
                            triangles.Add(triangles.Count);
                        }
                    }
                }
                    
            }
        }
        
        Mesh mesh = new Mesh()
        {
            vertices = verticies.ToArray(),
            triangles = triangles.ToArray()
        };
        mesh.RecalculateNormals();
        GameObject o = new GameObject()
        {
            name = "Walls",
            tag = "Map"
        };
        o.AddComponent<MeshFilter>().mesh = mesh;
        o.AddComponent<MeshCollider>().sharedMesh = mesh;
        o.AddComponent<MeshRenderer>().material = (Material) Resources.Load("FloorMaterial", typeof(Material));
        o.AddComponent<NavMeshSurface>();
        o.GetComponent<NavMeshSurface>().layerMask = LayerMask.GetMask("Ground");
        o.GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    private static void BakeNavMesh() {
        
    }
}

