using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class QLearningCore : AIController
{
    private QTable<State> qTable;
    private bool initialized;
    private int count;

    void Awake()
    {
        initialized = false;
        executor = GameObject.FindGameObjectWithTag("Executor").GetComponent<Executor>();
    }

    void Start()
    {
        InitializeAgent();

        qTable = new QTable<State>(this);

        count = 0;
        Initialize();
    }

    private void Initialize()
    {
        if (count < 10 && states.Count > 0)
        {
            currentState = GetRandomState(); //start in a random state
            initialized = true;
            Debug.Log("["+gameObject.GetInstanceID()+"] "+"state" + currentState);
        }
    }

    void Update()
    {
        
        if(!initialized || currentState == null)
        {
            Initialize();
        }
        else
        {
            if(currentState.state == State.states.succesful || currentState.state == State.states.failed) //if current state ended
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

                while(previousState == currentState)
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
