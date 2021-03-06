﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace ThreadSafe
{
    public class AIGroup : IEvaluator
    {
        public enum ExecuteMethod
        {
            oneStateAtATime,
            oneAIAtATime
        }
        public static ExecuteMethod executeMethod = ExecuteMethod.oneStateAtATime;

        protected QTable<State> qTable;
        public QTable<State> QTable { get { return qTable; } }

        public volatile List<AIController> members;

        public void InitWorld(World world)
        {
            lock(members)
            {
                foreach (AIController ai in members) 
                {
                    if(ai != null)
                    {
                        lock (world)
                        {
                            ai.mWorld = world.cleanCopy();
                        }
                    }
                }
            }
        }
        public void InitAIs(int AICount)
        {
            members = new List<AIController>();
            lock (members)
            {
                for (int i = 0; i < AICount; i++)
                {
                    members.Add(new AIController(this));
                }
            }
        }

        public AIController GetMember(int id)
        {
            return members[id];
        }
        public int GetMembersCount()
        {
            return members.Count;
        }
        public int GetAliveMembersCount()
        {
            lock(members)
            {
                int count = 0;
                foreach (AIController memb in members)
                {
                    if (memb != null && memb.GetHealth() > 0)
                    {
                        count++;
                    }
                }
                return count;
            }
        }
        public float GetFinalFitness()
        {
            float sumFitness = 0;

            foreach (AIController ai in members)
            {
                sumFitness += ai.timeAlive;
                ai.timeAlive = 0;
            }

            return sumFitness;
        }

        public float Evaluate(IDictionary<int, double> multipliers)
        {

            List<State> states = new List<State>();
            CollectResource clr;
            ConsumeResource csr;
            AttackVillager av;
            StealResource sr;

            if (multipliers.ContainsKey(5))
                clr = new CollectResource((float)multipliers[5]);
            else
                clr = new CollectResource(0);

            if (multipliers.ContainsKey(6))
                csr = new ConsumeResource((float)multipliers[6]);
            else
                csr = new ConsumeResource(0);

            if (multipliers.ContainsKey(7))
                av = new AttackVillager((float)multipliers[7]);
            else
                av = new AttackVillager(0);

            if (multipliers.ContainsKey(8))
                sr = new StealResource((float)multipliers[8]);
            else
                sr = new StealResource(0);

            //GiveResource gr = new GiveResource();

            //states = new List<State>();
            states.Add(clr);
            states.Add(csr);
            states.Add(av);
            states.Add(sr);
            qTable = new QTable<State>(states);
            //states.Add(gr);

            switch (executeMethod)
            {
                case ExecuteMethod.oneAIAtATime:
                    return FitnessOfOneAIAtATime();
                case ExecuteMethod.oneStateAtATime:
                    return FitnessOfOneStateAtATime();
                default:
                    return -1;
            }
        }

        public float EvaluateQLearning()
        {

            List<State> states = new List<State>();
            CollectResource clr;
            ConsumeResource csr;
            AttackVillager av;
            StealResource sr;

            
                clr = new CollectResource(1);

            
                csr = new ConsumeResource(1);

            
                av = new AttackVillager(1);

            
                sr = new StealResource(1);

            //GiveResource gr = new GiveResource();

            //states = new List<State>();
            states.Add(clr);
            states.Add(csr);
            states.Add(av);
            states.Add(sr);
            qTable = new QTable<State>(states);
            //states.Add(gr);

            switch (executeMethod)
            {
                case ExecuteMethod.oneAIAtATime:
                    return FitnessOfOneAIAtATime();
                case ExecuteMethod.oneStateAtATime:
                    return FitnessOfOneStateAtATime();
                default:
                    return -1;
            }
        }

        float FitnessOfOneAIAtATime()
        {
            lock(members)
            {
                float sumFitness = 0;

                foreach (AIController ai in members)
                {
                    sumFitness += ai.execute();
                }

                return sumFitness;
            }
        }
        float FitnessOfOneStateAtATime()
        {
            bool someOneAlive = true;

            while (someOneAlive)
            {
                someOneAlive = false;
                foreach (AIController ai in members)
                {
                    if (ai.GetHealth() > 0)
                    {
                        someOneAlive = true;

                        ai.timeAlive += 1f;

                        if (!ai.isInitialized() || ai.getCurState() == null)
                        {
                            ai.Initialize();
                        }
                        else
                        {
                            if (ai.state == AIController.states.succesful || ai.state == AIController.states.failed) //if current state ended
                            {
                                if (ai.getPrevState() != null) //if previous state is not null
                                {
                                    if (ai.state == AIController.states.succesful) //if the current state was succesful
                                    {
                                        //reward - add reward to connection between previous state and current state
                                        QTable.UpdateQValues(ai.getPrevState(), ai.getCurState(), ai.getCurState().CostFunction(), ai.getCurState().RewardFunction(ai));
                                    }
                                    else
                                    {
                                        //no reward - maybe punishment?
                                        QTable.UpdateQValues(ai.getPrevState(), ai.getCurState(), ai.getCurState().CostFunction(), 0);
                                    }
                                }

                                //set previous state as current state and go to next state
                                ai.setPrevState(ai.getCurState());

                                while (ai.getPrevState() == ai.getCurState())
                                {
                                    ai.setCurState(QTable.GetNextState(ai.getPrevState()));
                                }

                                ai.getPrevState().reset(); //reset state, so we can use it later
                                ai.state = AIController.states.init;
                            }
                            else //if state hasn't ended yet
                            {
                                //Debug.Log("[" + gameObject.GetInstanceID() + "] " + "state running");
                                ai.getCurState().execute(ai); //run it
                            }
                        }
                    }
                }
            }

            float sumFitness = 0;

            foreach (AIController ai in members)
            {
                sumFitness += ai.timeAlive;
                ai.timeAlive = 0;
            }

            return sumFitness;
        }

        public AIController getRandomVillager(bool checkForResource = false)
        {
            lock (members)
            {
                List<AIController> availableAgents = getAvailableVillagers(checkForResource);
                if (availableAgents.Count > 0)
                {
                    return availableAgents[StaticRandom.Rand(0, availableAgents.Count)];
                }
                else
                {
                    return null;
                }
            }
        }

        private List<AIController> getAvailableVillagers(bool checkForResource = false)
        {
            List<AIController> agentList = new List<AIController>();
            foreach (AIController agent in members)
            {
                if (agent != null && agent.GetHealth() > 0 && (!checkForResource || (agent != null && agent.collectedResources != null && agent.collectedResources.Count > 0)))
                {
                    agentList.Add(agent);
                }
            }

            return agentList;
        }



        public void InitWorld(global::World world)
        {
            throw new NotImplementedException();
        }


        public global::AIController GetMemberR(int id)
        {
            throw new NotImplementedException();
        }
    }
}
