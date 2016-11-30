using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadSafe
{
    public class QTable<T>
    {
        private List<T> states;

        //private NetworkGrid<T> statesTransitions;
        private Dictionary<T, Dictionary<T, float>> qValues;
        private Dictionary<T, int> visitedTimes;
        private float learningRate = 1;
        private float discountFactor = 0.95f;

        public QTable(List<T> states)
        {
            visitedTimes = new Dictionary<T, int>();
            this.states = new List<T>();
            foreach (T state in states)
                this.states.Add(state);
            qValues = new Dictionary<T, Dictionary<T, float>>();
        }

        //used in the start to select a random start state
        public T GetRandomState()
        {
            int rndIndex = StaticRandom.Rand(0, GetStatesCount());
            T state;
            try
            {
                state = states[rndIndex];
            }
            catch (Exception e)
            {
                string s = e.ToString();
                return default(T);
            }
            // Debug.Log("getting random state: " + state + " in " + rndIndex);
            return state;
        }
        public int GetStatesCount() //number of available states
        {
            return states.Count;
        }

        public void AddState(T state)
        {
            states.Add(state);
        }
        /* public T GetNextState(T currentState)
         {
             T to;
             //Explore if it's the firsState time thaState you visiState this state
             if (!qValues.ContainsKey(currentState))
             {
                 to = Explore(currentState);
                 UpdateVisitedTimes(to);
                 if(to != null) return to;
             }
             //ExploiState if you explored all the possible action/states from this state
             if (GetNumberOfConnectionExploredFromState(currentState) == agent.GetStatesCount())//statesTransitions.GetNodesConnectedFrom(currentState).Count)
             {
                 to = Exploit(currentState);
                 UpdateVisitedTimes(to);
                 if (to != null) return to;
             }
             //Explore if a random number between 0 and 100 is less than a number which increases the less connections from the currentState have been explored
             if (StaticRandom.Sample() < (1 - GetNumberOfConnectionExploredFromState(currentState) / agent.GetStatesCount()))
             {
                 to = Explore(currentState);
                 UpdateVisitedTimes(to);
                 if (to != null) return to;
             }
             //Explore with a random(low) chance
             if (StaticRandom.Sample() < 0.2)
             {
                 to = Explore(currentState);
                 UpdateVisitedTimes(to);
                 if (to != null) return to;
             }
             //Else exploit
             to = Exploit(currentState);
             UpdateVisitedTimes(to);
             return to;
         }*/

        public T GetNextState(T currentState)
        {
            T to;
            if (!qValues.ContainsKey(currentState))
            {
                to = Explore(currentState);
                UpdateVisitedTimes(to);
                if (to != null) return to;
            }
            if (StaticRandom.Sample() < 0.3)
            {
                to = Explore(currentState);
                UpdateVisitedTimes(to);
                if (to != null) return to;
            }
            else
            {
                to = Exploit(currentState);
                UpdateVisitedTimes(to);
                return to;
            }
            return to;
        }

        private int GetNumberOfConnectionExploredFromState(T from)
        {
            if (!qValues.ContainsKey(from))
                return 0;
            return qValues[from].Count;
        }

        private void UpdateVisitedTimes(T to)
        {
            if (visitedTimes.ContainsKey(to))
                visitedTimes[to] += 1;
            else
                visitedTimes[to] = 1;
        }

        private T Explore(T currentState)
        {
            //get all states
            List<T> l = new List<T>(states);// statesTransitions.GetNodesConnectedFrom(currentState);

            List<T> toVisit = new List<T>(states);
            if (!qValues.ContainsKey(currentState))
            {
                //if current state doesn't exist in the q table, then choose a random state as next state
                //statesTransitions.GetNodesConnectedFrom(currentState));
                return toVisit[StaticRandom.Rand(0, toVisit.Count)];
            }
            List<T> list = new List<T>(states);
            //otherwise choose random of states that hasn't been visited yet
            foreach (T node in list)//statesTransitions.GetNodesConnectedFrom(currentState))
                if (!qValues[currentState].ContainsKey(node))
                    toVisit.Add(node);

            return l[StaticRandom.Rand(0, l.Count)];
        }

        private T Exploit(T currentState)
        {
            float maxValue = float.MinValue;
            T best_choice = default(T);
            foreach (T to in qValues[currentState].Keys)
                if (qValues[currentState][to] > maxValue)
                {
                    maxValue = qValues[currentState][to];
                    best_choice = to;
                }
            return best_choice;
        }

        public float GetCostFromStateToState(T from, T to)
        {
            if (qValues.ContainsKey(from) && qValues[from].ContainsKey(to))
                return qValues[from][to];
            else
                return float.MinValue;
        }

        public float UpdateQValues(T from, T to, float cost, float reward)
        {
            Dictionary<T, float> costs;
            float old;
            if (qValues.ContainsKey(from))
            {
                costs = qValues[from];
                if (costs.ContainsKey(to))
                    old = costs[to];
                else
                    old = 0;
            }
            else
            {
                costs = new Dictionary<T, float>();
                old = 0;
            }
            float deltaQ = 0;
            float updatedLearningRate = learningRate;
            if (visitedTimes.ContainsKey(to))
                updatedLearningRate /= visitedTimes[to];
            deltaQ = updatedLearningRate * (reward + discountFactor * GetBestCostFromState(to) - cost);
            costs[to] = old + deltaQ;
            qValues[from] = costs;
            return costs[to];
        }

        private float GetBestCostFromState(T from)
        {
            if (!qValues.ContainsKey(from))
                return 0;

            float max = float.MinValue;
            foreach (T to in qValues[from].Keys)
                if (qValues[from][to] > max)
                {
                    max = qValues[from][to];
                }
            return max;
        }

        override public string ToString()
        {
            string s = "";
            foreach (T from in qValues.Keys)
            {
                string con = from + " => ";
                foreach (T to in qValues[from].Keys)
                {
                    s += con + to + " : " + qValues[from][to] + System.Environment.NewLine;
                }
            }

            return s;
        }
    }
}
