using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Delaunay 
{
    public static void CalculateDelaunay(List<Room> rooms) {
        // List<Triangle> triangulation = new List<Triangle>();
        // Triangle superTriangle = new Triangle(new Vector2Int(-1000, -1000), new Vector2Int(1000, -1000),
        //     new Vector2Int(0, 1000));
        // triangulation.Add(superTriangle);
        // foreach (var point in rooms) {
        //     List<Triangle> badTriangles = new List<Triangle>();
        //     foreach (var triangle in triangulation) {
        //         
        //     }
        //
        // }

        Triangle a = new Triangle(new Vector2(11, 3), new Vector2(67, 3), new Vector2(98, 4));

    }

    private bool InsideCircumcircle(Room room, Triangle triangle) {
        Vector2 center = triangle.a + triangle.b + triangle.c / 3;
        return false;
    }
    
    class Triangle
    {
        public Vector2 a;
        public Vector2 b;
        public Vector2 c;
        public float sideLength;
        public Vector2 center;
        public float circumcircleRadius;

        public Triangle(Vector2 a, Vector2 b, Vector2 c) {
            this.a = a;
            this.b = b;
            this.c = c;
            CalculateCircumcircle();
        }

        public float CalculateSideLength(Vector2 a, Vector2 b) {
            return Vector2.Distance(a, b);
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
            Debug.Log(center);
            circumcircleRadius = Vector2.Distance(center, a);
        }
        
    }

    class Polygon
    {
        public List<Vector2> points;

        public Polygon() {
            points = new List<Vector2>();
        }

        public void Add(Vector2 point) {
            points.Add(point);
        }
        
    }
}
