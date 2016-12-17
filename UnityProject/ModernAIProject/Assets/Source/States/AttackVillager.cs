using UnityEngine;
using System.Collections;

public class AttackVillager : State {
    /*
     * Precondition: another villager available
     * 
     * action: goto nearest villager & destroy it
     * 
     * succesful if: villager destroyed
     */

    AIController targetVillager = null;
    public AttackVillager(float rewardMultiplier)
    {
        this.rewardMultiplier = rewardMultiplier;
    }

    protected override void init(AIController agent)
    {
        targetVillager = agent.getMyGroup().getRandomVillager(); //gets random villager

        if (targetVillager != null && targetVillager != agent) //if there is an villager in the would
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
        //run navagent
        if (targetVillager != null)
        {
            agent.myAgent.SetDestination(targetVillager.transform.position);
            if (agent.myAgent.remainingDistance < agent.transform.localScale.x * 2F) //agent.transform.localScale.x*1.5F makes sure fat villagers still can kill someone
            {
                targetVillager.AddHealth(-5); //kills off the agent. maybe it should only damage it?
                //Debug.Log(agent.gameObject.name+": Attack");
                agent.state = AIController.states.succesful; //success
            }
        }
        else
        {
            //agent.AddHealth(-10000);
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
