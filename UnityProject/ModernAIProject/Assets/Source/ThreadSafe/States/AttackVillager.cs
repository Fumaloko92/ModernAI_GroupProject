using UnityEngine;
using System.Collections;

namespace ThreadSafe
{
    public class AttackVillager : State
    {
        /*
         * Precondition: other villager available in world
         * 
         * action: goto random villager and attack it
         * 
         * succesful if: villager attacked
         */
        AIController targetVillager = null;
        public AttackVillager(float rewardMultiplier)
        {
            this.rewardMultiplier = rewardMultiplier;
        }

        protected override void init(AIController agent)
        {
            targetVillager = agent.getMyGroup().getRandomVillager(); //gets random villager

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

            agent.AddHealth(-cost - 0.1F);
        }
        protected override void running(AIController agent)
        {
                //run teleport
                if (targetVillager != null)
                {
                    agent.pos = targetVillager.pos; //teleport to villager

                    targetVillager.AddHealth(-5); //kills off the agent. maybe it should only damage it?

                    agent.state = AIController.states.succesful; //success
                }
                else
                {
                agent.AddHealth(-10000);
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
            return (REWARD_VALUE * (agent.GetHealth())) * rewardMultiplier;
        }
        public override float CostFunction()
        {
            return cost;
        }

        public override string ToString()
        {
            return "Attack Villager";
        }
    }
}
