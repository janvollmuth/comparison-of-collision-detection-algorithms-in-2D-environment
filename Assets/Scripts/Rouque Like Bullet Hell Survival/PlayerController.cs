using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed;
    public CharacterController controller;
    public Camera Camera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 direction = new Vector2(horizontal, vertical);

        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.y); 

            controller.Move(direction * Speed * Time.deltaTime);
        }

        Camera.transform.SetPositionAndRotation(new Vector3(this.GetComponent<Transform>().transform.position.x, 
            this.GetComponent<Transform>().transform.position.y, -10f), new Quaternion(0, 0, 0, 0));
    }
}
