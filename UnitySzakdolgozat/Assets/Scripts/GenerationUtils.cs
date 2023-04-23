using System.Collections.Generic;
using UnityEngine;

namespace GenerationUtils
{
    public class MeshGeneration
    {
        public static void GenerateRooms(List<Room> rooms, GameObject o) {
            List<Vector3> verticies = new List<Vector3>();
            List<int> triangles = new List<int>();
            
            foreach (var room in rooms) { 
                Vector3 bottomLeft = new Vector3(room.Position.x - .5f, 0, room.Position.y - .5f);
                Vector3 topLeft = new Vector3(room.Position.x - .5f, 0, room.Position.y + room.height + .5f);
                Vector3 bottomRigth = new Vector3(room.Position.x + room.width + .5f, 0, room.Position.y - .5f);
                Vector3 topRight = new Vector3(room.Position.x + room.width + .5f, 0, room.Position.y + room.height + .5f);
    
                Vector3[] v = {bottomLeft, topLeft, topRight, bottomLeft, topRight, bottomRigth};
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
            mesh.RecalculateNormals();

            o.AddComponent<MeshFilter>().mesh = mesh;
            o.AddComponent<MeshCollider>().sharedMesh = mesh;
            o.AddComponent<MeshRenderer>().material = (Material) Resources.Load("FloorMaterial", typeof(Material));
        }

        public static void GenerateCorridors(bool[,] map) {
            List<Vector3> verticies = new List<Vector3>();
            List<int> triangles = new List<int>();

            for (int x = 0; x < map.GetLength(0); x++) {
                for (int y = 0; y < map.GetLength(1); y++) {
                    if (map[x, y]) {
                        GameManager.map[x, y] = true;
                        Vector3 bottomLeft = new Vector3(x - .5f, 0, y - .5f);
                        Vector3 topLeft = new Vector3(x - .5f, 0, y + .5f);
                        Vector3 bottomRigth = new Vector3(x + .5f, 0, y - .5f);
                        Vector3 topRight = new Vector3(x + .5f, 0, y + .5f);
    
                        Vector3[] v = {bottomLeft, topLeft, topRight, bottomLeft, topRight, bottomRigth};
                        for (int j = 0; j < 6; j++) {
                            verticies.Add(v[j]);
                            triangles.Add(triangles.Count);
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
            GameObject g = new GameObject();
            g.AddComponent<MeshFilter>().mesh = mesh;
            g.AddComponent<MeshCollider>().sharedMesh = mesh;
            g.AddComponent<MeshRenderer>().material = (Material) Resources.Load("FloorMaterial", typeof(Material));

        }

        public static void GenerateWalls(bool[,] map, int wallHeight) {
            List<Vector3> verticies = new List<Vector3>();
            List<int> triangles = new List<int>();

            for(int x = 0; x < map.GetLength(0); x++ ){
                for (int y = 0; y < map.GetLength(1); y++) {
                    if (map[x, y]) {
                        if (x > 0 && !map[x - 1, y]) { //left
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

                        if (x < map.GetLength(0) - 1 && !map[x + 1, y]) { //right
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

                        if (y > 0 && !map[x, y - 1]) { //down
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

                        if (y < map.GetLength(1) - 1 && !map[x, y + 1]) { //up
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
            GameObject g = new GameObject();
            g.AddComponent<MeshFilter>().mesh = mesh;
            g.AddComponent<MeshCollider>().sharedMesh = mesh;
            g.AddComponent<MeshRenderer>().material = (Material) Resources.Load("FloorMaterial", typeof(Material));
        }
    }
}