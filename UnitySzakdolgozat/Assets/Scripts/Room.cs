using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Room
{
    public int width;
    public int height;
    private Vector2 position;
    public Vector2 center;
    public Vector2 placeHolderPosition;
    public int placeHolderSize;
    private Vector2[] sidePositions;

    public Vector2 Position {
        get => position;
        set {
            position = value;
            center = center = new Vector2((int) (position.x + width / 2), (int) (position.y + height / 2));
            placeHolderPosition = new Vector2(value.x - 2, value.y - 2);
            placeHolderSize = 4;
        }
    }


    public Room(int mapSize, int maxSize) {
        int distance = Random.Range(0, mapSize);
        float angle = Random.Range(0, 2 * Mathf.PI);
        
        width = Random.Range(5, maxSize);
        height = Random.Range(5, maxSize);

        Position = new Vector2((int) (distance * Mathf.Cos(angle)), (int) (distance * Mathf.Sin(angle)));
       
        
    }

    public Room(Vector2 pos) {
        this.center = pos;
    }

    public bool Overlaps(Room other) {
        float left = Math.Max(placeHolderPosition.x, other.placeHolderPosition.x);
        float right = Math.Min(placeHolderPosition.x + width + placeHolderSize, other.placeHolderPosition.x + other.width + other.placeHolderSize);

        if (left < right)
        {
            float lower = Math.Max(placeHolderPosition.y, other.placeHolderPosition.y);
            float upper = Math.Min(placeHolderPosition.y + height + placeHolderSize, other.placeHolderPosition.y + other.height + other.placeHolderSize);

            if (lower < upper)
            {
                return true;
            }
        }

        return false;
    }

    public void ClaimArea(bool[,] map) {
        for (int x = (int) position.x; x <= position.x + width; x++) {
            for (int y = (int) position.y; y <= position.y + height; y++) {
                map[x, y] = true;
            }
        }
        
        sidePositions = new Vector2[4]
        {
            new (position.x + width + 1, center.y + Random.Range(-(height / 4), height / 4)),
            new (center.x + Random.Range(-(width / 4), width / 4), position.y + height + 1),
            new (position.x - 1, center.y + Random.Range(-(height / 4), height / 4)),
            new (center.x + Random.Range(-(width / 4), width / 4), position.y - 1)
        };
        
    }

    public bool ContainsPoint(Vector2 point) {
        return point.x > position.x && point.x < position.x + width &&
               point.y > position.y && point.y < position.y + height;
    }

    public (Vector2, Vector2) SelectEndPoints(Room other) {
        float angle = Mathf.Atan2(other.center.y - center.y, other.center.x - center.x) * Mathf.Rad2Deg + 45;
        angle = angle < 0 ? angle + 360 : angle;
        angle %= 360;
        Vector2 room1SidePosition = sidePositions[0];
        Vector2 room2SidePosition = sidePositions[0];
        
        
        switch (angle) {
            case var n when (n > 0 && n <= 90):
                room1SidePosition = sidePositions[0];
                room2SidePosition = other.sidePositions[1];
                break;
            case var n when (n > 90 && n <= 180):
                room1SidePosition = sidePositions[1];
                room2SidePosition = other.sidePositions[3];
                break;
            case var n when (n > 180 && n <= 270):
                room1SidePosition = sidePositions[2];
                room2SidePosition = other.sidePositions[0];
                break;
            case var n when (n > 270 && n <= 360):
                room1SidePosition = sidePositions[3];
                room2SidePosition = other.sidePositions[1];
                break;
        }

        return (room1SidePosition, room2SidePosition);
    }
    
}