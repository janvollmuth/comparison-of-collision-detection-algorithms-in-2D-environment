using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerJnR : MonoBehaviour
{
    //Editor
    private string[] collisionType = new string[]
    {
        "BruteForce", "SAP", "UniformGrid", "QuadTree"
    };
    [Dropdown("collisionType")]
    public string CollisionType;

    public static GameControllerJnR Instance { get; private set; }
    public GameObject Player;
    public GameObject Ground;

    private BruteForce bruteForce;
    private Quadtree quadtree;
    private UniformGrid uniformGrid;
    private SweepAndPrune sap;
    private ArrayList pipeData;
    private ArrayList objectData;


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
                bruteForce = new BruteForce();
                quadtree = new Quadtree(new RectangleQT(1, 0, 18, 11));
                uniformGrid = GetComponent<UniformGrid>();
                sap = new SweepAndPrune();
                break;

            case "UniformGrid":
                bruteForce = new BruteForce();
                uniformGrid = GetComponent<UniformGrid>();
                uniformGrid.GenerateGrid();
                sap = new SweepAndPrune();
                break;

            case "SAP":
                bruteForce = new BruteForce();
                sap = new SweepAndPrune();
                uniformGrid = GetComponent<UniformGrid>();
                break;

            default:
                bruteForce = new BruteForce();
                quadtree = new Quadtree(new RectangleQT(1, 0, 18, 11));
                uniformGrid = GetComponent<UniformGrid>();
                sap = new SweepAndPrune();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Stats.Instance.ResetStats();
        pipeData = GetComponent<PipeSpawner>().GetPipes();
        objectData = new ArrayList();

        switch (CollisionType)
        {
            case "BruteForce":
                UpdateBruteForce();
                break;

            case "QuadTree":
                quadtree.ClearQuadTree();
                UpdateQuadTree();
                break;

            case "UniformGrid":
                UpdateUniformGrid();
                break;

            case "SAP":
                UpdateSweepAndPrune();
                break;

            default:
                UpdateBruteForce();
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
        objectData = new ArrayList();
        objectData.Add(Player);
        CollisionBody cb = new CollisionBody(Player.transform.position.x, Player.transform.position.y, (Player.transform.localScale.x / 2), 0);
        cb.ShowBoundries();
        Stats.Instance.IncreaseObjectCounter();

        objectData.Add(Ground);
        cb = new CollisionBody(Ground.transform.position.x, Ground.transform.position.y, (Ground.transform.localScale.y / 2), 1);
        cb.ShowBoundries();
        Stats.Instance.IncreaseObjectCounter();

        for (int i = 0; i < pipeData.Count; i++)
        {
            if (pipeData[i] != null)
            {
                GameObject pipe = (GameObject)pipeData[i];
                cb = new CollisionBody(pipe.transform.position.x, pipe.transform.position.y, (pipe.transform.localScale.x/2), i);
                cb.ShowBoundries();
                Stats.Instance.IncreaseObjectCounter();
                objectData.Add(pipe);
            }
        }
    }

    private void CheckCollisionBruteForce()
    {
        bruteForce.CheckCollisionBruteForce(objectData);
    }

    #endregion

    #region Quadtree

    private void UpdateQuadTree()
    {
        //Can be used when you have a fixed map size
        //quadtree.ClearQuadTree();

        InsertPlayerToQuadTree();
        InsertGroundToQuadTree();
        InsertPipesToQuadtree();
        CheckCollisionQuadTree();
    }

    private void InsertPlayerToQuadTree()
    {
        quadtree.Insert(Player.transform.position.x, Player.transform.position.y, 0.5f, 0);
        objectData.Add(Player);
    }

    private void InsertGroundToQuadTree()
    {
        quadtree.InsertRect(Ground.transform.position.x, Ground.transform.position.y, 36f, 3f, 1);
        objectData.Add(Ground);
        Stats.Instance.AddQuadTreeObjects();
    }

    private void InsertPipesToQuadtree()
    {
        for (int i = 0; i < pipeData.Count; i++)
        {
            GameObject pipe = (GameObject)pipeData[i];
            objectData.Add(pipe);
            quadtree.InsertRect(pipe.transform.position.x, pipe.transform.position.y, pipe.transform.localScale.x, pipe.transform.localScale.y, i);
            Stats.Instance.AddQuadTreeObjects();
        }

        quadtree.ShowBoundries(quadtree);
    }

    private void CheckCollisionQuadTree()
    {
        for (int i = 0; i < objectData.Count; i++)
        {
            GameObject cb = (GameObject)objectData[i];

            int collisionBody = -1;

            if (i > 0)
            {
                collisionBody = quadtree.QuadtreeCollisionRec(quadtree, cb.transform.position.x, cb.transform.position.y, cb.transform.localScale.x, cb.transform.localScale.y, i);
            }
            else
            {
                collisionBody = quadtree.QuadtreeCollision(quadtree, cb.transform.position.x, cb.transform.position.y, 0.5f, i);
            }

            if (collisionBody == 0)
            {
                Debug.Break();
                Application.Quit();
                
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
        InsertGroundToUniformGrid();
        InsertPipesToUniformGrid();
        CheckCollissionUniformGrid();
    }

    private void InsertPlayerToUniformGrid()
    {
        uniformGrid.InsertRect(Player.transform.position.x, Player.transform.position.y, 1f, 1f, 0);
        objectData.Add(Player);
    }

    private void InsertGroundToUniformGrid()
    {
        uniformGrid.InsertRect(Ground.transform.position.x, Ground.transform.position.y, 36f, 3f, 1);
        objectData.Add(Ground);
    }

    private void InsertPipesToUniformGrid()
    {
        for (int i = 0; i < pipeData.Count; i++)
        {
            if (pipeData[i] != null)
            {
                GameObject pipe = (GameObject)pipeData[i];
                uniformGrid.InsertRect(pipe.transform.position.x, pipe.transform.position.y, 3f, 15f, i+2);
                objectData.Add(pipe);
            }
        }
    }

    private void CheckCollissionUniformGrid()
    {
        for (int i = 0; i < objectData.Count; i++)
        {
            GameObject gameObject = (GameObject)objectData[i];
            int cb;

            cb = uniformGrid.CheckCollisionUniformGridRect(objectData, gameObject.transform.position.x, gameObject.transform.position.y, i, gameObject.transform.localScale.x, gameObject.transform.localScale.y);

            if (cb == 0)
            {
                Debug.Break();
                Application.Quit();
            }
        }
    }

    #endregion

    #region Sweep&Prune

    private void UpdateSweepAndPrune()
    {
        Stats.Instance.ResetObjectCounter();

        objectData = new ArrayList();
        objectData.Add(Player);
        objectData.Add(Ground);

        for (int i = 0; i < pipeData.Count; i++)
        {
            if (pipeData[i] != null)
            {
                GameObject pipe = (GameObject)pipeData[i];
                objectData.Add(pipe);
            }
        }

        Stats.Instance.SetObjectCounter(objectData.Count);

        List<Bound> list = sap.UpdateSAPRect(objectData);
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

                            if (sap.CheckCollisionSAPRect(objectData, bound1, bound2))
                            {
                                if(bound1 == 0 || bound2 == 0)
                                {
                                    Debug.Break();
                                    Application.Quit();
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
