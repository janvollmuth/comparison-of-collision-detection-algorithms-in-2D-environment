using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public float SpawnRate;
    public float TimeBetweenWaves;

    public int EnemyCount;
    public ArrayList ObjectData;

    public GameObject enemy;
    public GameObject player;

    private bool waveIsDone = true;

    void Start()
    {
        ObjectData = new ArrayList();
        ObjectData.Add(player);
    }

    // Update is called once per frame
    void Update()
    {
        if (waveIsDone)
        {
            StartCoroutine(waveSpawner());
        }
    }

    IEnumerator waveSpawner()
    {
        waveIsDone = false;

        for (int i = 0; i < EnemyCount; i++)
        {
            int corner = Random.Range(0, 3);
            Vector2 pos = GetRandomSpawnLocation();

            GameObject enemyClone = Instantiate(enemy);
            ObjectData.Add(enemyClone);
            enemyClone.transform.SetPositionAndRotation(pos, new Quaternion(0, 0, 0, 0));

            //Benchmarking counter for dynamic objects
            Stats.Instance.IncreaseObjectCounter();

            yield return new WaitForSeconds(SpawnRate);
        }

        SpawnRate -= 0.1f;
        EnemyCount += 3;

        yield return new WaitForSeconds(TimeBetweenWaves);

        waveIsDone = true;
    }

    private Vector2 GetRandomSpawnLocation()
    {
        int corner = Random.Range(0, 4);

        float randomX;
        float randomY;
        Vector2 pos;

        switch (corner)
        {
            case 0:
                //upper right
                randomX = Random.Range(player.transform.position.x + 15f, player.transform.position.x + 30f);
                randomY = Random.Range(player.transform.position.y + 15f, player.transform.position.y + 30f);
                pos = new Vector3(randomX, randomY, 0f);
                break;

            case 1:
                //upper left
                randomX = Random.Range(player.transform.position.x - 15f, player.transform.position.x - 30f);
                randomY = Random.Range(player.transform.position.y + 15f, player.transform.position.y + 30f);
                pos = new Vector3(randomX, randomY, 0f);
                break;

            case 2:
                //bottom right
                randomX = Random.Range(player.transform.position.x + 15f, player.transform.position.x + 30f);
                randomY = Random.Range(player.transform.position.y - 15f, player.transform.position.y - 30f);
                pos = new Vector3(randomX, randomY, 0f);
                break;

            case 3:
                //bottom left
                randomX = Random.Range(player.transform.position.x - 15f, player.transform.position.x - 30f);
                randomY = Random.Range(player.transform.position.y - 15f, player.transform.position.y - 30f);
                pos = new Vector3(randomX, randomY, 0f);
                break;

            default:
                pos = new Vector3(0f, 0f, 0f);
                break;

        }

        return pos;
    }
}
