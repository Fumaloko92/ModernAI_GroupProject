using UnityEngine;
using System.Collections;
namespace ThreadSafe
{
    public class ConsumeResource : State
    {
        /*
         * Precondition: resource available in inventory
         * 
         * action: consume resource
         * 
         * succesful if: resource consumed
         */
        Resource targetResource = null;

        public ConsumeResource(AIController agent, float rewardMultiplier)
        {
            this.agent = agent;
            this.rewardMultiplier = rewardMultiplier;
        }

        protected override void init()
        {
            targetResource = agent.PopResource(); //take resource from the top of inventory
            cost = 1 / float.MaxValue;

            if (targetResource != null) //if we got something
            {
                state = states.running; //then continue running
            }
            else
            {
                state = states.failed; //else fail
            }
        }
        protected override void running()
        {
            if (targetResource != null)
            {
                //health reward
                agent.AddHealth(-cost+0.25F);

                //consume
                state = states.succesful;
            }
            else
            {
                //health penalty
                agent.AddHealth(-cost);

                state = states.failed;
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

        public override float RewardFunction() //reward function
        {
            return (REWARD_VALUE * (agent.GetHealth())) * rewardMultiplier;
        }
        public override float CostFunction()
        {
            return cost;
        }
    }
}