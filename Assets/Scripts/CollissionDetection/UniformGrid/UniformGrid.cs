using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniformGrid : MonoBehaviour 
{
    public RectangleUG[,] Grid;

    public int rows;
    public int columns;
    public int recSize;

    public Transform Player;

    private float gridSizeX;
    private float gridSizeY;

    private ArrayList objectData;

    private void Start()
    {
        objectData = new ArrayList();
    }

    public void GenerateGrid()
    {
        Grid = new RectangleUG[rows, columns];

        gridSizeX = columns * recSize;
        gridSizeY = rows * recSize;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                float posX = (Player.position.x + column * recSize) - (gridSizeX / 2);
                float posY = (Player.position.y + row * recSize) - (gridSizeY / 2);

                RectangleUG rec = new RectangleUG(posX, posY, recSize, recSize);
                Grid[row, column] = rec;
            }
        }
        ShowBoundries();
    }

    public void ClearGrid()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Grid[row, column].bodyIDs = new ArrayList();
            }
        }
    }

    #region Point

    public void Insert(float x, float y, float radius, int id)
    {
        for(int row = 0; row < rows; row++)
        {
            for(int column = 0; column < columns; column++)
            {
                if (Grid[row, column].ContainsObject(x,y))
                {
                    //Debug.Log("Contains Object in: " + Grid[row,column].x + " / " + Grid[row,column].y);
                    Grid[row, column].AddObject(id);
                    CollisionBody body = new CollisionBody(x, y, radius, id);
                    body.ShowBoundries();
                    Stats.Instance.AddUniformGridObjects();
                }
            }
        }

    }

    public int CheckCollisionUniformGrid(ArrayList objectData, float objectX, float objectY, int id, float radius)
    {
        int selectedRow = 0;
        int selectedColumn = 0;
        this.objectData = objectData;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (Grid[row, column].ContainsObject(objectX, objectY))
                {
                    selectedRow = row;
                    selectedColumn = column;
                }
            }
        }

        for(int i = 0; i < Grid[selectedRow,selectedColumn].bodyIDs.Count; i++)
        {
            int objectID = Grid[selectedRow, selectedColumn].GetIDFromList(i);

            if(id == objectID)
            {
                return -1;
            }

            if (!Grid[selectedRow, selectedColumn].ContainsObject(objectX, objectY))
            {
                return -1;
            }

            //Debug.Log("Compare: " + objectID + " with " + objectX + " / " + objectY + " / ObjectCount: " + Grid[selectedRow, selectedColumn].bodyIDs.Count + " / index: " + i);
            Stats.Instance.AddCollisionCheck();
            bool result = CompareObjects(objectID, objectX, objectY, radius);

            //Debug.Log(objectID + " / " + id);

            if (result)
            {
                return objectID;
            }         
        }
        return -1;
    }

    public bool CompareObjects(int targetID, float x, float y, float radius)
    {
        GameObject target = (GameObject)objectData[targetID];

        Debug.DrawLine(new Vector2(x,y), new Vector2(target.transform.position.x, target.transform.position.y), Color.yellow, 0.01f);

        if (Overlap(target.transform.position.x, target.transform.position.y, x, y, radius))
        {
            //Debug.Log("Overlapped");
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool Overlap(float object1_x, float object1_y, float object2_x, float object2_y, float radius)
    {
        if (object1_x - radius > object2_x + radius || object2_x - radius > object1_x + radius)
        {
            return false;
        }

        if (object1_y + radius < object2_y - radius || object2_y + radius < object1_y - radius)
        {
            return false;
        }

        return true;
    }

    #endregion

    #region Rectangle

    public void InsertRect(float x, float y, float width, float height, int id)
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (Grid[row, column].ContainsRect(x, y, width, height))
                {
                    Grid[row, column].bodyIDs.Add(id);
                    CollisionBody body = new CollisionBody(x, y, width, height, id);
                    body.ShowBoundries();
                    Stats.Instance.AddUniformGridObjects();
                }
            }
        }
    }

    public int CheckCollisionUniformGridRect(ArrayList objectData, float objectX, float objectY, int id, float width, float height)
    {
        List<Vector2> gridObjects = new List<Vector2>();
        this.objectData = objectData;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (Grid[row, column].ContainsRect(objectX, objectY, width, height))
                {
                    Vector2 gridObject = new Vector2(row, column);
                    gridObjects.Add(gridObject);
                }
            }
        }

        for (int i = 0; i < gridObjects.Count; i++)
        {
            int selectedRow = (int)gridObjects[i].x;
            int selectedColumn = (int)gridObjects[i].y;

            ArrayList idList = new ArrayList();

            for(int k = 0; k < Grid[selectedRow, selectedColumn].bodyIDs.Count; k++)
            {
                idList.Add(Grid[selectedRow, selectedColumn].bodyIDs[k]);
            }

            for(int t = 0; t < idList.Count; t++)
            {
                int objectID = (int)idList[t];
                if (objectID != id)
                {
                    if (CompareObjectsRect(objectID, objectX, objectY, width, height))
                    {
                        return id;
                    }
                }
            }
        }
        return -1;
    }

    public bool CompareObjectsRect(int targetID, float x, float y, float width, float height)
    {
        GameObject target = (GameObject)objectData[targetID];

        Debug.DrawLine(new Vector2(x, y), new Vector2(target.transform.position.x, target.transform.position.y), Color.yellow, 0.01f);
        Stats.Instance.AddCollisionCheck();
        if (OverlapRect(target.transform.position.x, target.transform.position.y, target.transform.localScale.x, target.transform.localScale.y, x, y, width, height))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool OverlapRect(float object1_x, float object1_y, float width1, float height1, float object2_x, float object2_y, float width2, float height2)
    {
        //Point vs Rectangle
        if (width1 == 0)
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

    public void ShowBoundries()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                RectangleUG rec = Grid[row, column];

                float width = rec.width;
                float height = rec.height;
                float x = rec.x;
                float y = rec.y;

                Vector2 bottomLeftPoint = new Vector2(x, y);
                Vector2 bottomRightPoint = new Vector2(x + width, y);

                Vector2 topRightPoint = new Vector2(x + width, y + height);
                Vector2 topLeftPoint = new Vector2(x, y + height);

                Debug.DrawLine(bottomLeftPoint, bottomRightPoint, Color.red, 0.01f);   //bottomLine
                Debug.DrawLine(bottomLeftPoint, topLeftPoint, Color.red, 0.01f);       //leftLine
                Debug.DrawLine(bottomRightPoint, topRightPoint, Color.red, 0.01f); //rightLine
                Debug.DrawLine(topLeftPoint, topRightPoint, Color.red, 0.01f);     //topLine
            }
        }
    }
}
