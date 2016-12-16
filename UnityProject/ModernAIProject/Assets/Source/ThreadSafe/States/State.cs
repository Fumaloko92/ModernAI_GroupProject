﻿using UnityEngine;
using System.Collections;
namespace ThreadSafe
{
    public abstract class State
    {
        protected float REWARD_VALUE = 2; //reward value
        protected float rewardMultiplier; //multiplier which is trained by NEAT
        protected float cost;

        public void execute(AIController agent)
        {
            switch (agent.state)
            {
                case AIController.states.init:
                    init(agent);
                    break;
                case AIController.states.running:
                    running(agent);
                    break;
                case AIController.states.succesful:
                    succesful();
                    break;
                case AIController.states.failed:
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

        public abstract override string ToString();
    }
}