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
    public AttackVillager(Executor executor, AIController agent, float rewardMultiplier)
    {
        this.executor = executor;
        this.agent = agent;
        this.rewardMultiplier = rewardMultiplier;
    }

    protected override void init()
    {
        //health penalty
        agent.AddHealth(-0.25F);

        //get a random villager in the world
        targetVillager = executor.GetRandomVillager(agent);

        //if there is villagers then continue running the state
        if (targetVillager != null)
        {
            state = states.running;
        }
        else //otherwise fail it
        {
            state = states.failed;
        }

        Debug.Log("[" + agent.gameObject.GetInstanceID() + "] " + "executing AttackVillager");
    }
    protected override void running()
    {
        if(executor.testModeON)
        {
            //run teleport
            if (targetVillager != null)
            {
                agent.transform.position = targetVillager.GetPositionInWorld(); //teleport to target villager

                executor.KillVillager(targetVillager); //kill it

                state = states.succesful; //success
            }
            else
            {
                state = states.failed; //failed
            }

        }
        else
        {
            //run navagent
            if (targetVillager != null)
            {
                agent.myAgent.SetDestination(targetVillager.GetPositionInWorld());
                if (agent.myAgent.remainingDistance < agent.transform.localScale.x * 1.5F) //agent.transform.localScale.x*1.5F makes sure fat villagers still can kill someone
                {
                    executor.KillVillager(targetVillager); //kill villager
                    
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
        Debug.Log("[" + agent.gameObject.GetInstanceID() + "] " + "succesful AttackVillager");
    }
    protected override void failed()
    {
        Debug.Log("[" + agent.gameObject.GetInstanceID() + "] " + "failed AttackVillager");
    }
    public override void reset() //resets values, so the state is ready for another use
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
