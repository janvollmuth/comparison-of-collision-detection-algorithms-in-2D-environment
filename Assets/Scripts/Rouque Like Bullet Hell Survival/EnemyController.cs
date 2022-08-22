using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float Speed;
    public float Distance;
    public int id;

    private Transform player;
    private bool update;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        update = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (update)
        {
            if (Vector2.Distance(transform.position, player.position) >= Distance)
            {
                transform.position = Vector2.MoveTowards(transform.position, player.position, Speed * Time.deltaTime);
            }
        }
    }

    public void SetUpdate(bool var)
    {
        if (var)
        {
            update = true;
        }
        else
        {
            update = false;
        }
    }
}
