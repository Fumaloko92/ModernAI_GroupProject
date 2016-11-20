using UnityEngine;
using System.Collections;

public class CollectResource : State {
    /*
     * Precondition: resource available in world
     * 
     * action: goto random resource & collect it
     * 
     * succesful if: resource collected
     */
    Resource targetResource = null;
    public CollectResource(Executor executor, World world, AIController agent, float rewardMultiplier)
    {
        this.executor = executor;
        this.world = world;
        this.agent = agent;
        this.rewardMultiplier = rewardMultiplier;
    }

    protected override void init()
    {
        //health penalty
        agent.AddHealth(-0.25F);

        targetResource = world.GetRandomResource(executor.MaterialForDestination); //gets random ressource

        if(targetResource != null) //if there is a resource in the would
        {
            state = states.running; //then start running
        }
        else //else fail
        {
            state = states.failed;
        }

        Debug.Log("[" + agent.gameObject.GetInstanceID() + "] " + "executing CollectResource");
    }
    protected override void running()
    {
        if(executor.testModeON)
        {
            //run teleport
            if (targetResource != null)
            {
                agent.transform.position = targetResource.GetPositionInWorld(); //teleport to resource

                agent.collectedResources.Add(targetResource); //collect to inventory
                world.RemoveFromResourcePool(targetResource); //remove from world

                state = states.succesful; //success
            }
            else
            {
                state = states.failed; //fail
            }

        }
        else
        {
            //run navagent
            if(targetResource != null)
            {
                agent.myAgent.SetDestination(targetResource.GetPositionInWorld());
                if (agent.myAgent.remainingDistance < agent.transform.localScale.x * 1.5F) //agent.transform.localScale.x*1.5F makes sure fat villagers still can collect resources
                {
                    agent.collectedResources.Add(targetResource); //collect to inventory
                    world.RemoveFromResourcePool(targetResource); //remove from world
                    
                    state = states.succesful; //success
                }
            }
            else
            {
                state = states.failed; //Fail
            }
        }
    }
    protected override void succesful()
    {
        Debug.Log("[" + agent.gameObject.GetInstanceID() + "] " + "succesful CollectResource");
    }
    protected override void failed()
    {
        Debug.Log("[" + agent.gameObject.GetInstanceID() + "] " + "failed CollectResource");
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
