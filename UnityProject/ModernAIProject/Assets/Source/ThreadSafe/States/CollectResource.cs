﻿using UnityEngine;
using System.Collections;
using System;

namespace ThreadSafe
{
    public class CollectResource : State
    {
        /*
         * Precondition: resource available in world
         * 
         * action: goto random resource & collect it
         * 
         * succesful if: resource collected
         */
        Resource targetResource = null;
        public CollectResource(float rewardMultiplier)
        {
            this.rewardMultiplier = rewardMultiplier;
        }

        protected override void init(AIController agent)
        {
            

            targetResource = agent.mWorld.GetRandomResource(); //gets random ressource



            if (targetResource != null) //if there is a resource in the would
            {
                cost = Vector3.Distance(agent.pos, targetResource.GetPosition()) / float.MaxValue;


                state = states.running; //then start running
            }
            else //else fail
            {
                cost = 10000;
                state = states.failed;
            }

            agent.AddHealth(-cost);
        }
        protected override void running(AIController agent)
        {
                //run teleport
                if (targetResource != null)
                {
                    agent.pos = targetResource.GetPosition(); //teleport to resource

                    agent.collectedResources.Add(targetResource); //collect to inventory
                    agent.mWorld.RemoveFromResourcePool(targetResource); //remove from world

                    state = states.succesful; //success
                }
                else
                {
                    state = states.failed; //fail
                }
        }
        protected override void succesful()
        {
        }
        protected override void failed()
        {
        }
        public override void reset()
        {
            targetResource = null;
            state = states.init;
        }

        public override float RewardFunction(AIController agent) //reward function
        {
            return (REWARD_VALUE * (agent.GetHealth())) * rewardMultiplier;
        }
        public override float CostFunction()
        {
            return cost;
        }
    }
}