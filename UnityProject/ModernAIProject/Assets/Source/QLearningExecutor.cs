using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class QLearningExecutor : MonoBehaviour {

    public GameObject WorldPrefab;
    public GameObject VillagerPrefab;
    public Material MaterialForDestination;
    public bool testModeON;

    private List<ThreadSafe.AIGroup> population;
    private static List<float> fitnesses;

    public int popCount = 1000;
    public int memberCount = 100;

    private World currentWorld;

    public bool runInBackground = false;
    // Use this for initialization
    void Awake()
    {

        GenerateWorld();

        population = new List<ThreadSafe.AIGroup>();
        fitnesses = new List<float>();
        for (int i = 0; i < popCount; i++ )
        {
            ThreadSafe.AIGroup ai = new ThreadSafe.AIGroup();
            ai.InitAIs(memberCount);
            ai.InitWorld(new ThreadSafe.World(currentWorld.ResourcePositions));

            population.Add(ai);
        }
    }


    void Start()
    {
        //InitializePopulation();
        //set which way the ai members should perform their qlearning
        ThreadSafe.AIGroup.executeMethod = ThreadSafe.AIGroup.ExecuteMethod.oneAIAtATime;

        StartCoroutine(runExperiment());
    }
    public void GenerateWorld()
    {
        GameObject g = (GameObject)Instantiate(WorldPrefab, Vector3.zero, Quaternion.identity);
        currentWorld = g.GetComponent<World>();
    }

    IEnumerator runExperiment()
    {
        foreach(ThreadSafe.AIGroup ai in population)
        {

            fitnesses.Add(ai.EvaluateQLearning());
            Serialize();

            yield return null;
        }
    }

    static public void Serialize()
    {

        string name = "log.csv";
        string serialization = "";
        serialization += "POP_NUM;FITNESS" + Environment.NewLine;
        int i = 1;
        foreach(float fitness in fitnesses)
        {
            serialization += i + ";" + fitness + Environment.NewLine;
            i++;
        }
        File.WriteAllText(name, serialization);

    }
}
