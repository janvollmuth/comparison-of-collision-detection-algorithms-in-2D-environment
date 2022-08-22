using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    public float MaxTime = 1;
    public float Height;
    public GameObject Pipe;

    private float timer = 0;
    private ArrayList pipes;


    // Start is called before the first frame update
    void Start()
    {
        pipes = new ArrayList();

        GameObject newPipe = Instantiate(Pipe);
        newPipe.transform.position = transform.position + new Vector3(20, Random.Range(-Height, Height), 0);

        GameObject pipeTop = newPipe.GetComponentInChildren<PipeMovement>().PipeTop;
        GameObject pipeBottom = newPipe.GetComponentInChildren<PipeMovement>().PipeBottom;

        pipes.Add(pipeTop);
        pipes.Add(pipeBottom);
    }

    // Update is called once per frame
    void Update()
    {
        if(timer > MaxTime && pipes.Count < 7)
        {
            GameObject newPipe = Instantiate(Pipe);
            newPipe.transform.position = transform.position + new Vector3(20, (int)Random.Range(-1, Height), 10);

            GameObject pipeTop = newPipe.GetComponentInChildren<PipeMovement>().PipeTop;
            GameObject pipeBottom = newPipe.GetComponentInChildren<PipeMovement>().PipeBottom;

            pipes.Add(pipeTop);
            pipes.Add(pipeBottom);
            timer = 0;
        }

        timer += Time.deltaTime;
    }

    public ArrayList GetPipes()
    {
        return pipes;
    }
}
