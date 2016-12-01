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

        public ConsumeResource(float rewardMultiplier)
        {
            this.rewardMultiplier = rewardMultiplier;
        }

        protected override void init(AIController agent)
        {
            targetResource = agent.PopResource(); //take resource from the top of inventory
            cost = 1 / float.MaxValue;

            if (targetResource != null) //if we got something
            {
                agent.state = AIController.states.running; //then continue running
            }
            else
            {
                agent.state = AIController.states.failed; //else fail
            }
        }
        protected override void running(AIController agent)
        {
            if (targetResource != null)
            {
                //health reward
                agent.AddHealth(-cost+0.25F);

                //consume
                agent.state = AIController.states.succesful;
            }
            else
            {
                //health penalty
                agent.AddHealth(-cost);

                agent.state = AIController.states.failed;
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