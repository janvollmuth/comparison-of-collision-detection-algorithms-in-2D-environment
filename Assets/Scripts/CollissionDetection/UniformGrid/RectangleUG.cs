using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleUG
{
    public float x;
    public float y;
    public int width;
    public int height;
    public ArrayList bodyIDs;

    public RectangleUG(float x, float y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.bodyIDs = new ArrayList();
    }

    public bool ContainsObject(float positionX, float positionY)
    {
        if (positionX >= x && positionX <= x + width &&
            positionY >= y && positionY <= y + height)
        {
            return true;
        }
        return false;
    }

    public bool ContainsRect(float positionX, float positionY, float widthTarget, float heightTarget)
    {
        Vector2 topleft;
        Vector2 topright;
        Vector2 botleft;
        Vector2 botright;

        //Player
        if (widthTarget == 0)
        {
            topleft = new Vector2(positionX - 0.5f, positionY + 0.5f);
            topright = new Vector2(positionX + 0.5f, positionY + 0.5f);
            botleft = new Vector2(positionX - 0.5f, positionY - 0.5f);
            botright = new Vector2(positionX + 0.5f, positionY - 0.5f);
        }
        else
        {
            topleft = new Vector2(positionX - (widthTarget / 2), positionY + (heightTarget / 2));
            topright = new Vector2(positionX + (widthTarget / 2), positionY + (heightTarget / 2));
            botleft = new Vector2(positionX - (widthTarget / 2), positionY - (heightTarget / 2));
            botright = new Vector2(positionX + (widthTarget / 2), positionY - (heightTarget / 2));
        }

        //TopLeft Point
        if (topleft.x >= x && topleft.x <= x + width && topleft.y >= y && topleft.y <= y + height)
        {
            Debug.DrawLine(new Vector3(x, y, 0), new Vector3(x + width, y + height), Color.green);
            return true;
        }
        //TopRight Point
        else if (topright.x >= x && topright.x <= x + width && topright.y >= y && topright.y <= y + height)
        {
            Debug.DrawLine(new Vector3(x, y, 0), new Vector3(x + width, y + height), Color.green);
            return true;
        }
        //BotLeft Point
        else if (botleft.x >= x && botleft.x <= x + width && botleft.y >= y && botleft.y <= y + height)
        {
            Debug.DrawLine(new Vector3(x, y, 0), new Vector3(x + width, y + height), Color.green);
            return true;
        }
        //BotRight Point
        else if (botright.x >= x && botright.x <= x + width && botright.y >= y && botright.y <= y + height)
        {
            Debug.DrawLine(new Vector3(x, y, 0), new Vector3(x + width, y + height), Color.green);
            return true;
        }
        //Top Line
        else if (x >= topleft.x && x + width <= topright.x && topleft.y <= y + height && topleft.y >= y)
        {
            Debug.DrawLine(new Vector3(x, y, 0), new Vector3(x + width, y + height), Color.green);
            return true;
        }
        //Bottom Line
        else if (x >= topleft.x && x + width <= topright.x && botleft.y >= y && botleft.y <= y + height)
        {
            Debug.DrawLine(new Vector3(x, y, 0), new Vector3(x + width, y + height), Color.green);
            return true;
        }
        //Left Line
        else if(topleft.y >= y + height && botleft.y <= y && topleft.x >= x && topleft.x <= x + width)
        {
            Debug.DrawLine(new Vector3(x, y, 0), new Vector3(x + width, y + height), Color.green);
            return true;
        }
        //Right Line
        else if (topleft.y >= y + height && botleft.y <= y && topright.x >= x && topright.x <= x + width)
        {
            Debug.DrawLine(new Vector3(x, y, 0), new Vector3(x + width, y + height), Color.green);
            return true;
        }

        return false;
    }

public void AddObject(int id)
    {
        if (bodyIDs.Contains(id))
        {
            return;
        }

        this.bodyIDs.Add(id);
    }

    public void ClearObjects()
    {
        this.bodyIDs.Clear();
    }

    public int GetIDFromList(int index)
    {
        return (int) bodyIDs[index];
    }
}
