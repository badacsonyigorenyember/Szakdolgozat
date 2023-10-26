using System.Collections.Generic;
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
    private List<Vector2> usedFloorPositions;
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
        usedFloorPositions = new List<Vector2>();
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
        angle = (angle + 360) % 360;

        int thisIndex = Mathf.FloorToInt(angle / 90) % 4;
        int otherIndex = (thisIndex + 2) % 4;

        Vector2 thisSidePosition = possibleEntrancePositions[thisIndex];
        Vector2 otherSidePosition = other.possibleEntrancePositions[otherIndex];

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

    public Vector2 GetRandPositionInRoom(bool toRemove) {
        if (usedFloorPositions.Count >= area.width ) {
            Debug.LogError("No more empty space in this room!");
            return new Vector2(-1, -1);
        }
        
        Vector2 pos = new Vector2(Random.Range(area.xMin + 1, area.xMax - 1), Random.Range(area.yMin + 1, area.yMax - 1));
        
        if (usedFloorPositions.Contains(pos))
            pos = GetRandPositionInRoom(toRemove);

        if(toRemove)
            usedFloorPositions.Add(pos);
        
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