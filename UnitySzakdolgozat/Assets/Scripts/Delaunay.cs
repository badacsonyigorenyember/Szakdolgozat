using System.Collections.Generic;
using UnityEngine;

public class Delaunay 
{
    public static List<Edge> CalculateDelaunay(List<Room> rooms) {
        List<Triangle> triangulation = new List<Triangle>();
        Triangle superTriangle = new Triangle(new Vector2Int(-1000, -1000), new Vector2Int(1000, -1000),
            new Vector2Int(0, 1000)); 
        
        //better supertriangle
        
        triangulation.Add(superTriangle);
        
        foreach (var point in rooms) { //iterate trough every room
            List<Triangle> badTriangles = new List<Triangle>();

            foreach (var triangle in triangulation) { //if one room is inside of a circumcircle of a triangle then add the triangle to the badTriangle list
                if (triangle.InsideCircumcircle(point)) {
                    AddToTriangleList(badTriangles, triangle);
                }
            }

            Polygon polygon = new Polygon();
            
            foreach (var triangle in badTriangles) { //iterate trough every triangles in badTriangle and get their edges. If an edge is only part of one bad triangle then add it to the polygon.
                foreach (var edge in triangle.GetEdges()) {
                    if (!TriangleListContainsEdge(badTriangles, triangle, edge)) { 
                        polygon.Add(edge);
                    }
                }
            }
            
            foreach (var badTriangle in badTriangles) { //remove the bad triangles from the triangulation list.
                foreach (var triangle in triangulation) {
                    if (triangle.Equals(badTriangle)) {
                        triangulation.Remove(triangle);
                        break;
                    }
                }
            }

            foreach (var edge in polygon.edges) { //create new triangles between the polygon's edges and the room.
                AddToTriangleList(triangulation, new Triangle(point.rect.position, edge.start, edge.end));
            }
        }
        
    
        for (int i = triangulation.Count - 1; i >= 0 ; i--) { //iterate trough every triangles we need. If one of them contains a point from the supertriangle, delete it.
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

        List<Edge> delaunayEdges = new List<Edge>();
        
        foreach (var t in triangulation) { //add the triangles edges to a list.
            foreach (var e in t.GetEdges()) {
                if (!delaunayEdges.Contains(e) && !delaunayEdges.Contains(e.Reverse())) 
                    delaunayEdges.Add(e);
            }
        }
        
        return delaunayEdges; //return the edges of the triangulation.
    }

    public static void MinimalSpanningTree(List<Edge> edges, Vector2 start, int numberOfVertices) {
        List<Edge> mst = new List<Edge>();
        List<Vector2> visitedVerticies = new List<Vector2>() {
            start
        };
        
        while (visitedVerticies.Count != numberOfVertices) { //iterate untill we visited all of the verticies.
            List<Edge> possibleEdges = new List<Edge>();
            
            foreach (var vertex in visitedVerticies) { //iterate trough the visited verticies, and collect all the possible edges.
                foreach (var edge in edges) {
                    if (edge.Contains(vertex) && !visitedVerticies.Contains(edge.OtherVertex(vertex))) 
                        possibleEdges.Add(edge);
                }
            }
            
            Edge minimalEdge = new Edge(float.MaxValue);
            
            foreach (var edge in possibleEdges) { //find the edge with the shortest length.
                if (edge.length < minimalEdge.length)
                    minimalEdge = edge;
            }
            
            mst.Add(minimalEdge); //add to minimal spanning tree.

            visitedVerticies.Add(visitedVerticies.Contains(minimalEdge.start) ? minimalEdge.end : minimalEdge.start); //add the next vertex to the visited list.
        }

        foreach (var edge in mst) {
            Debug.DrawLine(new Vector3(edge.start.x, 0, edge.start.y), new Vector3(edge.end.x, 0, edge.end.y), Color.magenta);
        }
    }
    
    private static void AddToTriangleList(List<Triangle> triangulation, Triangle triangle) {
        foreach (var t in triangulation) {
            if (t.Equals(triangle)) {
                return;
            }
        }
        triangulation.Add(triangle);
    }

    private static bool TriangleListContainsEdge(List<Triangle> triangles, Triangle triangle, Edge edge) {
        foreach (var t in triangles) {
            if(!t.Equals(triangle))
                foreach (var e in t.GetEdges()) {
                    if (e.Equals(edge))
                        return true;
                }
        }
        
        return false;
    }
    
    private class Triangle
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
            Vector2 abCenter = new Vector2((a.x + b.x) / 2, (a.y + b.y) / 2); //oldalfelezők
            Vector2 acCenter = new Vector2((a.x + c.x) / 2, (a.y + c.y) / 2);
            
            float abM = b.y - a.y == 0 ? 0 : (a.x - b.x) / (b.y - a.y); //oldalak
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
        
        public bool InsideCircumcircle(Room room) {
            return Vector2.Distance(room.rect.position, center) < circumcircleRadius;
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

    private class Polygon
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
}
