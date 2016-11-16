using UnityEngine;
using System.Collections;

public class StealResource : State {
    /*
     * Precondition: another villager available with resource in inventory
     * 
     * action: goto villager & steal resource
     * 
     * succesful if: resource gained
     */

    AIController targetVillager = null;
    public StealResource(Executor executor, AIController agent, float rewardMultipler)
    {
        this.executor = executor;
        this.agent = agent;
        this.rewardMultiplier = rewardMultiplier;
    }

    protected override void init()
    {
        //health penalty
        agent.AddHealth(-0.25F);

        targetVillager = executor.GetVillagerWithResource(agent); //get a villager with some resource in inventory

        if (targetVillager != null) //if there is a such thing
        {
            state = states.running; //then continue
        }
        else
        {
            state = states.failed; //else fail
        }

        Debug.Log("[" + agent.gameObject.GetInstanceID() + "] " + "executing StealResource");
    }
    protected override void running()
    {
        if(executor.testModeON)
        {
            //run teleport
            if (targetVillager != null && targetVillager.collectedResources.Count > 0) //make sure the villager is still alive and got my resource
            {
                agent.transform.position = targetVillager.GetPositionInWorld(); //teleport to villager

                Resource ress = targetVillager.collectedResources[targetVillager.collectedResources.Count - 1]; //steal resource from villager

                agent.collectedResources.Add(ress); //add resource to my own inventory
                targetVillager.collectedResources.Remove(ress); //remove from his inventory

                state = states.succesful; //success
            }
            else
            {
                state = states.failed; // failed
            }

        }
        else
        {
            //run navagent
            if (targetVillager != null && targetVillager.collectedResources.Count > 0) //make sure the villager is still alive and got a resource
            {
                agent.myAgent.SetDestination(targetVillager.GetPositionInWorld());
                if (agent.myAgent.remainingDistance < agent.transform.localScale.x * 1.5F) // agent.transform.localScale.x * 1.5F makes sure fat villagers is also able to steal
                {
                    Resource ress = targetVillager.collectedResources[targetVillager.collectedResources.Count - 1]; //get resource from target

                    agent.collectedResources.Add(ress); //add to my inventory
                    targetVillager.collectedResources.Remove(ress); //Remove from his inventory
                    
                    state = states.succesful; //success
                }
            }
            else
            {
                state = states.failed; //faild
            }
        }
    }
    protected override void succesful()
    {
        Debug.Log("[" + agent.gameObject.GetInstanceID() + "] " + "succesful StealResource");
    }
    protected override void failed()
    {
        Debug.Log("[" + agent.gameObject.GetInstanceID() + "] " + "failed StealResource");
    }
    public override void reset()
    {
        targetVillager = null;
        Debug.Log("[" + agent.gameObject.GetInstanceID() + "] " + "state reset");
        state = states.init;
    }

    public override float RewardFunction() //reward function
    {
        return (REWARD_VALUE * (agent.GetHealth())) * rewardMultiplier;
    }
}
