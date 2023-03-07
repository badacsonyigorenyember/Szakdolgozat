using UnityEngine;

public class Room
{
    public Rect rect;
    

    public Room(Vector2Int pos, int maxSize) {
        rect.position = pos;
        rect.height = Random.Range(3, maxSize);
        rect.width = Random.Range(3, maxSize);
        Debug.Log(rect.width + "\n" + rect.height);
    }
    
    

    
}