using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class AIController : MonoBehaviour {
    [HideInInspector] public NavMeshAgent myAgent;
    protected World world;
    [HideInInspector] public List<Resource> collectedResources;
    protected Executor executor;

    //states
    protected State previousState = null;
    protected State currentState = null;

    protected List<State> states; //list of possible states

    //health
    protected float maxHealth = 1;
    protected float health = 1;

    protected void InitializeAgent()
    {
        myAgent = GetComponent<NavMeshAgent>();
        collectedResources = new List<Resource>();

        // states - last parameter is the reward multiplier and is trained by NEAT
        AttackVillager av = new AttackVillager(executor,this, 1.5F);
        CollectResource clr = new CollectResource(executor, world, this, 1.5F);
        ConsumeResource csr = new ConsumeResource(this, 1.5F);
        //GiveResource gr = new GiveResource();
        StealResource sr = new StealResource(executor, this, 1.5F);

        states = new List<State>();
        states.Add(av);
        states.Add(clr);
        states.Add(csr);
        //states.Add(gr);
        states.Add(sr);
    }



    public void InitWorld(World world)
    {
        this.world = world;
    }

    //used in the start to select a random start state
    public State GetRandomState()
    {
        int rndIndex = StaticRandom.Rand(0, GetStatesCount());
        State state = states[rndIndex];
       // Debug.Log("getting random state: " + state + " in " + rndIndex);
        return state;
    }
    public int GetStatesCount() //number of available states
    {
        return states.Count;
    }
    public List<State> GetStates() //gets the states
    {
        return states;
    }

    public Vector3 GetPositionInWorld()
    {
        Vector3 pos = transform.position;
        pos.y = Terrain.activeTerrain.SampleHeight(pos);

        return pos;
    }
	
    //removes last resource from inventory and returns it
    public Resource PopResource()
    {
        Resource ress = null;

        if(collectedResources.Count > 0)
        {
            ress = collectedResources[collectedResources.Count - 1];
            collectedResources.Remove(collectedResources[collectedResources.Count - 1]);
        }

        return ress;
    }

    //health
    public void AddHealth(float value)
    {
        health += value;
        if(health > maxHealth)
        {
            health = maxHealth;
        }
        if(health < 0)
        {
            //die?

            health = 0;
        }
    }
    public float GetHealth()
    {
        return health;
    }
}
