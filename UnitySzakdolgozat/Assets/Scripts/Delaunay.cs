using System.Collections.Generic;
using UnityEngine;
using GeometryUtils;

public static class Delaunay 
{
    public static List<Edge> CalculateDelaunay(List<Room> rooms) {
        List<Triangle> triangulation = new List<Triangle>();
        Triangle superTriangle = new Triangle(new Vector2Int(-1000, -1000), new Vector2Int(1000, -1000),
            new Vector2Int(0, 1000)); 
        
        //better supertriangle
        
        triangulation.Add(superTriangle);
        
        foreach (var point in rooms) { //iterate through every room
            List<Triangle> badTriangles = new List<Triangle>();

            foreach (var triangle in triangulation) { //if one room is inside of a circumcircle of a triangle then add the triangle to the badTriangle list
                if (triangle.InsideCircumcircle(point)) {
                    TriangleUtils.AddToTriangleList(badTriangles, triangle);
                }
            }

            Polygon polygon = new Polygon();
            
            foreach (var triangle in badTriangles) { //iterate through every triangles in badTriangle and get their edges. If an edge is only part of one bad triangle then add it to the polygon.
                foreach (var edge in triangle.GetEdges()) {
                    if (!TriangleUtils.TriangleListContainsEdge(badTriangles, triangle, edge)) { 
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
                TriangleUtils.AddToTriangleList(triangulation, new Triangle(point.rect.position, edge.start, edge.end));
            }
        }
        
    
        for (int i = triangulation.Count - 1; i >= 0 ; i--) { //iterate through every triangles we need. If one of them contains a point from the supertriangle, delete it.
            if(triangulation[i].ContainsPoint(superTriangle.a)| triangulation[i].ContainsPoint(superTriangle.b) ||
               triangulation[i].ContainsPoint(superTriangle.c))
                triangulation.Remove(triangulation[i]);
            /*else {
                Debug.DrawLine(new Vector3(triangulation[i].a.x, 0, triangulation[i].a.y),
                    new Vector3(triangulation[i].b.x, 0, triangulation[i].b.y), Color.green);
                Debug.DrawLine(new Vector3(triangulation[i].a.x, 0, triangulation[i].a.y),
                    new Vector3(triangulation[i].c.x, 0, triangulation[i].c.y), Color.green);
                Debug.DrawLine(new Vector3(triangulation[i].c.x, 0, triangulation[i].c.y),
                    new Vector3(triangulation[i].b.x, 0, triangulation[i].b.y), Color.green);
            }*/
            
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

        foreach (var edge in mst) {
            Debug.DrawLine(new Vector3(edge.start.x, 0, edge.start.y), new Vector3(edge.end.x, .1f, edge.end.y), Color.magenta);
        }

        return mst;
    }

    public static void FinalPaths(List<Room> rooms) {
        List<Edge> fullTriangulation = CalculateDelaunay(rooms);
        List<Edge> mst = MinimalSpanningTree(fullTriangulation, rooms[0].rect.position, rooms.Count);
        List<Edge> finalPaths = new List<Edge>(mst.Count);
        finalPaths.AddRange(mst);
        
        foreach (var e in fullTriangulation) {
            if (!finalPaths.Contains(e) && !finalPaths.Contains(e.Reverse())) {
                if (Random.Range(0f, 1f) <= 0.15f) {
                    finalPaths.Add(e);
                }
            }
        }
        foreach (var edge in finalPaths) {
            if(!mst.Contains(edge))
                Debug.DrawLine(new Vector3(edge.start.x, 0, edge.start.y), new Vector3(edge.end.x, 0, edge.end.y), Color.yellow);
        }
        
    }
    
    
}
