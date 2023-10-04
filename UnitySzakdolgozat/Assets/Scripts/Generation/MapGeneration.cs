using System.Collections.Generic;
using GeometryUtils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using Utils;

public class MapGeneration : MonoBehaviour
{
    public static void GenerateMap(int roomCount, int mapSize, int maxRoomSize) {
        float time = Time.realtimeSinceStartup;
        List<Room> rooms;
        (rooms, mapSize) = RoomGeneration.CreateRooms(roomCount, mapSize, maxRoomSize);
        GameManager.availableRooms = rooms;
        GameManager.map = new Map((mapSize + maxRoomSize) * 2 + 1);
        rooms.ForEach(r => r.ClaimArea(GameManager.map));
        
        List<Edge> edges = Delaunay.FinalEdges(rooms);
        Debug.Log("háromszög után: " + (Time.realtimeSinceStartup - time));

        Delaunay.PathFinding(edges, rooms, GameManager.map);

        GameObject map = new GameObject {name = "Map", tag = "Map" };
        GenerateFloor(GameManager.map, map);
        GenerateWalls(3, GameManager.map);

        GameManager.rooms = rooms;
        Debug.Log("minden után: " + (Time.realtimeSinceStartup - time));

    }

    public static void GenerateFloor(Map map, GameObject go) {
        GameObject floorPrefab = Resources.Load<GameObject>("Prefabs/Floor");

        List<CombineInstance> floorInstances = new List<CombineInstance>();
        List<CombineInstance> ceilingInstances = new List<CombineInstance>();
        
        for(int x = 0; x < map.Size; x++ ){
            for (int y = 0; y < map.Size; y++) {
                if (map[x, y] == FieldType.Room || map[x, y] == FieldType.Corridor) {
                    GameObject obj = Instantiate(floorPrefab, new Vector3(x, 0, y), Quaternion.identity);

                    Mesh mesh = obj.GetComponent<MeshFilter>().sharedMesh; 
                    Matrix4x4 localToWorldMatrix = obj.transform.localToWorldMatrix;
                    
                    floorInstances.Add(new CombineInstance
                    {
                        mesh = mesh,
                        transform = localToWorldMatrix
                    });

                    obj.transform.position = new Vector3(x, 3, y);
                    obj.transform.rotation = Quaternion.Euler(180, 0, 0);
                    localToWorldMatrix = obj.transform.localToWorldMatrix;
                    ceilingInstances.Add(new CombineInstance
                    {
                        mesh = mesh,
                        transform = localToWorldMatrix
                    });
                    
                    Destroy(obj);
                }
            }
        }

        GameObject floor = new GameObject
        {
            name = "Floor",
            layer = LayerMask.NameToLayer("Ground"),
            transform = { parent = go.transform }
        };
        
        GameObject ceiling = new GameObject { name = "Ceiling" , transform = { parent = go.transform }};

        Mesh floorMesh = new Mesh{ indexFormat = IndexFormat.UInt32 };
        floorMesh.CombineMeshes(floorInstances.ToArray());
        floorMesh.RecalculateNormals();
        floorMesh.RecalculateTangents();

        Mesh ceilingMesh = new Mesh {indexFormat = IndexFormat.UInt32 };
        ceilingMesh.CombineMeshes(ceilingInstances.ToArray());
        ceilingMesh.RecalculateNormals();
        ceilingMesh.RecalculateTangents();

        floor.AddComponent<MeshFilter>().mesh = floorMesh;
        floor.AddComponent<MeshCollider>().sharedMesh = floorMesh;
        floor.AddComponent<MeshRenderer>().material = (Material) Resources.Load("Materials/Floor", typeof(Material));
        
        floor.AddComponent<NavMeshSurface>();
        floor.GetComponent<NavMeshSurface>().layerMask = LayerMask.GetMask("Ground");
        floor.GetComponent<NavMeshSurface>().BuildNavMesh();
        
        ceiling.AddComponent<MeshFilter>().mesh = ceilingMesh;
        ceiling.AddComponent<MeshRenderer>().material = (Material) Resources.Load("Materials/Ceiling", typeof(Material));

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
        o.AddComponent<MeshRenderer>().material = (Material) Resources.Load("Materials/Floor", typeof(Material));
        o.AddComponent<MeshCollider>().sharedMesh = floor;
        
        

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

    public static void GenerateWalls(int wallHeigh, Map map) {
        GameObject wallPrefab = Resources.Load<GameObject>("Prefabs/Wall");
        
        List<CombineInstance> wallInstances = new List<CombineInstance>();
        List<CombineInstance> skirtingInstances = new List<CombineInstance>();

        for (int x = 0; x < map.Size; x++) {
            for (int y = 0; y < map.Size; y++) {
                if (map[x, y] != FieldType.Null) {
                    if (x > 0 && map[x - 1, y] == FieldType.Null) { //left
                        GameObject obj = Instantiate(wallPrefab, new Vector3(x, 0, y), Quaternion.Euler(0, 180, 0));

                        wallInstances.Add(new CombineInstance
                        {
                            mesh = obj.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh,
                            transform = obj.transform.GetChild(0).localToWorldMatrix
                        });
                        skirtingInstances.Add(new CombineInstance
                        {
                            mesh = obj.transform.GetChild(1).GetComponent<MeshFilter>().sharedMesh,
                            transform = obj.transform.GetChild(1).localToWorldMatrix
                        });
                        
                        Destroy(obj);

                    }

                    if (x < map.Size - 1 && map[x + 1, y] == FieldType.Null) { //right
                        GameObject obj = Instantiate(wallPrefab, new Vector3(x, 0, y), Quaternion.Euler(0, 0, 0));
                        wallInstances.Add(new CombineInstance
                        {
                            mesh = obj.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh,
                            transform = obj.transform.GetChild(0).localToWorldMatrix
                        });
                        skirtingInstances.Add(new CombineInstance
                        {
                            mesh = obj.transform.GetChild(1).GetComponent<MeshFilter>().sharedMesh,
                            transform = obj.transform.GetChild(1).localToWorldMatrix
                        });
                        
                        Destroy(obj);
                    }

                    if (y > 0 && map[x, y - 1] == FieldType.Null) { //down
                        GameObject obj = Instantiate(wallPrefab, new Vector3(x, 0, y), Quaternion.Euler(0, 90, 0));
                        wallInstances.Add(new CombineInstance
                        {
                            mesh = obj.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh,
                            transform = obj.transform.GetChild(0).localToWorldMatrix
                        });
                        skirtingInstances.Add(new CombineInstance
                        {
                            mesh = obj.transform.GetChild(1).GetComponent<MeshFilter>().sharedMesh,
                            transform = obj.transform.GetChild(1).localToWorldMatrix
                        });
                        
                        Destroy(obj);
                    }

                    if (y < map.Size - 1 && map[x, y + 1] == FieldType.Null) { //up
                        GameObject obj = Instantiate(wallPrefab, new Vector3(x, 0, y), Quaternion.Euler(0, -90, 0));
                        wallInstances.Add(new CombineInstance
                        {
                            mesh = obj.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh,
                            transform = obj.transform.GetChild(0).localToWorldMatrix
                        });
                        skirtingInstances.Add(new CombineInstance
                        {
                            mesh = obj.transform.GetChild(1).GetComponent<MeshFilter>().sharedMesh,
                            transform = obj.transform.GetChild(1).localToWorldMatrix
                        });
                        
                        Destroy(obj);
                    }
                }
            }
        }
        
        GameObject wall = new GameObject
        {
            name = "Wall",
            tag = "Map"
        };
        GameObject wallSide = new GameObject
        {
            name = "Wallside",
            transform =
            {
                parent = wall.transform
            }
        };

        GameObject skirting = new GameObject
        {
            name = "Skirting",
            transform =
            {
                parent = wall.transform
            }
        };

        Mesh wallMesh = new Mesh{ indexFormat = IndexFormat.UInt32 };
        wallMesh.CombineMeshes(wallInstances.ToArray());
        wallMesh.RecalculateNormals();
        wallMesh.RecalculateTangents();

        Mesh skirtingMesh = new Mesh {indexFormat = IndexFormat.UInt32 };
        skirtingMesh.CombineMeshes(skirtingInstances.ToArray());
        skirtingMesh.RecalculateNormals();
        skirtingMesh.RecalculateTangents();


        wallSide.AddComponent<MeshFilter>().mesh = wallMesh;
        wallSide.AddComponent<MeshCollider>().sharedMesh = wallMesh;
        wallSide.AddComponent<MeshRenderer>().material = (Material) Resources.Load("Materials/Wall", typeof(Material));

        skirting.AddComponent<MeshFilter>().mesh = skirtingMesh;
        skirting.AddComponent<MeshRenderer>().material = (Material) Resources.Load("Materials/Skirting", typeof(Material));

    }
    
}

