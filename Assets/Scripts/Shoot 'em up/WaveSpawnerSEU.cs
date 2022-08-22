using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawnerSEU : MonoBehaviour
{
    public GameObject[] EnemyList;
    public float TimeBetweenWaves;
    public GameObject Formation1;
    public GameObject Formation2;
    public GameObject Formation3;
    public Transform Spawn1;
    public Transform Spawn2;
    public Transform Spawn3;

    private bool waveSpawn;

    // Start is called before the first frame update
    void Start()
    {
        EnemyList = new GameObject[0];
        waveSpawn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (waveSpawn)
        {
            StartCoroutine(waveSpawner());
        }

        EnemyList = GameObject.FindGameObjectsWithTag("Enemy");

        CheckFormationOutOfBounds();
    }

    IEnumerator waveSpawner()
    {
        waveSpawn = false;

        int rand = Random.Range(1, 4);
        Transform spawnPoint = null;

        switch (rand)
        {
            case 1:
                spawnPoint = Spawn1;
                break;

            case 2:
                spawnPoint = Spawn2;
                break;

            case 3:
                spawnPoint = Spawn3;
                break;
        }

        rand = Random.Range(1, 4);
        GameObject formation = null;

        switch (rand)
        {
            case 1:
                formation = Instantiate(Formation1);
                Stats.Instance.IncreaseObjectCounter(3);
                break;

            case 2:
                formation = Instantiate(Formation2);
                Stats.Instance.IncreaseObjectCounter(3);
                break;

            case 3:
                formation = Instantiate(Formation3);
                Stats.Instance.IncreaseObjectCounter(7);
                break;
        }

        formation.transform.localPosition = new Vector3(spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z);

        yield return new WaitForSeconds(TimeBetweenWaves);

        waveSpawn = true;
    }

    private void CheckFormationOutOfBounds()
    {
        if (EnemyList.Length > 0)
        {
            for (int i = 0; i < EnemyList.Length; i++)
            {
                GameObject gameObject = EnemyList[i];

                if (gameObject.transform.position.y < -12)
                {
                    EnemyList[i] = null;
                    Destroy(gameObject);
                    Stats.Instance.DecreaseObjectCounter();
                }
            }
        }
    }
}
