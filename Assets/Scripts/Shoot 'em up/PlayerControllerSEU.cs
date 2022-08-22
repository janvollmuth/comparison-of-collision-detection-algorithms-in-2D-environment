using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerSEU : MonoBehaviour
{
    public float Speed;
    public int TimeBetweenBullets;
    public GameObject Bullet;
    public ArrayList BulletList;
    private CharacterController controller;
    private bool bulletSpawn;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        bulletSpawn = true;
        BulletList = new ArrayList();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 direction = new Vector2(horizontal, vertical);

        if (direction.magnitude >= 0.1f && transform.position.x >= -11f && transform.position.x <= 11f && transform.position.y <= 7f && transform.position.y >= -5f)
        {
            controller.Move(direction * Speed * Time.deltaTime);
        }
        
        if(direction.magnitude >= 0.1f && transform.position.x < -11f)
        {
            if(horizontal > 0)
            {
                controller.Move(direction * Speed * Time.deltaTime);
            }
        }
        
        if(direction.magnitude >= 0.1f && transform.position.x > 11f)
        {
            if (horizontal < 0)
            {
                controller.Move(direction * Speed * Time.deltaTime);
            }
        }
        
        if (direction.magnitude >= 0.1f && transform.position.y < -5f)
        {
            if (vertical > 0)
            {
                controller.Move(direction * Speed * Time.deltaTime);
            }
        }
        
        if(direction.magnitude >= 0.1f && transform.position.y > 7f)
        {
            if (vertical < 0)
            {
                controller.Move(direction * Speed * Time.deltaTime);
            }
        }

        if (bulletSpawn)
        {
            StartCoroutine(bulletSpawner());
        }

        CheckForBulletsOutOfBounds();
    }

    private void CheckForBulletsOutOfBounds()
    {
        if(BulletList.Count > 0)
        {
            for(int i = 0; i < BulletList.Count; i++)
            {
                GameObject gameObject = (GameObject)BulletList[i];

                if(gameObject != null && gameObject.transform.position.y > 10)
                {
                    BulletList.Remove(gameObject);
                    Destroy(gameObject);
                }
            }
        }
    }

    IEnumerator bulletSpawner()
    {
        bulletSpawn = false;

        GameObject bulletClone = Instantiate(Bullet);
        bulletClone.transform.SetPositionAndRotation(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), new Quaternion(0, 0, 0, 0));
        bulletClone.GetComponent<BulletMovement>().id = BulletList.Count;
        BulletList.Add(bulletClone);
        Stats.Instance.IncreaseObjectCounter();

        yield return new WaitForSeconds(TimeBetweenBullets);

        bulletSpawn = true;
    }
}
