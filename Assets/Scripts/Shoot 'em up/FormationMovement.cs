using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationMovement : MonoBehaviour
{
    public float Speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - (Speed * Time.deltaTime), transform.position.z);
    }
}
