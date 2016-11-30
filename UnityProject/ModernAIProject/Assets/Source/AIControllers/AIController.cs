using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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
    protected QTable<State> qTable;
    protected bool initialized;
    protected int count;

    //health
    protected float maxHealth;
    protected float health;

    protected void InitializeAgent()
    {
        maxHealth = VillagerInfo.maxHealth;
        health = VillagerInfo.health;
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

    protected void Initialize()
    {
        if (count < 10 && states.Count > 0)
        {
            currentState = GetRandomState(); //start in a random state
            initialized = true;
            Debug.Log("[" + gameObject.GetInstanceID() + "] " + "state" + currentState);
        }
    }

    /// <summary>
    /// Executes the AI.
    /// Used by NEAT to execute the AI and get a fitness of its' performance
    /// </summary>
    /// <returns>time alive</returns>
    public float execute()
    {
        float timeAlive = 0;

        executor.testModeON = true;

        InitializeAgent();

        qTable = new QTable<State>(this);

        count = 0;
        Initialize();

        while (GetHealth() > 0)
        {
            timeAlive += 1 * Time.deltaTime;

            if (!initialized || currentState == null)
            {
                Initialize();
            }
            else
            {
                if (currentState.state == State.states.succesful || currentState.state == State.states.failed) //if current state ended
                {
                    Debug.Log("[" + gameObject.GetInstanceID() + "] " + "state ended: ");
                    Debug.Log("[" + gameObject.GetInstanceID() + "] " + qTable.ToString());

                    if (previousState != null) //if previous state is not null
                    {
                        if (currentState.state == State.states.succesful) //if the current state was succesful
                        {
                            //reward - add reward to connection between previous state and current state
                            qTable.UpdateQValues(previousState, currentState, 0, currentState.RewardFunction());
                        }
                        else
                        {
                            //no reward - maybe punishment?
                            qTable.UpdateQValues(previousState, currentState, 1, 0);
                        }
                    }

                    //set previous state as current state and go to next state
                    previousState = currentState;

                    while (previousState == currentState)
                    {
                        currentState = qTable.GetNextState(previousState);
                    }


                    Debug.Log("[" + gameObject.GetInstanceID() + "] " + "state" + currentState);
                    previousState.reset(); //reset state, so we can use it later
                }
                else //if state hasn't ended yet
                {
                    //Debug.Log("[" + gameObject.GetInstanceID() + "] " + "state running");
                    currentState.execute(); //run it
                }
            }
        }

        return timeAlive;
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

    public abstract float Evaluate(IDictionary<int, double> dict);
}
