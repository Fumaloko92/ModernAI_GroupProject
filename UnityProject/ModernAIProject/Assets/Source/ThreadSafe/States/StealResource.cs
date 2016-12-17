using UnityEngine;
using System.Collections;
using System;

namespace ThreadSafe
{
    class StealResource : State
    {
        /*
         * Precondition: another villager available with resource in inventory
         * 
         * action: goto villager & steal resource
         * 
         * succesful if: resource gained
         */

        AIController targetVillager = null;
        public StealResource(float rewardMultiplier)
        {
            this.rewardMultiplier = rewardMultiplier;
        }

        protected override void init(AIController agent)
        {
            targetVillager = agent.getMyGroup().getRandomVillager(true); //gets random villager with resource

            if (targetVillager != null) //if there is an villager in the would
            {
                cost = Vector3.Distance(agent.pos, targetVillager.pos) / float.MaxValue;

                agent.state = AIController.states.running; //then start running
            }
            else //else fail
            {
                cost = 10000;
                agent.state = AIController.states.failed;
            }

            agent.AddHealth(healthCost);
        }
        protected override void running(AIController agent)
        {
            //run teleport
            if (targetVillager != null && targetVillager.collectedResources.Count > 0)
            {
                agent.pos = targetVillager.pos; //teleport to villager

                //steal

                Resource ress = targetVillager.collectedResources[targetVillager.collectedResources.Count - 1]; //steal resource from villager

                agent.collectedResources.Add(ress); //add resource to my own inventory
                targetVillager.collectedResources.Remove(ress); //remove from his inventory

                agent.state = AIController.states.succesful; //success
            }
            else
            {
                agent.state = AIController.states.failed; //fail
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
            targetVillager = null;
        }

        public override float RewardFunction(AIController agent) //reward function
        {
            return rewardMultiplier;
        }
        public override float CostFunction()
        {
            return cost;
        }

        public override string ToString()
        {
            return "Steal Resource";
        }
    }
}
