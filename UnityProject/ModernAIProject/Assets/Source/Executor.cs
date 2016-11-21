using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Executor : MonoBehaviour {
    public GameObject WorldPrefab;
    public GameObject VillagerPrefab;
    public Material MaterialForDestination;
    public bool testModeON;
    public int populationCount;

    private int maxGenerations;
    private int generation;
    private List<AIController> population;
    private World currentWorld;

    public bool runInBackground = false;

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
    }

    public void InitializePopulation()
    {
        for (int i = 0; i < populationCount; i++)
        {
            GameObject g = (GameObject)Instantiate(VillagerPrefab, Vector3.zero, Quaternion.identity);

            //spawn at random location
            int sizeX = Mathf.RoundToInt(Terrain.activeTerrain.terrainData.size.x);
            int sizeZ = Mathf.RoundToInt(Terrain.activeTerrain.terrainData.size.z);

            Vector3 pos = new Vector3(StaticRandom.Rand(-sizeX/2,sizeX/2),0,StaticRandom.Rand(-sizeZ/2,sizeZ/2));
            g.transform.position = new Vector3(pos.x,Terrain.activeTerrain.SampleHeight(pos),pos.z);
            //

            //name the villager with it's instance ID, so we can identify it in the console view
            g.name = "Villager [" + g.GetInstanceID()+"]";

            AIController aiControl = g.GetComponent<AIController>();

            aiControl.InitWorld(currentWorld);
            population.Add(aiControl);
        }
    }

    public void Run()
    {

    }

    public void Evaluate()
    {

    }

    //get random villager that is not the villager in the param
    public AIController GetRandomVillager(AIController curAgent) 
    {
        if (population.Count <= 1) //if noone but the param villager in the world is available
        {
            return null;
        }
        else
        {
            AIController villager = null;

            //get random villager
            villager = population[StaticRandom.Rand(0, population.Count)];
            while (villager.Equals(curAgent))
            {
                villager = population[StaticRandom.Rand(0, population.Count)];
            }

            return villager;
        }
    }

    public void KillVillager(AIController victim)
    {
        //remove from population and destroy villager - disabled for testing purposes
        /*
        population.Remove(victim);
        Destroy(victim.gameObject);*/
    }

    //Get villager with resource, which is not the villager in the param
    public AIController GetVillagerWithResource(AIController curAgent)
    {
        if (population.Count <= 1)
        {
            return null;
        }
        else
        {
            AIController villager = null;

            //go through each villager and choose the one that is not the curAgent and has atleast one resource
            foreach(AIController agent in population)
            {
                if(!agent.Equals(curAgent))
                {
                    if(agent.collectedResources.Count > 0)
                    {
                        villager = agent;
                        break;
                    }
                }
            }

            return villager;
        }
    }
}
