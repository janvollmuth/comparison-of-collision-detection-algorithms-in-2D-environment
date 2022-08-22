using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerRLBHS : MonoBehaviour
{
    //Editor
    private string[] collisionType = new string[]
    {
        "BruteForce", "SAP", "UniformGrid", "QuadTree"
    };
    [Dropdown("collisionType")]
    public string CollisionType;

    public static GameControllerRLBHS Instance { get; private set; }
    public GameObject player;

    private BruteForce bruteForce;
    private Quadtree quadtree;
    private UniformGrid uniformGrid;
    private SweepAndPrune sap;
    private ArrayList objectData;
    private ArrayList objectDataWithID;


    // Start is called before the first frame update
    void Start()
    {
        switch (CollisionType)
        {
            case "BruteForce":
                bruteForce = new BruteForce();
                sap = new SweepAndPrune();
                uniformGrid = GetComponent<UniformGrid>();
                break;

            case "QuadTree":
                quadtree = new Quadtree(new RectangleQT(player.transform.position.x + 1, player.transform.position.y + 1, 25, 25));
                uniformGrid = GetComponent<UniformGrid>();
                sap = new SweepAndPrune();
                break;

            case "UniformGrid":
                uniformGrid = GetComponent<UniformGrid>();
                sap = new SweepAndPrune();
                break;

            case "SAP":
                sap = new SweepAndPrune();
                uniformGrid = GetComponent<UniformGrid>();

                break;

            default:
                quadtree = new Quadtree(new RectangleQT(player.transform.position.x + 1, player.transform.position.y + 1, 25, 25));
                uniformGrid = GetComponent<UniformGrid>();
                sap = new SweepAndPrune();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Stats.Instance.ResetStats();
        objectData = GetComponent<WaveSpawner>().ObjectData;
        objectDataWithID = new ArrayList();

        switch (CollisionType)
        {
            case "BruteForce":
                UpdateBruteForce();
                break;

            case "QuadTree":
                quadtree = new Quadtree(new RectangleQT(player.transform.position.x + 1, player.transform.position.y + 1, 25, 25));
                UpdateQuadTree();
                break;

            case "UniformGrid":
                UpdateUniformGrid();
                break;

            case "SAP":
                UpdateSweepAndPrune();
                break;

            default:
                quadtree = new Quadtree(new RectangleQT(player.transform.position.x + 1, player.transform.position.y + 1, 25, 25));
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
        objectDataWithID = new ArrayList();
        objectDataWithID.Add(player);
        CollisionBody cb = new CollisionBody(player.transform.position.x, player.transform.position.y, (player.transform.localScale.x / 2), 0);
        cb.ShowBoundries();
        Stats.Instance.IncreaseObjectCounter();

        for (int i = 0; i < objectData.Count; i++)
        {
            if (objectData[i] != null)
            {
                GameObject pipe = (GameObject)objectData[i];
                cb = new CollisionBody(pipe.transform.position.x, pipe.transform.position.y, (pipe.transform.localScale.x / 2), i);
                cb.ShowBoundries();
                objectDataWithID.Add(pipe);
            }
        }
        Stats.Instance.SetObjectCounter(objectDataWithID.Count);
    }

    private void CheckCollisionBruteForce()
    {
        bruteForce.CheckCollisionBruteForce(objectDataWithID);
    }

    #endregion

    #region Quadtree

    private void UpdateQuadTree()
    {
        //Can be used when you have a fixed map size
        //quadtree.ClearQuadTree();

        InsertPlayerToQuadTree();
        InsertEnemyToQuadtree();
        CheckCollisionQuadTree();
    }

    public void InsertPlayerToQuadTree()
    {
        quadtree.Insert(player.transform.position.x, player.transform.position.y, 0.5f, 0);
        objectDataWithID.Add(player);
    }

    private void InsertEnemyToQuadtree()
    {
        for (int i = 0; i < objectData.Count; i++)
        {
            GameObject enemy = (GameObject)objectData[i];

            if(enemy.GetComponent<EnemyController>() != null)
            {
                enemy.GetComponent<EnemyController>().id = i;

                objectDataWithID.Add(enemy);

                if (enemy.activeSelf)
                {
                    quadtree.Insert(enemy.transform.position.x, enemy.transform.position.y, 0.5f, i);
                }
            }
        }

        quadtree.ShowBoundries(quadtree);
        GetComponent<WaveSpawner>().ObjectData = objectDataWithID;
    }

    private void CheckCollisionQuadTree()
    {
        for (int i = 0; i < objectData.Count; i++)
        {
            GameObject cb = (GameObject)objectData[i];

            int collisionBody = quadtree.QuadtreeCollision(quadtree, cb.transform.position.x, cb.transform.position.y, 0.5f, i);

            if (collisionBody == 0)
            {
                Destroy(cb);
                objectDataWithID.RemoveAt(i);
                Stats.Instance.DecreaseObjectCounter();
            }

            if (collisionBody != -1 && collisionBody > 0)
            {
                //Debug.Log("Set enemy position");
                cb.GetComponent<EnemyController>().SetUpdate(false);
                GameObject gameObject = (GameObject)objectData[collisionBody];
                gameObject.GetComponent<EnemyController>().SetUpdate(false);
            } 
        }
        GetComponent<WaveSpawner>().ObjectData = objectDataWithID;
    }

    #endregion

    #region UniformGrid

    private void UpdateUniformGrid()
    {
        uniformGrid.GenerateGrid();
        InsertPlayerToUniformGrid();
        InsertEnemyToUniformGrid();
        CheckColliisonUniformGrid();
    }

    private void InsertPlayerToUniformGrid()
    {
        uniformGrid.Insert(player.transform.position.x, player.transform.position.y, 0.5f, 0);
        objectDataWithID.Add(player);
    }

    private void InsertEnemyToUniformGrid()
    {
        for (int i = 0; i < objectData.Count; i++)
        {
            if(objectData[i] != null)
            {
                GameObject enemy = (GameObject)objectData[i];

                if (enemy.GetComponent<EnemyController>() != null)
                {
                    enemy.GetComponent<EnemyController>().id = i;

                    objectDataWithID.Add(enemy);

                    if (enemy.activeSelf)
                    {
                        uniformGrid.Insert(enemy.transform.position.x, enemy.transform.position.y, 0.5f, i);
                    }
                }
            }
        }

        GetComponent<WaveSpawner>().ObjectData = objectDataWithID;
    }

    private void CheckColliisonUniformGrid()
    {
        for(int i = 0; i < objectData.Count; i++)
        {
            GameObject gameObject = (GameObject)objectData[i];
            int cb;

            if (gameObject.GetComponent<EnemyController>() != null)
            {

                cb = uniformGrid.CheckCollisionUniformGrid(objectData, gameObject.transform.position.x, gameObject.transform.position.y, gameObject.GetComponent<EnemyController>().id, 0.5f);
                
                if (cb == 0)
                {
                    Destroy(gameObject);
                    objectDataWithID.RemoveAt(i);
                    Stats.Instance.DecreaseObjectCounter();
                }
            }
        }
        GetComponent<WaveSpawner>().ObjectData = objectDataWithID;
    }

    #endregion

    #region Sweep&Prune

    private void UpdateSweepAndPrune()
    {
        List<Bound> list = sap.UpdateSAP(objectData);
        CheckCollisionSweepAndPrune(list);
    }

    private void CheckCollisionSweepAndPrune(List<Bound> list)
    {
        ArrayList collisionObjects = new ArrayList();

        for(int i = 0; i < list.Count; i++)
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

                    foreach(int bound2 in collisionObjects)
                    {
                        if(bound1 != bound2)
                        {
                            Stats.Instance.AddCollisionCheck();

                            if (sap.CheckCollisionSAP(objectData, bound1, bound2, 0.5f))
                            {
                                //Debug.Log(bound1 + " and " + bound2 + " overlapped.");

                                if(bound1 == 0)
                                {
                                    Destroy((GameObject)objectData[bound2]);
                                    objectData.RemoveAt(bound2);
                                    Stats.Instance.DecreaseObjectCounter();
                                }

                                if(bound2 == 0)
                                {       
                                    Destroy((GameObject)objectData[bound1]);
                                    objectData.RemoveAt(bound1);
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
