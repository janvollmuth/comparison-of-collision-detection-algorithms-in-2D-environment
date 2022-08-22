using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruteForce
{
    private ArrayList objects;

   public BruteForce()
    {
        objects = new ArrayList();
    }

    public void CheckCollisionBruteForce(ArrayList list)
    {
        this.objects = list;

        for (int i = 0; i < objects.Count; i++)
        {
            for (int j = 0; j < objects.Count; j++)
            {
                if(i != j)
                {
                    GameObject object1 = (GameObject)objects[i];
                    GameObject object2 = (GameObject)objects[j];

                    Debug.DrawLine(new Vector3(object1.transform.position.x, object1.transform.position.y), new Vector3(object2.transform.position.x, object2.transform.position.y), Color.yellow);
                    Stats.Instance.AddCollisionCheck();

                    if (Overlap(object1.transform.position.x, object1.transform.position.y, object2.transform.position.x, object2.transform.position.y, 0.5f))
                    {
                        HandleCollision(i, j);
                    }
                }                
            }
        }
    }

    private void HandleCollision(int id1, int id2)
    {
        
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
}
