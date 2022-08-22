using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerSEU : MonoBehaviour
{
    //Editor
    private string[] collisionType = new string[]
    {
        "BruteForce", "SAP", "UniformGrid", "QuadTree"
    };
    [Dropdown("collisionType")]
    public string CollisionType;

    public static GameControllerSEU Instance { get; private set; }
    public GameObject player;

    private BruteForce bruteForce;
    private Quadtree quadtree;
    private UniformGrid uniformGrid;
    private SweepAndPrune sap;
    private ArrayList objectData;
    private ArrayList bulletData;
    private ArrayList enemyData;
    private GameObject[] enemys;


    private WaveSpawnerSEU waveSpawner;

    // Start is called before the first frame update
    void Start()
    {
        waveSpawner = GetComponent<WaveSpawnerSEU>();

        switch (CollisionType)
        {
            case "BruteForce":
                bruteForce = new BruteForce();
                sap = new SweepAndPrune();
                uniformGrid = GetComponent<UniformGrid>();
                break;

            case "QuadTree":
                quadtree = new Quadtree(new RectangleQT(transform.position.x, transform.position.y, 13, 13));
                uniformGrid = GetComponent<UniformGrid>();
                sap = new SweepAndPrune();
                break;

            case "UniformGrid":
                uniformGrid = GetComponent<UniformGrid>();
                sap = new SweepAndPrune();
                uniformGrid.GenerateGrid();
                break;

            case "SAP":
                sap = new SweepAndPrune();
                uniformGrid = GetComponent<UniformGrid>();

                break;

            default:
                quadtree = new Quadtree(new RectangleQT(transform.position.x, transform.position.y, 13, 13));
                uniformGrid = GetComponent<UniformGrid>();
                sap = new SweepAndPrune();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Stats.Instance.ResetStats();
        objectData = new ArrayList();
        bulletData = player.GetComponent<PlayerControllerSEU>().BulletList;

        switch (CollisionType)
        {
            case "BruteForce":
                UpdateBruteForce();
                break;

            case "QuadTree":
                quadtree = new Quadtree(new RectangleQT(transform.position.x, transform.position.y, 13, 13));
                UpdateQuadTree();
                break;

            case "UniformGrid":
                UpdateUniformGrid();
                break;

            case "SAP":
                UpdateSweepAndPrune();
                break;

            default:
                quadtree = new Quadtree(new RectangleQT(transform.position.x, transform.position.y, 13, 13));
                UpdateQuadTree();
                break;
        }
    }

    #region BruteForce

    private void UpdateBruteForce()
    {
        Stats.Instance.ResetObjectCounter();
        InsertObjectsBruteForce();
        CheckCollisionBruteForce();
    }

    private void InsertObjectsBruteForce()
    {
        objectData.Add(player);
        CollisionBody cb = new CollisionBody(player.transform.position.x, player.transform.position.y, (player.transform.localScale.x / 2), 0);
        cb.ShowBoundries();

        enemys = GameObject.FindGameObjectsWithTag("Enemy");

        for (int i = 0; i < enemys.Length; i++)
        {
            GameObject enemy = enemys[i];
            cb = new CollisionBody(enemy.transform.position.x, enemy.transform.position.y, (enemy.transform.localScale.x / 2), i);
            cb.ShowBoundries();
            objectData.Add(enemy);
        }

        ArrayList bullets = player.GetComponent<PlayerControllerSEU>().BulletList;

        for (int i = 0; i < bullets.Count; i++)
        {
            GameObject bullet = (GameObject)bullets[i];
            cb = new CollisionBody(bullet.transform.position.x, bullet.transform.position.y, (bullet.transform.localScale.x / 2), i);
            cb.ShowBoundries();
            objectData.Add(bullet);
        }
        Stats.Instance.SetObjectCounter(objectData.Count);
    }

    private void CheckCollisionBruteForce()
    {
        bruteForce.CheckCollisionBruteForce(objectData);
    }

    #endregion

    #region QuadTree

    private void UpdateQuadTree()
    {
        quadtree.ClearQuadTree();

        InsertPlayerToQuadTree();
        InsertEnemysToQuadtree();
        InsertBulletsToQuadTree();
        quadtree.ShowBoundries(quadtree);
        CheckCollisionQuadTree();
    }

    private void InsertPlayerToQuadTree()
    {
        quadtree.Insert(player.transform.position.x, player.transform.position.y, 0.5f, 0);
        objectData.Add(player);
    }

    private void InsertEnemysToQuadtree()
    {
        enemys = GameObject.FindGameObjectsWithTag("Enemy");

        for (int i = 0; i < enemys.Length; i++)
        {
            GameObject enemy = enemys[i];

            quadtree.Insert(enemy.transform.position.x, enemy.transform.position.y, 0.5f, objectData.Count);
            objectData.Add(enemy);
        }
    }

    private void InsertBulletsToQuadTree()
    {
        ArrayList bullets = player.GetComponent<PlayerControllerSEU>().BulletList;

        for(int i = 0; i < bullets.Count; i++)
        {
            GameObject bullet = (GameObject)bullets[i];
            quadtree.Insert(bullet.transform.position.x, bullet.transform.position.y, 0.25f, objectData.Count);
            objectData.Add(bullet);
        }
    }

    private void CheckCollisionQuadTree()
    {
        for (int i = 0; i < objectData.Count; i++)
        {
            GameObject cb = (GameObject)objectData[i];

            int collisionBody = quadtree.QuadtreeCollision(quadtree, cb.transform.position.x, cb.transform.position.y, 0.5f, i);

            if (collisionBody != -1 && collisionBody < objectData.Count)
            {
                GameObject cb2 = (GameObject)objectData[collisionBody];
                if (cb.gameObject.name.Contains("Player") && cb2.gameObject.name.Contains("Enemy"))
                {
                    Debug.Break();
                    Application.Quit();
                }

                if (cb2.gameObject.name.Contains("Player") && cb.gameObject.name.Contains("Enemy"))
                {
                    Debug.Break();
                    Application.Quit();
                }

                if (cb.gameObject.name.Contains("Bullet") && cb2.gameObject.name.Contains("Enemy"))
                {
                    objectData.RemoveAt(i);
                    bulletData.Remove(cb);
                    player.GetComponent<PlayerControllerSEU>().BulletList = bulletData;

                    Destroy(cb);
                    Destroy(cb2);
                }
            } 
        }
    }

    #endregion

    #region UniformGrid

    private void UpdateUniformGrid()
    {
        uniformGrid.ClearGrid();
        uniformGrid.ShowBoundries();
        InsertPlayerToUniformGrid();
        InsertEnemysToUniformGrid();
        InsertBulletsToUniformGrid();
        CheckColliisonUniformGrid();
    }

    private void InsertPlayerToUniformGrid()
    {
        uniformGrid.Insert(player.transform.position.x, player.transform.position.y, 0.5f, 0);
        objectData.Add(player);
    }

    private void InsertEnemysToUniformGrid()
    {
        enemys = GameObject.FindGameObjectsWithTag("Enemy");

        for (int i = 0; i < enemys.Length; i++)
        {
            GameObject enemy = enemys[i];

            uniformGrid.Insert(enemy.transform.position.x, enemy.transform.position.y, 0.5f, objectData.Count);
            objectData.Add(enemy);
        }
    }

    private void InsertBulletsToUniformGrid()
    {
        ArrayList bullets = player.GetComponent<PlayerControllerSEU>().BulletList;

        for (int i = 0; i < bullets.Count; i++)
        {
            GameObject bullet = (GameObject)bullets[i];
            uniformGrid.Insert(bullet.transform.position.x, bullet.transform.position.y, 0.25f, objectData.Count);
            objectData.Add(bullet);
        }
    }

    private void CheckColliisonUniformGrid()
    {
        for (int i = 0; i < objectData.Count; i++)
        {
            GameObject cb = (GameObject)objectData[i];

            int collisionBody = uniformGrid.CheckCollisionUniformGrid(objectData, cb.transform.position.x, cb.transform.position.y, i, 0.5f);

            if (collisionBody != -1 && collisionBody < objectData.Count)
            {
                GameObject cb2 = (GameObject)objectData[collisionBody];

                if (cb.gameObject.name.Contains("Player") && cb2.gameObject.name.Contains("Enemy"))
                {
                    Debug.Break();
                    Application.Quit();
                }

                if (cb2.gameObject.name.Contains("Player") && cb.gameObject.name.Contains("Enemy"))
                {
                    Debug.Break();
                    Application.Quit();
                }

                if (cb.gameObject.name.Contains("Bullet") && cb2.gameObject.name.Contains("Enemy"))
                {
                    //Debug.Log("Collision Object " + cb.gameObject.name + " & Object " + cb2.gameObject.name);

                    objectData.RemoveAt(i);
                    bulletData.Remove(cb);
                    player.GetComponent<PlayerControllerSEU>().BulletList = bulletData;

                    Destroy(cb);
                    Destroy(cb2);
                }
            }
        }
    }

    #endregion

    #region Sweep & Prune

    private void UpdateSweepAndPrune()
    {
        objectData.Add(player);

        enemys = GameObject.FindGameObjectsWithTag("Enemy");

        for (int i = 0; i < enemys.Length; i++)
        {
            GameObject enemy = enemys[i];
            objectData.Add(enemy);
        }

        ArrayList bullets = player.GetComponent<PlayerControllerSEU>().BulletList;

        for (int i = 0; i < bullets.Count; i++)
        {
            GameObject bullet = (GameObject)bullets[i];
            objectData.Add(bullet);
        }


        List<Bound> list = sap.UpdateSAP(objectData);
        CheckCollisionSweepAndPrune(list);
    }

    private void CheckCollisionSweepAndPrune(List<Bound> list)
    {
        ArrayList collisionObjects = new ArrayList();

        for (int i = 0; i < list.Count; i++)
        {
            Bound b = list[i];

            switch (b.upperOrLower)
            {
                case 0:
                    collisionObjects.Add(b.id);
                    break;

                case 1:

                    int bound1 = b.id;
                    collisionObjects.Remove(b.id);

                    foreach (int bound2 in collisionObjects)
                    {
                        if (bound1 != bound2)
                        {
                            Stats.Instance.AddCollisionCheck();

                            if (sap.CheckCollisionSAP(objectData, bound1, bound2, 0.5f))
                            {
                                //Debug.Log(bound1 + " and " + bound2 + " overlapped.");

                                GameObject cb1 = (GameObject)objectData[bound1];
                                GameObject cb2 = (GameObject)objectData[bound2];

                                if (cb1.gameObject.name.Contains("Player") && cb2.gameObject.name.Contains("Enemy"))
                                {
                                    Debug.Break();
                                    Application.Quit();
                                }

                                if (cb2.gameObject.name.Contains("Player") && cb1.gameObject.name.Contains("Enemy"))
                                {
                                    Debug.Break();
                                    Application.Quit();
                                }

                                if (cb1.gameObject.name.Contains("Bullet") && cb2.gameObject.name.Contains("Enemy"))
                                {
                                    bulletData.Remove(cb1);
                                    player.GetComponent<PlayerControllerSEU>().BulletList = bulletData;

                                    Destroy(cb1);
                                    Stats.Instance.DecreaseObjectCounter();
                                    Destroy(cb2);
                                    Stats.Instance.DecreaseObjectCounter();
                                }
                            }
                        }
                    }
                    break;
            }
        }
    }

    #endregion
}
