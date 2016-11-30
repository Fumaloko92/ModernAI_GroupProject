using UnityEngine;
using System.Collections;
namespace ThreadSafe
{
    public abstract class State
    {
        protected float REWARD_VALUE = 2; //reward value
        protected float rewardMultiplier; //multiplier which is trained by NEAT
        protected float cost;

        //states the state can be in!
        public enum states
        {
            init,
            running,
            succesful,
            failed
        }
        public states state = states.init;

        public void execute(AIController agent)
        {
            switch (state)
            {
                case states.init:
                    init(agent);
                    break;
                case states.running:
                    running(agent);
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

        protected abstract void init(AIController agent);
        protected abstract void running(AIController agent);
        protected abstract void succesful();
        protected abstract void failed();

        abstract public void reset();

        public abstract float RewardFunction(AIController agent);
        public abstract float CostFunction();
    }
}