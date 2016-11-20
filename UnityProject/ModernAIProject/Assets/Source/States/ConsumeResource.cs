using UnityEngine;
using System.Collections;

public class ConsumeResource : State {
    /*
     * Precondition: resource available in inventory
     * 
     * action: consume resource
     * 
     * succesful if: resource consumed
     */
    Resource targetResource = null;
    public ConsumeResource(AIController agent, float rewardMultipler)
    {
        this.agent = agent;
        this.rewardMultiplier = rewardMultiplier;
    }

    protected override void init()
    {
        targetResource = agent.PopResource(); //take resource from the top of inventory

        if (targetResource != null) //if we got something
        {
            state = states.running; //then continue running
        }
        else
        {
            state = states.failed; //else fail
        }

        Debug.Log("[" + agent.gameObject.GetInstanceID() + "] " + "executing ConsumeResource");
    }
    protected override void running()
    {
        if(targetResource != null)
        {
            //health penalty
            agent.AddHealth(0.25F);

            //consume
            state = states.succesful;
        }
        else
        {
            //health penalty
            agent.AddHealth(-0.25F);

            state = states.failed;
        }
    }
    protected override void succesful()
    {
        Debug.Log("[" + agent.gameObject.GetInstanceID() + "] " + "succesful ConsumeResource");
    }
    protected override void failed()
    {
        Debug.Log("[" + agent.gameObject.GetInstanceID() + "] " + "failed ConsumeResource");
    }
    public override void reset()
    {
        targetResource = null;
        Debug.Log("[" + agent.gameObject.GetInstanceID() + "] " + "state reset");
        state = states.init;
    }

    public override float RewardFunction() //reward function
    {
        return (REWARD_VALUE * (agent.GetHealth())) * rewardMultiplier;
    }
}
