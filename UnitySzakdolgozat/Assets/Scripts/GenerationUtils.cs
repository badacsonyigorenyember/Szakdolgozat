using System.Collections.Generic;
using GeometryUtils;
using UnityEngine;
using Edge = GeometryUtils.Edge;

namespace GenerationUtils
{
    public class MeshGeneration
    {
        public static Mesh GenerateRooms(List<Room> rooms, bool[,] map) {
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
            

            Mesh mesh = new Mesh()
            {
                vertices = verticies.ToArray(),
                triangles = triangles.ToArray()
            };
            mesh.RecalculateNormals();
            return mesh;
        }

        public static Mesh GenerateCorridors(List<Edge> edges) {
            List<Vector3> verticies = new List<Vector3>();
            List<int> triangles = new List<int>();

            return null;
        }

        
        
    }
}