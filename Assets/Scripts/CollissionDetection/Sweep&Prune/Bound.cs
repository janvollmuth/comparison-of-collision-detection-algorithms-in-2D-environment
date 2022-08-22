using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bound
{
    public float value;
    public int id;
    public int upperOrLower;   //0 for lower bound and 1 for upper bound

    public Bound(float axisValue, int id, int upperOrLower)
    {
        this.value = axisValue;
        this.id = id;
        this.upperOrLower = upperOrLower;
    }
}
