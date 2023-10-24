using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GeometryUtils;
using Utils;

public static class Delaunay
{
    private static List<Edge> CalculateDelaunay(List<Room> rooms) {
        List<Triangle> triangulation = new List<Triangle>();
        Triangle superTriangle = new Triangle(new Vector2(-10000, -10000), new Vector2(10000, -10000),
            new Vector2(0, 10000)); 
        
        
        triangulation.Add(superTriangle);
        
        foreach (var point in rooms) { 
            List<Triangle> badTriangles = new List<Triangle>();

            foreach (var triangle in triangulation) { 
                if (triangle.InsideCircumcircle(point.area.center)) {
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
                TriangleUtils.AddToTriangleList(new Triangle(point.area.center, edge.start, edge.end), triangulation);
            }
        }
        
    
        for (int i = triangulation.Count - 1; i >= 0 ; i--) { 
            if(triangulation[i].ContainsPoint(superTriangle.a)| triangulation[i].ContainsPoint(superTriangle.b) ||
               triangulation[i].ContainsPoint(superTriangle.c))
                triangulation.Remove(triangulation[i]);

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

    private static List<Edge> MinimalSpanningTree(List<Edge> edges, Vector2 start, int numberOfVertices) {
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


        return mst;
    }

    public static List<Edge> FinalEdges(List<Room> rooms) {
        List<Edge> fullTriangulation = CalculateDelaunay(rooms);
        List<Edge> mst = MinimalSpanningTree(fullTriangulation, rooms[0].area.center, rooms.Count);
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
                if (room.area.center == edge.start) {
                    room1 = room;
                    continue;
                }

                if (room.area.center == edge.end) 
                    room2 = room;
            }

            if (room1 != null && room2 != null) {
                (edge.start, edge.end) = room1.SelectEndPoints(room2);
                SetRoomAsNeighbour(room1, room2);
            }

        }

        return finalPaths;
    }

    private static void SetRoomAsNeighbour(Room a, Room b) {
        if(!a.neighbours.Contains(b))
            a.neighbours.Add(b);
        if(!b.neighbours.Contains(a))
            b.neighbours.Add(a);
    }

    public static void PathFinding(List<Edge> edges, Map map) {
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
                    RetracePath(current, map);
                    break;
                }
                
                foreach (var neighbour in current.GetNeighbours(map.Size)) {
                    if(closed.Any(x => x.pos == neighbour.pos))
                        continue;

                    int multiplyer = 10;
                    
                    if (neighbour.HasRoomNextToIt(map.Size))
                        multiplyer += 50;
                    if (map[neighbour.pos.x, neighbour.pos.y] == FieldType.Corridor) 
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
        
    }

    private static void RetracePath(Node end, Map map) {
        Node current = end;
        while (current.parent != null) {
            if(map[current.pos.x, current.pos.y] != FieldType.Room)
                map[current.pos.x, current.pos.y] = FieldType.Corridor;
            current = current.parent;
        }
        if(map[current.pos.x, current.pos.y] != FieldType.Room)
            map[current.pos.x, current.pos.y] = FieldType.Corridor;
    }

}
