using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleQT
{
    public float x;
    public float y;
    public float width;
    public float height;

    public RectangleQT(float x, float y, float width, float height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    public bool ContainsObject(float positionX, float positionY)
    {
        if (positionX >= x - width && positionX <= x + width &&
            positionY >= y - height && positionY <= y + height)
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

        if (topleft.x >= x - width && topleft.x <= x + width &&
            topleft.y >= y - height && topleft.y <= y + height)
        {
            return true;
        }
        else if(topright.x >= x - width && topright.x <= x + width &&
            topright.y >= y - height && topright.y <= y + height)
        {
            return true;
        }
        else if (botleft.x >= x - width && botleft.x <= x + width &&
            botleft.y >= y - height && botleft.y <= y + height)
        {
            return true;
        }
        else if (botright.x >= x - width && botright.x <= x + width &&
            botright.y >= y - height && botright.y <= y + height)
        {
            return true;
        }
        return false;
    }
}
