using System;
using UnityEngine;

public class Rect
{
    public int width;
    public int height;
    public Vector2 position;
    public Vector2 center;
    public int xMin;
    public int xMax;
    public int yMin;
    public int yMax;

    public Rect(Vector2 position, int width, int height) {
        this.position = position;
        this.width = width;
        this.height = height;
        this.center = new Vector2((int) (position.x + width / 2), (int) (position.y + height / 2));
        this.xMin = (int) position.x;
        this.xMax = (int) position.x + width;
        this.yMin = (int) position.y;
        this.yMax = (int) position.y + height;
    }

    public void setPosition(Vector2 pos) {
        this.position = pos;
        this.center = new Vector2((int) (position.x + width / 2), (int) (position.y + height / 2));
        xMin = (int) pos.x;
        xMax = (int) pos.x + width;
        yMin = (int) pos.y;
        yMax = (int) pos.y + height;
    }

    public bool Overlaps(Rect other) {
        float left = Math.Max(position.x, other.position.x);
        float right = Math.Min(position.x + width, other.position.x + other.width);

        if (left < right)
        {
            float lower = Math.Max(position.y, other.position.y);
            float upper = Math.Min(position.y + height, other.position.y + other.height);

            if (lower < upper)
            {
                return true;
            }
        }

        return false;
    }
}
