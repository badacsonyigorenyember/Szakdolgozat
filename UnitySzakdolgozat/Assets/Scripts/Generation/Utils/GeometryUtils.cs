using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace GeometryUtils

{
    public class TriangleUtils
    {
        public static void AddToTriangleList(Triangle triangle, List<Triangle> triangulation) {
            foreach (var t in triangulation) {
                if (t.Equals(triangle)) {
                    return;
                }
            }
            triangulation.Add(triangle);
        }
        
        public static bool TriangleListContainsEdge(List<Triangle> triangles, Triangle triangle, Edge edge) {
            foreach (var t in triangles) {
                if(!t.Equals(triangle))
                    foreach (var e in t.GetEdges()) {
                        if (e.Equals(edge))
                            return true;
                    }
            }
        
            return false;
        }
    }
    
    public class Triangle
    {
        public Vector2 a;
        public Vector2 b;
        public Vector2 c;
        private Vector2 center;
        private float circumcircleRadius;

        public Triangle(Vector2 a, Vector2 b, Vector2 c) {
            this.a = a;
            this.b = b;
            this.c = c;
            CalculateCircumcircle();
            
        }

        public bool Equals(Triangle t) {
            return t != null && 
                   (a == t.a && b == t.b && c == t.c ||
                   a == t.a && b == t.c && c == t.b ||
                   a == t.b && b == t.a && c == t.c ||
                   a == t.b && b == t.c && c == t.a ||
                   a == t.c && b == t.a && c == t.b ||
                   a == t.c && b == t.b && c == t.a);

        }

        public bool ContainsPoint(Vector2 point) {
            if (point == a || point == b || point == c) return 
                true;
            return false;
        }

        private void CalculateCircumcircle() {
            Vector2 abCenter = new Vector2((a.x + b.x) / 2, (a.y + b.y) / 2); 
            Vector2 acCenter = new Vector2((a.x + c.x) / 2, (a.y + c.y) / 2);
            
            float abM = b.y - a.y == 0 ? 0 : (a.x - b.x) / (b.y - a.y); 
            float acM = c.y - a.y == 0 ? 0 : (a.x - c.x) / (c.y - a.y);

            float abB = abCenter.y - abM * abCenter.x;
            float acB = acCenter.y - acM * acCenter.x;

            float x, y;
            
            if (Mathf.Approximately(a.x, b.x)) {
                y = abCenter.y;
                x = (y - acB) / acM;
            }else if (Mathf.Approximately(a.x, c.x)) {
                y = acCenter.y;
                x = (y - abB) / abM;
            }else
                x = (acB - abB) / (abM - acM);

            if (Mathf.Approximately(a.y, b.y)) {
                x = abCenter.x;
                y = acM * x + acB;
            }else if (Mathf.Approximately(a.y, c.y)) {
                x = acCenter.x;
                y = abM * x + abB;
            }else 
                y = abM * x + abB;

            center = new Vector2(x, y);
            circumcircleRadius = Vector2.Distance(center, a);
        }
        
        public bool InsideCircumcircle(Vector2 point) {
            return Vector2.Distance(point, center) < circumcircleRadius;
        }

        public List<Edge> GetEdges() {
            return new List<Edge>() {
                new(a, b), new(a, c), new(b, c)
            };
        }

    }

    public class Edge
    {
        public Vector2 start;
        public Vector2 end;
        public readonly float length;

        public Edge(Vector2 start, Vector2 end) {
            this.start = start;
            this.end = end;
            length = Vector2.Distance(this.start, this.end);
        }

        public Edge(float length) {
            this.length = length;
            start = new Vector2(0, 0);
            end = new Vector2(0, 0);
        }

        public bool Equals(Edge e) {
            if (start == e.start && end == e.end || start == e.end && end == e.start) 
                return true;
            return false;
        }

        public Edge Reverse() {
            return new Edge(end, start);
        }

        public bool Contains(Vector2 vertex) {
            return start == vertex || end == vertex;
        }

        public Vector2 OtherVertex(Vector2 vertex) {
            return start == vertex ? end : start;
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

    public class Node
    {
        public Vector2 pos;
        public Node parent;
        public int g, h;
        
        public int f => g + h;
        
        public Node(Vector2 pos, Node parent) {
            this.pos = pos;
            this.parent = parent;
            g = 0;
            h = 0;
        }

        public List<Node> GetNeighbours(int mapSize) {
            List<Node> neighbours = new List<Node>()
            {
                new(new Vector2(this.pos.x - 1, this.pos.y), this),
                new(new Vector2(this.pos.x + 1, this.pos.y), this),
                new(new Vector2(this.pos.x, this.pos.y - 1), this),
                new(new Vector2(this.pos.x, this.pos.y + 1), this)
            };
            
            return neighbours.Where(n =>
                n.pos.x >= 0 && n.pos.x < mapSize &&
                n.pos.y >= 0 && n.pos.y < mapSize).ToList();
        }

        public bool HasRoomNextToIt(int mapSize) {
            for(int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    Vector2 position = this.pos + new Vector2(x, y);
                    if(position.x >= 0 && position.x < mapSize && 
                       position.y >= 0 && position.y < mapSize)
                        if (GameManager.Map[position.x, position.y] == FieldType.Room)
                            return true;
                }
                
            }

            return false;
        }

        public int GetDistance(Vector2 other) {
            int dstX = (int)Math.Abs(other.x - pos.x);
            int dstY = (int)Math.Abs(other.y - pos.y);

            return dstX + dstY;

        }
        
    }

}