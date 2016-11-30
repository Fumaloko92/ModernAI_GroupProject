using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace ThreadSafe
{
    abstract public class AIController : IEvaluator
    {
        protected World world;
        public List<Resource> collectedResources = new List<Resource>();
        public Vector3 pos;

        //states
        protected State previousState = null;
        protected State currentState = null;

      

        protected QTable<State> qTable;
        protected bool initialized;
        protected int count;

        //health
        protected float maxHealth = 1;
        protected float health = 1;

        protected void InitializeAgent()
        {
        }



        public void InitWorld(World world)
        {
            this.world = world.cleanCopy();
        }

        //removes last resource from inventory and returns it
        public Resource PopResource()
        {
            Resource ress = null;

            if (collectedResources.Count > 0)
            {
                ress = collectedResources[collectedResources.Count - 1];
                collectedResources.Remove(collectedResources[collectedResources.Count - 1]);
            }

            return ress;
        }

        protected void Initialize()
        {
            if (count < 10 && qTable.GetStatesCount() > 0)
            {
                currentState = qTable.GetRandomState(); //start in a random state
                initialized = true;

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

            InitializeAgent();

            

            count = 0;
            Initialize();

            while (GetHealth() > 0)
            {
                timeAlive+=1f;

                if (!initialized || currentState == null)
                {
                    Initialize();
                }
                else
                {
                    if (currentState.state == State.states.succesful || currentState.state == State.states.failed) //if current state ended
                    {


                        if (previousState != null) //if previous state is not null
                        {
                            if (currentState.state == State.states.succesful) //if the current state was succesful
                            {
                                //reward - add reward to connection between previous state and current state
                                qTable.UpdateQValues(previousState, currentState, currentState.CostFunction(), currentState.RewardFunction());
                            }
                            else
                            {
                                //no reward - maybe punishment?
                                qTable.UpdateQValues(previousState, currentState, currentState.CostFunction(), 0);
                            }
                        }

                        //set previous state as current state and go to next state
                        previousState = currentState;

                        while (previousState == currentState)
                        {
                            currentState = qTable.GetNextState(previousState);
                        }



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
            if (health > maxHealth)
            {
                health = maxHealth;
            }
            if (health < 0)
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

}