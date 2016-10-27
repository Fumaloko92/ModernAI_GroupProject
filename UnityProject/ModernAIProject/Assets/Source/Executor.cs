using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Executor : MonoBehaviour {
    public GameObject WorldPrefab;
    public GameObject VillagerPrefab;

    private int populationCount;
    private int maxGenerations;
    private int generation;
    private List<AIController> population;
    private World currentWorld;
    private State currentState;

	// Use this for initialization
	void Awake () {
        population = new List<AIController>();
        GenerateWorld();
        InitializePopulation();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void GenerateWorld()
    {
        GameObject g = (GameObject)Instantiate(WorldPrefab, Vector3.zero, Quaternion.identity);
        currentWorld = g.GetComponent<World>();
    }

    public void InitializePopulation()
    {
        GameObject g = (GameObject)Instantiate(VillagerPrefab, Vector3.zero, Quaternion.identity);
        g.GetComponent<AIController>().InitWorld(currentWorld);
        population.Add(g.GetComponent<AIController>());
    }

    public void Run()
    {

    }

    public void Evaluate()
    {

    }
}
