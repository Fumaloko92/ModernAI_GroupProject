using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace ThreadSafe
{
    public class QLearningCore : AIController
    {
        override public float Evaluate(IDictionary<int, double> multipliers)
        {
            
            List<State> states = new List<State>();
            CollectResource clr;
            ConsumeResource csr;

            if (multipliers.ContainsKey(5))
                clr = new CollectResource(world, this, (float)multipliers[5]);
            else
                clr = new CollectResource(world, this, 0);

            if (multipliers.ContainsKey(6))
                csr = new ConsumeResource(this, (float)multipliers[6]);
            else
                csr = new ConsumeResource(this, 0);

            //GiveResource gr = new GiveResource();

            //states = new List<State>();
            states.Add(clr);
            states.Add(csr);
            qTable = new QTable<State>(states);
            //states.Add(gr);
            return execute();
        }


    }

}