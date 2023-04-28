using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeometryUtils;
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
    public Vector2[] sidePositions;
    private BitArray usedSidePositions;
    public List<Room> neighbours;
    public bool locked;

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
        
        usedSidePositions = new BitArray(new[] { 0b0000 });
        neighbours = new List<Room>();
        locked = false;
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
            new (position.x + width, center.y + Random.Range(-(height / 4), height / 4)),
            new (center.x + Random.Range(-(width / 4), width / 4), position.y + height),
            new (position.x, center.y + Random.Range(-(height / 4), height / 4)),
            new (center.x + Random.Range(-(width / 4), width / 4), position.y)
        };
        
    }

    public List<Vector2> DoorsLocations() {
        return sidePositions.Where((p, i) => usedSidePositions[i]).ToList();
    }

    public (Vector2, Vector2) SelectEndPoints(Room other) {
        float angle = Mathf.Atan2(other.center.y - center.y, other.center.x - center.x) * Mathf.Rad2Deg + 45;
        angle = angle < 0 ? angle + 360 : angle;
        angle %= 360;
        Vector2 thisSidePosition = sidePositions[0];
        Vector2 otherSidePosition = sidePositions[0];
        
        
        switch (angle) {
            case var n when (n > 0 && n <= 90):
                thisSidePosition = sidePositions[0];
                otherSidePosition = other.sidePositions[2];
                usedSidePositions[0] = true;
                other.usedSidePositions[2] = true;
                break;
            case var n when (n > 90 && n <= 180):
                thisSidePosition = sidePositions[1];
                otherSidePosition = other.sidePositions[3];
                usedSidePositions[1] = true;
                other.usedSidePositions[3] = true;
                break;
            case var n when (n > 180 && n <= 270):
                thisSidePosition = sidePositions[2];
                otherSidePosition = other.sidePositions[0];
                usedSidePositions[2] = true;
                other.usedSidePositions[0] = true;
                break;
            case var n when (n > 270 && n <= 360):
                thisSidePosition = sidePositions[3];
                otherSidePosition = other.sidePositions[1];
                usedSidePositions[3] = true;
                other.usedSidePositions[1] = true;
                break;
        }

        return (thisSidePosition, otherSidePosition);
    }

    public List<Vector2> GetWallPositions() {
        List<Vector2> walls = new List<Vector2>();
        for (int y = (int) position.y; y < position.y + height; y++) {
            if (!usedSidePositions[0] || y != (int) sidePositions[0].y)
                walls.Add(new Vector2(position.x, y));
            if (!usedSidePositions[2] || y != (int) sidePositions[2].y)
                walls.Add(new Vector2(position.x + width, y));
        }
        
        for (int x = (int) position.x; x < position.x + width; x++) {
            if (!usedSidePositions[1] || x != (int) sidePositions[1].x)
                walls.Add(new Vector2(x, position.y + height));
            if (!usedSidePositions[3] || x != (int) sidePositions[3].x)
                walls.Add(new Vector2(x, position.y));
        }

        return walls;
    }
}