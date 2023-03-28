using UnityEngine;

public class Room
{
    public int width;
    public int height;
    public Vector2 position;
    public Vector2 placeHolder;
    

    public Room(int mapSize, int maxSize) {
        int distance = Random.Range(0, mapSize);
        float angle = Random.Range(0, 2 * Mathf.PI);

        position = new Vector2((int) (distance * Mathf.Cos(angle)), (int) (distance * Mathf.Sin(angle)));
        
        width = Random.Range(5, maxSize);
        height = Random.Range(5, maxSize);
        placeHolder = new Vector2(width + 2, height + 2);
        Debug.Log(position);
    }

    public bool Overlaps(Room room) {

        if (position.x + placeHolder.x < room.position.x || room.position.x + room.placeHolder.x < position.x)
            return false;
        if (position.y + placeHolder.y < room.position.y || room.position.y + room.placeHolder.y < position.y)
            return false;
        return true;
    }
    
}