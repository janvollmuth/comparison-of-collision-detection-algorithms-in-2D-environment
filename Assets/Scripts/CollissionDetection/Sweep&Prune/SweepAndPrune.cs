using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweepAndPrune
{
    public List<Bound> xAxis;

    public SweepAndPrune()
    {
        xAxis = new List<Bound>();
    }

    public List<Bound> UpdateSAP(ArrayList objects)
    {
        xAxis.Clear();


        for(int i = 0; i < objects.Count; i++)
        {
            GameObject gameObject = (GameObject)objects[i];

            if(gameObject != null)
            {
                //lower bound
                float lowerValue = gameObject.transform.position.x - (gameObject.transform.localScale.x / 2);
                int id = gameObject.name == "Player" ? 0 : i;
                int upperOrLower = 0;
                if (gameObject.GetComponent<EnemyController>() != null)
                {
                    gameObject.GetComponent<EnemyController>().id = id;
                }
                Bound lower = new Bound(lowerValue, id, upperOrLower);
                ShowBoundries(lowerValue, gameObject.transform.position.y);

                //upperBound
                float upperValue = gameObject.transform.position.x + (gameObject.transform.localScale.x / 2);
                id = gameObject.name == "Player" ? 0 : i;
                upperOrLower = 1;
                Bound upper = new Bound(upperValue, id, upperOrLower);
                ShowBoundries(upperValue, gameObject.transform.position.y);

                //add bounds
                xAxis.Add(lower);
                xAxis.Add(upper);
            }
        }

        //sort list
        SortList();

        return xAxis;
    }

    public List<Bound> UpdateSAPRect(ArrayList objects)
    {
        xAxis.Clear();


        for (int i = 0; i < objects.Count; i++)
        {
            GameObject gameObject = (GameObject)objects[i];

            int id = i;

            //lower bound
            float lowerValue = gameObject.transform.position.x - (gameObject.transform.localScale.x/2);
            int upperorlower = 0;
            ShowBoundries(lowerValue, gameObject.transform.position.y);
            Bound lower = new Bound(lowerValue, id, upperorlower);

            //upperBound
            float upperValue = gameObject.transform.position.x + (gameObject.transform.localScale.x / 2);
            upperorlower = 1;
            ShowBoundries(upperValue, gameObject.transform.position.y);
            Bound upper = new Bound(upperValue, id, upperorlower);

            //add bounds
            xAxis.Add(lower);
            xAxis.Add(upper);
        }

        //sort list
        SortList();

        return xAxis;
    }

    private void SortList()
    {
        for(int i = 1; i < xAxis.Count; i++)
        {
            int j = i;
            while(j > 0)
            {
                if(xAxis[j].value < xAxis[j - 1].value)
                {
                    Bound temp = xAxis[j-1];
                    xAxis[j-1] = xAxis[j];
                    xAxis[j] = temp;
                    j--;
                }
                else
                {
                    break;
                }
            }
        }
    }

    #region Point

    public bool CheckCollisionSAP(ArrayList objectData, int object1, int object2, float radius)
    {
        GameObject target = (GameObject)objectData[object1];
        GameObject target2 = (GameObject)objectData[object2];

        Debug.DrawLine(new Vector2(target2.transform.position.x, target2.transform.position.y), new Vector2(target.transform.position.x, target.transform.position.y), Color.yellow, 0.01f);

        if (Overlap(target.transform.position.x, target.transform.position.y, target2.transform.position.x, target2.transform.position.y, radius))
        {
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

    public bool CheckCollisionSAPRect(ArrayList objectData, int object1, int object2)
    {
        GameObject target = (GameObject)objectData[object1];
        GameObject target2 = (GameObject)objectData[object2];

        Debug.DrawLine(new Vector2(target2.transform.position.x, target2.transform.position.y), new Vector2(target.transform.position.x, target.transform.position.y), Color.yellow, 0.01f);

        if (OverlapRect(target.transform.position.x, target.transform.position.y, target.transform.localScale.x, target.transform.localScale.y, target2.transform.position.x, target2.transform.position.y, target2.transform.localScale.x, target2.transform.localScale.y))
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

    private void ShowBoundries(float x, float y)
    {
        Debug.DrawLine(new Vector2(x, y), new Vector2(x, 0), Color.red, 0.01f);
    }
}
