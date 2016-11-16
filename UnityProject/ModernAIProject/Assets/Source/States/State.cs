using UnityEngine;
using System.Collections;

public abstract class State {

    protected Executor executor;
    protected World world;
    protected AIController agent;

    protected float REWARD_VALUE = 2; //reward value
    protected float rewardMultiplier; //multiplier which is trained by NEAT

    //states the state can be in!
	public enum states
    {
        init,
        running,
        succesful,
        failed
    }
    public states state = states.init;

    public void execute()
    {
        switch(state)
        {
            case states.init:
                init();
                break;
            case states.running:
                running();
                break;
            case states.succesful:
                succesful();
                break;
            case states.failed:
                failed();
                break;
            default:
                break;
        }
    }

    protected abstract void init();
    protected abstract void running();
    protected abstract void succesful();
    protected abstract void failed();

    abstract public void reset();

    public abstract float RewardFunction();
}
