using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace ThreadSafe
{
    public class AIGroup : IEvaluator
    {
        protected QTable<State> qTable;
        public QTable<State> QTable { get { return qTable; } }

        protected List<AIController> members;

        public AIGroup()
        {

        }
        public void InitWorld(World world)
        {
            foreach (AIController ai in members)
            {
                ai.mWorld = world.cleanCopy();;
            }
        }
        public void InitAIs(int AICount)
        {
            members = new List<AIController>();
            for (int i = 0; i < AICount; i++)
            {
                members.Add(new AIController(this));
            }
        }

        public float Evaluate(IDictionary<int, double> multipliers)
        {

            List<State> states = new List<State>();
            CollectResource clr;
            ConsumeResource csr;

            if (multipliers.ContainsKey(5))
                clr = new CollectResource((float)multipliers[5]);
            else
                clr = new CollectResource(0);

            if (multipliers.ContainsKey(6))
                csr = new ConsumeResource((float)multipliers[6]);
            else
                csr = new ConsumeResource(0);

            //GiveResource gr = new GiveResource();

            //states = new List<State>();
            states.Add(clr);
            states.Add(csr);
            qTable = new QTable<State>(states);
            //states.Add(gr);

            float avgFitness = 0;

            foreach (AIController ai in members)
            {
                /*float aiFit = ai.execute();
                if (avgFitness < aiFit)
                {
                    avgFitness = aiFit;
                }*/
                avgFitness += ai.execute();
            }

            avgFitness /= members.Count;

            return avgFitness;
        }
    }
}
