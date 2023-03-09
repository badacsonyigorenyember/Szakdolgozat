using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Delaunay 
{
    public static void CalculateDelaunay(List<Room> rooms) {
        int asd = 1;
        List<Triangle> triangulation = new List<Triangle>();
        Triangle superTriangle = new Triangle(new Vector2Int(-1000, -1000), new Vector2Int(1000, -1000),
            new Vector2Int(0, 1000));
        triangulation.Add(superTriangle);
        foreach (var point in rooms) {
            List<Triangle> badTriangles = new List<Triangle>();

            foreach (var triangle in triangulation) {
                if (triangle.InsideCircumcircle(point)) {
                    AddToList(badTriangles, triangle);
                }
            }
            

            Polygon polygon = new Polygon();
            foreach (var triangle in badTriangles) {
                foreach (var edge in triangle.edges) {
                    if (edge.ContainsEdge(badTriangles, triangle)) { 
                        polygon.Add(edge);
                    }
                }
            }


            foreach (var triangle in badTriangles) {
                foreach (var t in triangulation) {
                    if (t.Equals(triangle)) {
                        triangulation.Remove(triangle);
                        break;
                    }
                }
            }

            foreach (var edge in polygon.edges) {
                AddToList(triangulation, new Triangle(point.rect.position, edge.start, edge.end));
            }

        }
        
    
        for (int i = triangulation.Count - 1; i >= 0 ; i--) {
            if(triangulation[i].ContainsPoint(superTriangle.a)| triangulation[i].ContainsPoint(superTriangle.b) ||
               triangulation[i].ContainsPoint(superTriangle.c))
                triangulation.Remove(triangulation[i]);
            else {
                Debug.DrawLine(new Vector3(triangulation[i].a.x, 0, triangulation[i].a.y),
                    new Vector3(triangulation[i].b.x, 0, triangulation[i].b.y), Color.green);
                Debug.DrawLine(new Vector3(triangulation[i].a.x, 0, triangulation[i].a.y),
                    new Vector3(triangulation[i].c.x, 0, triangulation[i].c.y), Color.green);
                Debug.DrawLine(new Vector3(triangulation[i].c.x, 0, triangulation[i].c.y),
                    new Vector3(triangulation[i].b.x, 0, triangulation[i].b.y), Color.green);
            }
            
        }

        Debug.Log(triangulation.Count);


    }

    public static void AddToList(List<Triangle> triangulation, Triangle triangle) {
        foreach (var t in triangulation) {
            if (t.Equals(triangle)) {
                Debug.Log("kilép");
                return;
            }
        }
        triangulation.Add(triangle);
    }

    
    
    public class Triangle
    {
        public Vector2 a;
        public Vector2 b;
        public Vector2 c;
        public Vector2 center;
        public float circumcircleRadius;
        public List<Edge> edges;

        public Triangle(Vector2 a, Vector2 b, Vector2 c) {
            this.a = a;
            this.b = b;
            this.c = c;
            CalculateCircumcircle();
            edges = new List<Edge>() {
                new(a, b), new(a, c), new(b, c)
            };
        }

        public bool Equals(Triangle t) {
            if (t == null) return false;
            if (a == t.a && b == t.b && c == t.c ||
                a == t.a && b == t.c && c == t.b ||
                a == t.b && b == t.a && c == t.c ||
                a == t.b && b == t.c && c == t.a ||
                a == t.c && b == t.a && c == t.b ||
                a == t.c && b == t.b && c == t.a) 
                return true;
            return false;

        }

        public bool ContainsPoint(Vector2 point) {
            if (point == a || point == b || point == c) return 
                true;
            return false;
        }
        

        private void CalculateCircumcircle() {
            Vector2 abCenter = new Vector2((a.x + b.x) / 2, (a.y + b.y) / 2);
            Vector2 acCenter = new Vector2((a.x + c.x) / 2, (a.y + c.y) / 2);
            
            float abM = b.x - a.x == 0 ? 0 : (b.y - a.y) / (b.x - a.x);
            float acM = c.x - a.x == 0 ? 0 : (c.y - a.y) / (c.x - a.x);
            
            float negRecAbM = abM == 0 ? 0 : -1 / abM;
            float negRecAcM = acM == 0 ? 0 : -1 / acM;
            
            float abB = abCenter.y - negRecAbM * abCenter.x;
            float acB = acCenter.y - negRecAcM * acCenter.x;

            float x, y;

            if (a.x == b.x) {
                y = abCenter.y;
                x = (y - acB) / negRecAcM;
            }else if (a.x == c.x) {
                y = acCenter.y;
                x = (y - abB) / negRecAbM;
            }else
                x = (acB - abB) / (negRecAbM - negRecAcM);

            if (a.y == b.y) {
                x = abCenter.x;
                y = negRecAcM * x + acB;
            }else if (a.y == c.y) {
                x = acCenter.x;
                y = negRecAbM * x + abB;
            }else 
                y = negRecAbM * x + abB;

            center = new Vector2(x, y);
            circumcircleRadius = Vector2.Distance(center, a);
        }
        
        public bool InsideCircumcircle(Room room) {
            return Vector2.Distance(room.rect.position, center) < circumcircleRadius;
        }

        
    }

    public class Polygon
    {
        public List<Edge> edges;

        public Polygon() {
            edges = new List<Edge>();
        }

        public void Add(Edge edge) {
            foreach (var e in edges) {
                if (e.Equals(edge)) return;
            }
            edges.Add(edge);
        }
        
    }
    
    
    public class Edge
    {
        public Vector2 start;
        public Vector2 end;

        public Edge(Vector2 start, Vector2 end) {
            this.start = start;
            this.end = end;
        }
            
        public bool ContainsEdge(List<Triangle> triangles, Triangle actualTriangle) {
            foreach (var t in triangles) {
                if(t.Equals(actualTriangle)) 
                    continue;
                foreach (var e in t.edges) {
                    if (Equals(e))
                        return false;
                }
            }

            return true;
        }

        private bool Equals(Edge e) {
            if (start == e.start && end == e.end || start == e.end && end == e.start) 
                return true;
            return false;
        }
    }
}
