using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerJnR : MonoBehaviour
{

    public float Gravity;
    public float Velocity;
    public GameObject Player;

    private CharacterController controller;


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 direction = new Vector2(0, 1);
            controller.Move(direction * Velocity);
        }
        else
        {
            Player.gameObject.transform.position = new Vector2(Player.transform.position.x, Player.transform.position.y + Gravity);
        }
    }
}
