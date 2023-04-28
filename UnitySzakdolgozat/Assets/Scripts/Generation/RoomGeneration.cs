
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomGeneration
{
    public static List<Room> CreateRooms(int roomCount, int mapSize, int maxRoomSize) {
        List<Room> rooms = new List<Room>();
        int attempts = 0;
        int actualRooms = 0;
        while (actualRooms < roomCount) {
            if (attempts == 5000) {
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
            
        MapGeneration.GenerateRooms(rooms, mapSize);

        //todo switch these two
        
        foreach (var room in rooms) {
            room.Position += new Vector2(mapSize, mapSize);
        }

        return rooms;

    }
}    


