using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

public class Room
{
    public Rect area;
    private Rect placeHolder;
    public Vector2[] sidePositions;
    private BitArray usedSidePositions;
    public List<Room> neighbours;
    public bool locked;
    public List<Door> doors;
    private List<Vector2> occupiedPositions;


    public Room(int mapSize, int maxSize) {
        int distance = Random.Range(0, mapSize - maxSize - 1);
        float angle = Random.Range(0, 2 * Mathf.PI);

        Vector2 areaPos = new Vector2((int) (distance * Mathf.Cos(angle)), (int) (distance * Mathf.Sin(angle)));
        
        area = new Rect(areaPos, Random.Range(5, maxSize), Random.Range(5, maxSize));

        placeHolder = new Rect(area.position - Vector2.one * 2, area.width + 4, area.height + 4);
        
        usedSidePositions = new BitArray(new[] { 0b0000 });
        neighbours = new List<Room>();
        locked = false;
        doors = new List<Door>();
        occupiedPositions = new List<Vector2>();
    }

    public bool Overlaps(Room other) {
        return placeHolder.Overlaps(other.placeHolder);
    }

    public void ClaimArea(Map map) {
        for (int x = (int) area.position.x; x <= area.position.x + area.width; x++) {
            for (int y = (int) area.position.y; y <= area.position.y + area.height; y++) {
                map[x, y] = FieldType.Room;
            }
        }
        
        sidePositions = new Vector2[]
        {
            new (area.position.x + area.width, area.center.y + Random.Range(-(area.height / 4), area.height / 4)),
            new (area.center.x + Random.Range(-(area.width / 4), area.width / 4), area.position.y + area.height),
            new (area.position.x, area.center.y + Random.Range(-(area.height / 4), area.height / 4)),
            new (area.center.x + Random.Range(-(area.width / 4), area.width / 4), area.position.y)
        };

    }

    public List<Vector2> DoorsLocations() {
        return sidePositions.Where((_, i) => usedSidePositions[i]).ToList();
    }

    public (Vector2, Vector2) SelectEndPoints(Room other) {
        float angle = Mathf.Atan2(other.area.center.y - area.center.y, other.area.center.x - area.center.x) * Mathf.Rad2Deg + 45;
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
                
                occupiedPositions.Add(sidePositions[0]);
                other.occupiedPositions.Add(other.sidePositions[2]);
                
                break;
            case var n when (n > 90 && n <= 180):
                thisSidePosition = sidePositions[1];
                otherSidePosition = other.sidePositions[3];
                
                usedSidePositions[1] = true;
                other.usedSidePositions[3] = true;
                
                occupiedPositions.Add(sidePositions[1]);
                other.occupiedPositions.Add(other.sidePositions[3]);
                
                break;
            case var n when (n > 180 && n <= 270):
                thisSidePosition = sidePositions[2];
                otherSidePosition = other.sidePositions[0];
                
                usedSidePositions[2] = true;
                other.usedSidePositions[0] = true;
                
                occupiedPositions.Add(sidePositions[2]);
                other.occupiedPositions.Add(other.sidePositions[0]);
                
                break;
            case var n when (n > 270 && n <= 360):
                thisSidePosition = sidePositions[3];
                otherSidePosition = other.sidePositions[1];
                
                usedSidePositions[3] = true;
                other.usedSidePositions[1] = true;
                
                occupiedPositions.Add(sidePositions[3]);
                other.occupiedPositions.Add(other.sidePositions[1]);
                
                break;
        }

        return (thisSidePosition, otherSidePosition);
    }

    public List<Vector2> GetWallPositions() {
        List<Vector2> walls = new List<Vector2>();
        for (int y = (int) area.position.y + 1; y < area.position.y + area.height - 1; y++) {
            if (!usedSidePositions[0] || y != (int) sidePositions[0].y)
                walls.Add(new Vector2(area.position.x, y));
            if (!usedSidePositions[2] || y != (int) sidePositions[2].y)
                walls.Add(new Vector2(area.position.x + area.width, y));
        }
        
        for (int x = (int) area.position.x + 1; x < area.position.x + area.width - 1; x++) {
            if (!usedSidePositions[1] || x != (int) sidePositions[1].x)
                walls.Add(new Vector2(x, area.position.y + area.height));
            if (!usedSidePositions[3] || x != (int) sidePositions[3].x)
                walls.Add(new Vector2(x, area.position.y));
        }

        return walls.Distinct().ToList();
    }

    public Vector2 GetRandPositionInRoom() {
        if (occupiedPositions.Count >= area.width * area.height) {
            Debug.LogError("No more empty space in this room!");
            return new Vector2(-1, -1);
        }
        
        Vector2 pos = new Vector2(Random.Range(area.xMin, area.xMax), Random.Range(area.yMin, area.yMax));
        if (occupiedPositions.Contains(pos))
            pos = GetRandPositionInRoom();

        return pos;
    }
}