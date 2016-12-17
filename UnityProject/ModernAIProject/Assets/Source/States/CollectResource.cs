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
    public CollectResource(float rewardMultiplier)
    {
        this.rewardMultiplier = rewardMultiplier;
    }

    protected override void init(AIController agent)
    {
        targetResource = agent.mWorld.GetRandomResource(); //gets random ressource


        if (targetResource != null) //if there is a resource in the would
        {
            cost = Vector3.Distance(agent.pos, targetResource.transform.position) / float.MaxValue;
            Debug.Log("cost:" + cost);

            agent.state = AIController.states.running; //then start running
        }
        else //else fail
        {
            cost = 10000;
            //Debug.Log("fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff");
            agent.state = AIController.states.failed;
        }

        agent.AddHealth(healthCost);
    }
    protected override void running(AIController agent)
    {
        //run navagent
        if(targetResource != null)
        {
            agent.myAgent.SetDestination(targetResource.GetPositionInWorld());
            if (agent.myAgent.remainingDistance < agent.transform.localScale.x * 1.5F) //agent.transform.localScale.x*1.5F makes sure fat villagers still can collect resources
            {
                agent.collectedResources.Add(targetResource); //collect to inventory
                agent.mWorld.RemoveFromResourcePool(targetResource); //remove from world

                Debug.Log(agent.gameObject.name + ": Collect");
                agent.state = AIController.states.succesful; //success
            }
        }
        else
        {
            //agent.AddHealth(-10000);
            //Debug.Log("wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww");
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

    public override string ToString()
    {
        return "Collect Resource";
    }
}
