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
    public HashSet<Vector2> entrancePositions;
    public Vector2[] possibleEntrancePositions;
    public List<Room> neighbours;
    public bool locked;
    public List<Door> doors;
    private List<Vector2> occupiedPositions;
    private List<Vector2> wallPositions;


    public Room(int mapSize, int maxSize) {
        int distance = Random.Range(0, mapSize - maxSize - 1);
        float angle = Random.Range(0, 2 * Mathf.PI);

        Vector2 areaPos = new Vector2((int) (distance * Mathf.Cos(angle)), (int) (distance * Mathf.Sin(angle)));
        
        area = new Rect(areaPos, Random.Range(5, maxSize), Random.Range(5, maxSize));

        placeHolder = new Rect(area.position - Vector2.one * 2, area.width + 4, area.height + 4);
        
        neighbours = new List<Room>();
        locked = false;
        doors = new List<Door>();
        occupiedPositions = new List<Vector2>();
        entrancePositions = new HashSet<Vector2>();
        wallPositions = new List<Vector2>();
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
        
        possibleEntrancePositions = new Vector2[]
        {
            new (area.position.x + area.width, area.center.y + Random.Range(-(area.height / 4), area.height / 4)), //right
            new (area.center.x + Random.Range(-(area.width / 4), area.width / 4), area.position.y + area.height), //top
            new (area.position.x, area.center.y + Random.Range(-(area.height / 4), area.height / 4)), // left
            new (area.center.x + Random.Range(-(area.width / 4), area.width / 4), area.position.y) // bottom
        };
        
        GetWallPositions();

    }

    public (Vector2, Vector2) SelectEndPoints(Room other) {
        float angle = Mathf.Atan2(other.area.center.y - area.center.y, other.area.center.x - area.center.x) * Mathf.Rad2Deg + 45;
        angle = angle < 0 ? angle + 360 : angle;
        angle %= 360;
        Vector2 thisSidePosition = possibleEntrancePositions[0];
        Vector2 otherSidePosition = other.possibleEntrancePositions[0];
        
        
        switch (angle) {
            case var n when (n > 0 && n <= 90):
                thisSidePosition = possibleEntrancePositions[0];
                otherSidePosition = other.possibleEntrancePositions[2];
                break;
            
            case var n when (n > 90 && n <= 180):
                thisSidePosition = possibleEntrancePositions[1];
                otherSidePosition = other.possibleEntrancePositions[3];
                break;
            
            case var n when (n > 180 && n <= 270):
                thisSidePosition = possibleEntrancePositions[2];
                otherSidePosition = other.possibleEntrancePositions[0];
                break;
            
            case var n when (n > 270 && n <= 360):
                thisSidePosition = possibleEntrancePositions[3];
                otherSidePosition = other.possibleEntrancePositions[1];
                break;
        }

        entrancePositions.Add(thisSidePosition);
        wallPositions.Remove(thisSidePosition);

        other.entrancePositions.Add(otherSidePosition);
        other.wallPositions.Remove(otherSidePosition);

        return (thisSidePosition, otherSidePosition);
    }

    private void GetWallPositions() {
        for (int y = area.yMin + 1; y < area.yMax; y++) {
            wallPositions.Add(new Vector2(area.xMin, y));
            wallPositions.Add(new Vector2(area.xMax, y));
        }
        
        for(int x = area.xMin + 1; x < area.xMax - 1; x++ ) {
            wallPositions.Add(new Vector2(x, area.yMin));
            wallPositions.Add(new Vector2(x, area.yMax));
        }
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

    public Vector2 GetRandomWallPosition() {
        int rand = Random.Range(0, wallPositions.Count);
        if (wallPositions.Count == 0) return new Vector2(-1, -1);
        
        Vector2 pos = wallPositions[rand];
        wallPositions.RemoveAt(rand);

        return pos;
    }
}