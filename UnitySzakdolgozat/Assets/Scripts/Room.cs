using UnityEngine;

public class Room
{
    public Rect rect;
    

    public Room(Vector2Int pos, int maxSize) {
        rect.height = Random.Range(5, maxSize);
        rect.width = Random.Range(5, maxSize);
        rect.position = pos;
    }
    
}