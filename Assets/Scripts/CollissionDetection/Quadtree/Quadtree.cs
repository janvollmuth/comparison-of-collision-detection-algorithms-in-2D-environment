using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quadtree
{
    public RectangleQT boundry;
    private CollisionBody node;
    private bool divided = false;
    private bool nodeInserted = false;
    private Quadtree northEast, northWest, southEast, southWest;

    public Quadtree(RectangleQT boundry)
    {
        this.boundry = boundry;
    }

    //Clear all nodes from the QuadTree
    public void ClearQuadTree()
    {
        if (!nodeInserted)
        {
            return;
        }
        nodeInserted = false;

        if (divided)
        {
            northEast.ClearQuadTree();
            northWest.ClearQuadTree();
            southEast.ClearQuadTree();
            southWest.ClearQuadTree();
        }
        divided = false;
    }

    #region Point

    public bool Insert(float x, float y, float radius, int id)
    {
        //Checking if the position is in the boundries of the node.
        if (!boundry.ContainsObject(x, y))
        {
            return false;
        }

        if (!nodeInserted)
        {
            this.node = new CollisionBody(x, y, radius, id);
            Stats.Instance.AddQuadTreeObjects();
            this.node.ShowBoundries();
            nodeInserted = true;
            return true;
        }
        else
        {
            if (!divided)
            {
                SubDivide();
            }

            if (northEast.Insert(x, y, radius, id)) return true;
            if (northWest.Insert(x, y, radius, id)) return true;
            if (southEast.Insert(x, y, radius, id)) return true;
            if (southWest.Insert(x, y, radius, id)) return true;
        }
        return false;
    }

    //Check Node
    public int Query(float x, float y, float radius, int id)
    {
        if (!nodeInserted)
        {
            return -1;
        }

        if (!boundry.ContainsObject(x, y))
        {
            return -1;
        }

        if (node.id == id)
        {
            return -1;
        }

        //Add collision check for benchmark
        Stats.Instance.AddCollisionCheck();

        Debug.DrawLine(new Vector2(x, y), new Vector2(node.x, node.y), Color.yellow, 0.01f);

        if (Overlap(node.x, node.y, x, y, radius))
        {
            return node.id;
        }

        if (divided)
        {
            return QueryLeafs(x, y, radius, id);
        }
        return -1;
    }

    //Check Children
    public int QueryLeafs(float x, float y, float radius, int id)
    {
        int result = -1;

        result = northEast.Query(x, y, radius, id);
        if(result != -1)
        {
            return result;
        }

        result = northWest.Query(x, y, radius, id);
        if (result != -1)
        {
            return result;
        }

        result = southEast.Query(x, y, radius, id);
        if (result != -1)
        {
            return result;
        }

        result = southWest.Query(x, y, radius, id);
        if (result != -1)
        {
            return result;
        }
        return -1;
    }

    private void SubDivide()
    {
        if(northEast == null)
        {
            float height = boundry.height / 2;
            float width = boundry.width / 2;
            float x = boundry.x;
            float y = boundry.y;

            RectangleQT ne = new RectangleQT(x + width, y + height, width, height);
            RectangleQT nw = new RectangleQT(x - width, y + height, width, height);
            RectangleQT se = new RectangleQT(x + width, y - height, width, height);
            RectangleQT sw = new RectangleQT(x - width, y - height, width, height);

            northEast = new Quadtree(ne);
            northWest = new Quadtree(nw);
            southEast = new Quadtree(se);
            southWest = new Quadtree(sw);
        }
        divided = true;
    }

    public bool Overlap(float object1_x, float object1_y, float object2_x, float object2_y, float radius)
    {
        if (object1_x - radius > object2_x + radius || object2_x - radius > object1_x + radius)
        {
            return false;
        }

        if(object1_y + radius < object2_y - radius || object2_y + radius < object1_y - radius)
        {
            return false;
        }

        return true;
    }

    public void ShowBoundries(Quadtree tree)
    {
        Quadtree qt = tree;

        float h = qt.boundry.height;
        float w = qt.boundry.width;
        float x = qt.boundry.x;
        float y = qt.boundry.y;

        Vector2 bottomLeftPoint = new Vector2(x - w, y - h);
        Vector2 bottomRightPoint = new Vector2(x + w, y - h);
        Vector2 topRightPoint = new Vector2(x + w, y + h);
        Vector2 topLeftPoint = new Vector2(x - w, y + h);

        Debug.DrawLine(bottomLeftPoint, bottomRightPoint, Color.red, 0.01f);   //bottomLine
        Debug.DrawLine(bottomLeftPoint, topLeftPoint, Color.red, 0.01f);       //leftLine
        Debug.DrawLine(bottomRightPoint, topRightPoint, Color.red, 0.01f); //rightLine
        Debug.DrawLine(topLeftPoint, topRightPoint, Color.red, 0.01f);     //topLine

        if (divided)
        {
            qt.northEast.ShowBoundries(qt.northEast);
            qt.northWest.ShowBoundries(qt.northWest);
            qt.southEast.ShowBoundries(qt.southEast);
            qt.southWest.ShowBoundries(qt.southWest);
        }
    }

    //Collision Point and Point
    public int QuadtreeCollision(Quadtree qt, float x, float y, float radius, int id)
    {
        int foundCollisionBody = qt.Query(x, y, radius, id);

        if (foundCollisionBody != -1)
        {
            //Debug.Log("Collision Object " + id + " & Object " + foundCollisionBody);

            return foundCollisionBody;
        }
        else
        {
            return -1;
        }
    }

    #endregion

    #region Rectangle

    public bool InsertRect(float x, float y, float width, float height, int id)
    {
        //Checking if the position is in the boundries of the node.
        if (!boundry.ContainsRect(x, y, width, height))
        {
            return false;
        }

        if (!nodeInserted)
        {
            this.node = new CollisionBody(x, y, width, height, id);
            this.node.ShowBoundries();
            nodeInserted = true;
            return true;
        }
        else
        {
            if (!divided)
            {
                SubDivide();
            }

            if (northEast.InsertRect(x, y, width, height, id)) return true;
            if (northWest.InsertRect(x, y, width, height, id)) return true;
            if (southEast.InsertRect(x, y, width, height, id)) return true;
            if (southWest.InsertRect(x, y, width, height, id)) return true;
        }
        return false;
    }

    public int QuadtreeCollisionRec(Quadtree qt, float x, float y, float width, float height, int id)
    {
        int foundCollisionBody = qt.QueryRec(x, y, width, height, id);

        if (foundCollisionBody != -1)
        {
            return foundCollisionBody;
        }
        else
        {
            return -1;
        }
    }

    public int QueryRec(float x, float y, float width, float height, int id)
    {
        if (!nodeInserted)
        {
            return -1;
        }

        if (!boundry.ContainsRect(x, y, width, height))
        {
            return -1;
        }

        if (node.id == id)
        {
            return -1;
        }

        if (node.x == x)
        {
            return -1;
        }

        //Add collision check for benchmark
        Stats.Instance.AddCollisionCheck();

        Debug.DrawLine(new Vector2(x, y), new Vector2(node.x, node.y), Color.yellow, 0.01f);

        if (OverlapRec(node.x, node.y, node.width, node.height, x, y, width, height))
        {  
            return node.id;
        }

        if (divided)
        {
            return QueryLeafsRec(x, y, width, height, id);
        }
        return -1;
    }

    public int QueryLeafsRec(float x, float y, float width, float height, int id)
    {
        int result = -1;

        result = northEast.QueryRec(x, y, width, height, id);
        if (result != -1)
        {
            return result;
        }

        result = northWest.QueryRec(x, y, width, height, id);
        if (result != -1)
        {
            return result;
        }

        result = southEast.QueryRec(x, y, width, height, id);
        if (result != -1)
        {
            return result;
        }

        result = southWest.QueryRec(x, y, width, height, id);
        if (result != -1)
        {
            return result;
        }
        return -1;
    }

    public bool OverlapRec(float object1_x, float object1_y, float width1, float height1, float object2_x, float object2_y, float width2, float height2)
    {
        //Point vs Rectangle
        if(width1 == 0)
        {
            if (object1_x - 0.5f > object2_x + (width2 / 2) || object2_x - (width2 / 2) > object1_x + 0.5f)
            {
                return false;
            }

            if (object1_y + 0.5f < object2_y - (height2 / 2) || object2_y + (height2 / 2) < object1_y - 0.5f)
            {
                return false;
            }
            return true;
        }

        //Rectangle vs Point
        if (width2 == 0)
        {
            if (object1_x - (width1 / 2) > object2_x + 0.5f || object2_x - 0.5f > object1_x + (width1 / 2))
            {
                return false;
            }

            if (object1_y + (height1 / 2) < object2_y - 0.5f || object2_y + 0.5f < object1_y - (height1 / 2))
            {
                return false;
            }
            return true;
        }

        if (object1_x - (width1 / 2) > object2_x + (width2 / 2) || object2_x - (width2 / 2) > object1_x + (width1 / 2))
        {
            return false;
        }

        if (object1_y + (height1 / 2) < object2_y - (height2 / 2) || object2_y + (height2 / 2) < object1_y - (height1 / 2))
        {
            return false;
        }

        return true;
    }

    #endregion
}
