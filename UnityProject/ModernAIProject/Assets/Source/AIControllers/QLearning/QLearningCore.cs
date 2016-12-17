using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class QLearningCore// : AIController
{/*
    void Awake()
    {
        initialized = false;
        executor = GameObject.FindGameObjectWithTag("Executor").GetComponent<Executor>();
    }

    void Start()
    {
        if (!executor.runInBackground)
        {
            InitializeAgent();

            qTable = new QTable<State>(this);

            count = 0;
            Initialize();
        }
    }

    void Update()
    {
        if (!executor.runInBackground)
        {
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
    }
    override public float Evaluate(IDictionary<int, double> multipliers)
    {
        AttackVillager av;
        CollectResource clr;
        ConsumeResource csr;
        StealResource sr;
        if (multipliers.ContainsKey(4))
            av = new AttackVillager(executor, this, (float)multipliers[4]);
        else
            av = new AttackVillager(executor, this, 0);

        if (multipliers.ContainsKey(5))
            clr = new CollectResource(executor, world, this, (float)multipliers[5]);
        else
            clr = new CollectResource(executor, world, this, 0);

        if (multipliers.ContainsKey(6))
            csr = new ConsumeResource(this, (float)multipliers[6]);
        else
            csr = new ConsumeResource(this, 0);
        //GiveResource gr = new GiveResource();
        if (multipliers.ContainsKey(7))
            sr = new StealResource(executor, this, (float)multipliers[7]);
        else
            sr = new StealResource(executor, this, 0);

        states = new List<State>();
        states.Add(av);
        states.Add(clr);
        states.Add(csr);
        //states.Add(gr);
        states.Add(sr);
        return execute();
    }*/
}
