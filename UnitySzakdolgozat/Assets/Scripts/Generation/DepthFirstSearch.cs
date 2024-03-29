using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public static class DepthFirstSearch
{
    
    public static Room FindNotArticularPoint(List<Room> rooms) {
        List<Room> articulationPoints = new List<Room>();

        Dictionary<Room, bool> visited = new Dictionary<Room, bool>();
        Dictionary<Room, int> discoveryTimes = new Dictionary<Room, int>();
        Dictionary<Room, int> lowTimes = new Dictionary<Room, int>();

        int time = 0;
        foreach (Room room in rooms)
        {
            if (!visited.ContainsKey(room))
            {
                DoDepthFirstSearch(room, null, visited, discoveryTimes, lowTimes, articulationPoints, time);
            }
        }

        List<Room> notArticularRooms = rooms.Where(r => !articulationPoints.Contains(r)).ToList();

        if (notArticularRooms.Count == 0)
            return null;

        return notArticularRooms[Random.Range(0, notArticularRooms.Count)];
    }

    private static void DoDepthFirstSearch(Room room, Room parent, Dictionary<Room, bool> visited, Dictionary<Room, int> discoveryTimes, Dictionary<Room, int> lowTimes, List<Room> articulationPoints, int time)
    {
        visited[room] = true;
        discoveryTimes[room] = time;
        lowTimes[room] = time;
        time++;

        int childCount = 0;
        bool isArticulationPoint = false;

        foreach (Room neighbor in room.neighbours)
        {
            if (!visited.ContainsKey(neighbor))
            {
                DoDepthFirstSearch(neighbor, room, visited, discoveryTimes, lowTimes, articulationPoints, time);

                childCount++;

                if (lowTimes[neighbor] >= discoveryTimes[room])
                {
                    isArticulationPoint = true;
                }

                lowTimes[room] = Math.Min(lowTimes[room], lowTimes[neighbor]);
            }
            else if (neighbor != parent)
            {
                lowTimes[room] = Math.Min(lowTimes[room], discoveryTimes[neighbor]);
            }
        }

        if ((parent == null && childCount > 1) || (parent != null && isArticulationPoint))
        {
            articulationPoints.Add(room);
        }
    }
    
}
