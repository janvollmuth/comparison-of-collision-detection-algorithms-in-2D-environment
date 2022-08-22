using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeMovement : MonoBehaviour
{
    public float Speed;
    public GameObject PipeTop;
    public GameObject PipeBottom;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.left * Speed * Time.deltaTime;

        if(this.transform.position.x < -20)
        {
            this.gameObject.transform.position = new Vector3 (20, (int)Random.Range(-1, 5), gameObject.transform.position.z);
        }
    }
}
