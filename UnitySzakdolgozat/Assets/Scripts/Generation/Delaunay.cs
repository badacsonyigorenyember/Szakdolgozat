using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GeometryUtils;

public static class Delaunay
{
    public static List<Edge> CalculateDelaunay(List<Room> rooms) {
        List<Triangle> triangulation = new List<Triangle>();
        Triangle superTriangle = new Triangle(new Vector2(-10000, -10000), new Vector2(10000, -10000),
            new Vector2(0, 10000)); 
        
        
        triangulation.Add(superTriangle);
        
        foreach (var point in rooms) { 
            List<Triangle> badTriangles = new List<Triangle>();

            foreach (var triangle in triangulation) { 
                if (triangle.InsideCircumcircle(point.center)) {
                    TriangleUtils.AddToTriangleList(triangle, badTriangles);
                }
            }

            Polygon polygon = new Polygon();
            
            foreach (var triangle in badTriangles) { 
                foreach (var edge in triangle.GetEdges()) {
                    if (!TriangleUtils.TriangleListContainsEdge(badTriangles, triangle, edge)) { 
                        polygon.Add(edge);
                    }
                }
            }
            
            foreach (var badTriangle in badTriangles) { 
                foreach (var triangle in triangulation) {
                    if (triangle.Equals(badTriangle)) {
                        triangulation.Remove(triangle);
                        break;
                    }
                }
            }

            foreach (var edge in polygon.edges) { 
                TriangleUtils.AddToTriangleList(new Triangle(point.center, edge.start, edge.end), triangulation);
            }
        }
        
    
        for (int i = triangulation.Count - 1; i >= 0 ; i--) { 
            if(triangulation[i].ContainsPoint(superTriangle.a)| triangulation[i].ContainsPoint(superTriangle.b) ||
               triangulation[i].ContainsPoint(superTriangle.c))
                triangulation.Remove(triangulation[i]);
            else {
                /*Debug.DrawLine(new Vector3(triangulation[i].a.x, 0, triangulation[i].a.y),
                    new Vector3(triangulation[i].b.x, 0, triangulation[i].b.y), Color.black);
                Debug.DrawLine(new Vector3(triangulation[i].a.x, 0, triangulation[i].a.y),
                    new Vector3(triangulation[i].c.x, 0, triangulation[i].c.y), Color.black);
                Debug.DrawLine(new Vector3(triangulation[i].c.x, 0, triangulation[i].c.y),
                    new Vector3(triangulation[i].b.x, 0, triangulation[i].b.y), Color.black);*/
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

    public static List<Edge> MinimalSpanningTree(List<Edge> edges, Vector2 start, int numberOfVertices) {
        List<Edge> mst = new List<Edge>();
        List<Vector2> visitedVerticies = new List<Vector2>() {
            start
        };
        
        while (visitedVerticies.Count != numberOfVertices) { //iterate untill we visited all of the verticies.
            List<Edge> possibleEdges = new List<Edge>();
            
            foreach (var vertex in visitedVerticies) { //iterate through the visited verticies, and collect all the possible edges.
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

        //foreach (var edge in mst) { Debug.DrawLine(new Vector3(edge.start.x, 0, edge.start.y), new Vector3(edge.end.x, .1f, edge.end.y), Color.magenta); }

        return mst;
    }

    public static List<Edge> FinalEdges(List<Room> rooms) {
        List<Edge> fullTriangulation = CalculateDelaunay(rooms);
        List<Edge> mst = MinimalSpanningTree(fullTriangulation, rooms[0].center, rooms.Count);
        List<Edge> finalPaths = new List<Edge>(mst.Count);
        finalPaths.AddRange(mst);
        
        
        foreach (var e in fullTriangulation) {
            if (!finalPaths.Any(edge => e.Equals(edge) )) {

                if (Random.Range(0f, 1f) <= 0.15f) {
                    finalPaths.Add(e);
                }
            }
        }

        foreach (var edge in finalPaths) {
            Room room1 = null, room2 = null;
            foreach (var room in rooms) {
                if (room.center == edge.start) {
                    room1 = room;
                    continue;
                }

                if (room.center == edge.end) 
                    room2 = room;
            }

            if (room1 != null && room2 != null) {
                Debug.Log(" room1 " + room1);
                (edge.start, edge.end) = room1.SelectEndPoints(room2);
                SetRoomAsNeighbour(room1, room2);
            }

        }
        
        //foreach (var edge in finalPaths) { Debug.DrawLine(new Vector3(edge.start.x, 0, edge.start.y), new Vector3(edge.end.x, 0.1f, edge.end.y), Color.magenta); }

        return finalPaths;
    }

    static void SetRoomAsNeighbour(Room a, Room b) {
        if(!a.neighbours.Contains(b))
            a.neighbours.Add(b);
        if(!b.neighbours.Contains(a))
            b.neighbours.Add(a);
    }

    public static void PathFinding(List<Edge> edges, List<Room> rooms, bool[,] map) {
        bool[,] everyPath = new bool[map.GetLength(0), map.GetLength(1)];
        int count = 0;

        foreach (var edge in edges) {
            Node start = new Node(edge.start, null);
            HashSet<Node> open = new HashSet<Node>(){ start };
            HashSet<Node> closed = new HashSet<Node>();
        
            while (open.Any()) {
                Node current = open.OrderBy(node => node.f).First();
                foreach (var node in open) {
                    if (node.f < current.f || node.f == current.f && node.h < current.h)
                        current = node;
                }

                open.Remove(current);
                closed.Add(current);

                if (current.pos == edge.end) {
                    RetracePath(current, everyPath);
                    break;
                }

                foreach (var neighbour in current.GetNeighbours(map, edge.end)) {
                    if(closed.Any(x => x.pos == neighbour.pos))
                        continue;

                    int multiplyer = 10;
                    count++;
                    
                    if (neighbour.HasRoomNextToIt())
                        multiplyer += 50;
                    if (everyPath[(int) neighbour.pos.x, (int) neighbour.pos.y]) 
                        multiplyer = 1;

                    int cost = current.g + current.GetDistance(neighbour.pos) * multiplyer;

                    if (cost < neighbour.g || !open.Any(x => x.pos == neighbour.pos)) {
                        neighbour.g = cost;
                        neighbour.h = neighbour.GetDistance(edge.end) * 10;
                        neighbour.parent = current;
                        if(!open.Any(x => x.pos == neighbour.pos))
                            open.Add(neighbour);
                
                    }

                }
        
                closed.Add(current);
            }
        }
        Debug.Log(count + " count");
        MapGeneration.GenerateCorridors(everyPath);
    }

    static void RetracePath(Node end, bool[,] everyPath) {
        Node current = end;
        while (current.parent != null) {
            Vector2Int curentPos = new Vector2Int((int)current.pos.x, (int)current.pos.y);
            everyPath[curentPos.x, curentPos.y] = true;
            //Debug.DrawLine(new Vector3(curentPos.x, 0, curentPos.y), new Vector3(current.parent.pos.x, 0, current.parent.pos.y), Color.black);
            current = current.parent;
        }
        everyPath[(int)current.pos.x, (int)current.pos.y] = true;
    }

    public static bool PathExists(Room start, Room end) {
        if (end.locked)
            return false;
        
        Queue<Room> rooms = new Queue<Room>();
        HashSet<Room> visited = new HashSet<Room>();
        rooms.Enqueue(start);
        visited.Add(start);

        while (rooms.Count > 0) {
            Room current = rooms.Dequeue();
            
            if (current == end) {
                return true;
            }
            
            foreach (var neighbor in current.neighbours) {
                Debug.Log("neighbour pos: " + neighbor.center);
                if (!visited.Contains(neighbor) && !neighbor.locked) {
                    rooms.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        return false;
    }
    
    
}
