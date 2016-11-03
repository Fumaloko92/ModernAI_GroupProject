using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Executor : MonoBehaviour {
    public GameObject WorldPrefab;
    public GameObject VillagerPrefab;
    public GameObject PlaceholderPrefab;
    public GameObject ConnectionValuePlaceholder;
    public Material MaterialForDestination;
    public int gridHeight;
    public int gridWidth;
    public bool testModeON;
    public int populationCount;

    private int maxGenerations;
    private int generation;
    private List<AIController> population;
    private World currentWorld;
    private State currentState;

    // Use this for initialization
    void Awake()
    {
        population = new List<AIController>();
        GenerateWorld();
        InitializePopulation();
    }

    public void GenerateWorld()
    {
        GameObject g = (GameObject)Instantiate(WorldPrefab, Vector3.zero, Quaternion.identity);
        currentWorld = g.GetComponent<World>();
        currentWorld.InitializeGrid(gridHeight, gridWidth, PlaceholderPrefab, true);
    }

    public void InitializePopulation()
    {
        for (int i = 0; i < populationCount; i++)
        {
            GameObject g = (GameObject)Instantiate(VillagerPrefab, Vector3.zero, Quaternion.identity);
            g.GetComponent<AIController>().InitWorld(currentWorld);
            population.Add(g.GetComponent<AIController>());
        }
    }

    public void Run()
    {

    }

    public void Evaluate()
    {

    }
}
