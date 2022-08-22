using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stats : MonoBehaviour
{
    public int CollisionChecks;
    public int ObjectCounter;
    public int QuadTreeObjects;
    public int UniformGridOjects;
    public float FramesPerSecond;

    private List<StatObject> stats;
    private bool fill = true;

    public static Stats Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        stats = new List<StatObject>();
    }

    private void Update()
    {
        if (fill)
        {
            StartCoroutine(fillStats());
        }
    }

    IEnumerator fillStats()
    {
        fill = false;

        StatObject statObject = new StatObject(CollisionChecks, ObjectCounter, QuadTreeObjects, UniformGridOjects, FramesPerSecond);

        stats.Add(statObject);

        Debug.Log(stats.Count);

        if(stats.Count == 120)
        {
            string path = "E:/Repositiories/CollisionDetectionTest/Assets/Auswertungen/RLBHS_QuadTree_Extreme.txt";

            Debug.Log(path);

            StreamWriter writer = new StreamWriter(path, true);

            switch (SceneManager.GetActiveScene().name)
            {
                case "Rouque Like Bullet Hell Survival":
                    writer.WriteLine(SceneManager.GetActiveScene().name + " | " + Instance.GetComponentInParent<GameControllerRLBHS>().CollisionType);
                    break;

                case "Jump'n'Run":
                    writer.WriteLine(SceneManager.GetActiveScene().name + " | " + Instance.GetComponentInParent<GameControllerJnR>().CollisionType);
                    break;

                case "Shoot 'em up":
                    writer.WriteLine(SceneManager.GetActiveScene().name + " | " + Instance.GetComponentInParent<GameControllerSEU>().CollisionType);
                    break;
            }

            for(int i = 0; i < stats.Count; i++)
            {
                writer.WriteLine(i + " | " + stats[i].CollisionChecks + " | " + stats[i].ObjectCounter + " | " + stats[i].QuadTreeObjects + " | " + stats[i].UniformGridOjects + " | " + stats[i].FramesPerSecond);
            }
            writer.Close();
        }

        yield return new WaitForSeconds(1);

        fill = true;
    }

    public Stats()
    {
        CollisionChecks = 0;
        ObjectCounter = 0;
        QuadTreeObjects = 0;
        UniformGridOjects = 0;
        FramesPerSecond = 0;
    }

    public void AddCollisionCheck()
    {
        CollisionChecks++;
    }

    public void IncreaseObjectCounter()
    {
        ObjectCounter++;
    }

    public void IncreaseObjectCounter(int count)
    {
        ObjectCounter += count;
    }

    public void DecreaseObjectCounter()
    {
        ObjectCounter--;
    }

    public void AddQuadTreeObjects()
    {
        QuadTreeObjects++;
    }

    public void AddUniformGridObjects()
    {
        UniformGridOjects++;
    }

    public void ResetStats()
    {
        CollisionChecks = 0;
        QuadTreeObjects = 0;
        UniformGridOjects= 0;
    }

    public void SetObjectCounter(int objects)
    {
        ObjectCounter = objects;
    }

    public void ResetObjectCounter()
    {
        ObjectCounter = 0;
    }

    public void SetFramesPerSecond(float fps)
    {
        FramesPerSecond = fps;
    }

    public class StatObject
    {
        public int CollisionChecks;
        public int ObjectCounter;
        public int QuadTreeObjects;
        public int UniformGridOjects;
        public float FramesPerSecond;

        public StatObject(int collisionChecks, int objectCounter, int quadTreeObjects, int uniformGridOjects, float framesPerSecond)
        {
            CollisionChecks = collisionChecks;
            ObjectCounter = objectCounter;
            QuadTreeObjects = quadTreeObjects;
            UniformGridOjects = uniformGridOjects;
            FramesPerSecond = framesPerSecond;
        }
    }
}
