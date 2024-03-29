
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RoomGeneration
{
    public static (List<Room>, int) CreateRooms(int roomCount, int mapSize, int maxRoomSize) {
        List<Room> rooms = new List<Room>();
        int attempts = 0;
        int actualRooms = 0;
        while (actualRooms < roomCount) {
            if (attempts >= 5000) {
                mapSize += mapSize / 2;
                attempts = 0;
            }
            
            Room room = new Room(mapSize, maxRoomSize);

            if (!rooms.Any(r => r.Overlaps(room))) {
                rooms.Add(room);
                actualRooms++;
                attempts = 0;
            }
            else
                attempts++;

        }
        
        foreach (var room in rooms) {
            room.area.setPosition(room.area.position + new Vector2(mapSize, mapSize));

        }
        
        return (rooms, mapSize);

    }
}    


