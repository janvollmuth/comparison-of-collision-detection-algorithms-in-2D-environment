using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionBody
{
    public float x;
    public float y;
    public float radius;
    public float width;
    public float height;
    public int id;

    public CollisionBody(float x, float y, float radius, int id)
    {
        this.x = x;
        this.y = y;
        this.radius = radius;
        this.width = 0;
        this.height = 0;
        this.id = id;
    }

    public CollisionBody(float x, float y, float width, float height, int id)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.radius = 0;
        this.id = id;
    }

    public void ShowBoundries()
    {
        if(radius != 0)
        {
            float x = this.x;
            float y = this.y;

            Vector2 bottomLeftPoint = new Vector2(x - radius, y - radius);
            Vector2 bottomRightPoint = new Vector2(x + radius, y - radius);
            Vector2 topRightPoint = new Vector2(x + radius, y + radius);
            Vector2 topLeftPoint = new Vector2(x - radius, y + radius);

            Debug.DrawLine(bottomLeftPoint, bottomRightPoint, Color.blue, 0.01f);   //bottomLine
            Debug.DrawLine(bottomLeftPoint, topLeftPoint, Color.blue, 0.01f);       //leftLine
            Debug.DrawLine(bottomRightPoint, topRightPoint, Color.blue, 0.01f); //rightLine
            Debug.DrawLine(topLeftPoint, topRightPoint, Color.blue, 0.01f);     //topLine
        }
        else if(radius == 0)
        {
            float x = this.x;
            float y = this.y;

            Vector2 bottomLeftPoint = new Vector2(x - (width / 2), y - (height / 2));
            Vector2 bottomRightPoint = new Vector2(x + (width / 2), y - (height / 2));
            Vector2 topRightPoint = new Vector2(x + (width / 2), y + (height / 2));
            Vector2 topLeftPoint = new Vector2(x - (width / 2), y + (height / 2));

            Debug.DrawLine(bottomLeftPoint, bottomRightPoint, Color.blue, 0.01f);   //bottomLine
            Debug.DrawLine(bottomLeftPoint, topLeftPoint, Color.blue, 0.01f);       //leftLine
            Debug.DrawLine(bottomRightPoint, topRightPoint, Color.blue, 0.01f); //rightLine
            Debug.DrawLine(topLeftPoint, topRightPoint, Color.blue, 0.01f);     //topLine
        }

    
    }
}
