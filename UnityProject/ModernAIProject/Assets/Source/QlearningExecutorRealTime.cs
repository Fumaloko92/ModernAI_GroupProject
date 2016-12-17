using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class QlearningExecutorRealTime : MonoBehaviour
{

    public GameObject WorldPrefab;
    public GameObject VillagerPrefab;
    public Material MaterialForDestination;
    public bool testModeON;

    private int popIndex = 0;
    private List<AIGroup> population;
    private static List<float> fitnesses;

    public int popCount = 1000;
    public int memberCount = 100;

    private World currentWorld;

    public bool runInBackground = false;
    // Use this for initialization
    void Awake()
    {

        
    }


    void Start()
    {
        //InitializePopulation();
        //set which way the ai members should perform their qlearning
        
        
        //StartCoroutine(runExperiment());

        GenerateWorld();
        AIGroup.executeMethod = AIGroup.ExecuteMethod.oneAIAtATime;
        AIGroup.executor = this;
        
        population = new List<AIGroup>();
        //fitnesses = new List<float>();
        //for (int i = 0; i < popCount; i++)
        //{
            AIGroup ai = new AIGroup();
            ai.CreateQTable();
            
            ai.InitAIs(memberCount);
            ai.InitWorld(currentWorld);
           
            population.Add(ai);
        //}
    }
    public void GenerateWorld()
    {
        GameObject g = (GameObject)Instantiate(WorldPrefab, Vector3.zero, Quaternion.identity);
        currentWorld = g.GetComponent<World>();
    }

    void Update()
    {
        if (popIndex < population.Count)
        {
            bool alive = false;
            foreach (AIController ai in population[popIndex].members)
            {
                if (ai.GetHealth() > 0)
                {
                    alive = true;
                    ai.GetComponent<NavMeshAgent>().enabled = true;
                    ai.executeRealTime();
                }
                else
                {
                    ai.GetComponent<NavMeshAgent>().enabled = false;
                }
            }

            if (!alive)
            {
                Debug.Log("DEAD");
                popIndex++;
               /* AIGroup ai = new AIGroup();
                ai.CreateQTable();

                ai.InitAIs(memberCount);
                ai.InitWorld(currentWorld);
                population.Add(ai);*/
            }
        }
    }

    public AIController instantiateVillager()
    {
        GameObject g = (GameObject)Instantiate(VillagerPrefab, Vector3.zero, Quaternion.identity);

        //spawn at random location
        int sizeX = Mathf.RoundToInt(Terrain.activeTerrain.terrainData.size.x);
        int sizeZ = Mathf.RoundToInt(Terrain.activeTerrain.terrainData.size.z);

        Vector3 pos = new Vector3(StaticRandom.Rand(0, sizeX), 0, StaticRandom.Rand(0, sizeZ));
        g.transform.position = new Vector3(pos.x, Terrain.activeTerrain.SampleHeight(pos), pos.z);
        //

        //name the villager with it's instance ID, so we can identify it in the console view
        g.name = "Villager [" + g.GetInstanceID() + "]";

        AIController aiControl = g.GetComponent<AIController>();

        return aiControl;
    }
}
