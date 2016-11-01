using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QTable<T>
{
    private NetworkGrid<T> statesTransitions;
    private Dictionary<T, Dictionary<T, float>> qValues;
    private Dictionary<T, int> visitedTimes;
    private float learningRate = 1;
    private float discountFactor = 0.95f;

    public QTable(T[,] matrix)
    {
        visitedTimes = new Dictionary<T, int>();
        statesTransitions = new NetworkGrid<T>(matrix);
        qValues = new Dictionary<T, Dictionary<T, float>>();
    }

    public T GetRandomState()
    {
        return statesTransitions.GetRandomNodeContent();
    }

    public T GetNextState(T currentState)
    {
        T to;
        //Explore if it's the first time that you visit this state
        if (!qValues.ContainsKey(currentState))
        {
            to = Explore(currentState);
            UpdateVisitedTimes(to);
            return to;
        }
        //Exploit if you explored all the possible action/states from this state
        if (GetNumberOfConnectionExploredFromState(currentState) == statesTransitions.GetNodesConnectedFrom(currentState).Count)
        {
            to = Exploit(currentState);
            UpdateVisitedTimes(to);
            return to;
        }
        //Explore if a random number between 0 and 100 is less than a number which increases the less connections from the currentState have been explored
        if (StaticRandom.Rand(0, 10000) % 101 < (1 - GetNumberOfConnectionExploredFromState(currentState) / statesTransitions.GetNodesConnectedFrom(currentState).Count) * 100)
        {
            to = Explore(currentState);
            UpdateVisitedTimes(to);
            return to;
        }
        //Explore with a random(low) chance
        if (StaticRandom.Rand(0, 10000) % 101 < 20)
        {
            to = Explore(currentState);
            UpdateVisitedTimes(to);
            return to;
        }
        //Else exploit
        to = Exploit(currentState);
        UpdateVisitedTimes(to);
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
        List<T> l = statesTransitions.GetNodesConnectedFrom(currentState);
        List<T> toVisit = new List<T>();
        if (!qValues.ContainsKey(currentState))
        {
            toVisit = new List<T>(statesTransitions.GetNodesConnectedFrom(currentState));
            return toVisit[StaticRandom.Rand(0, toVisit.Count)];
        }
        foreach (T node in statesTransitions.GetNodesConnectedFrom(currentState))
            if (!qValues[currentState].ContainsKey(node))
                toVisit.Add(node);

        return toVisit[StaticRandom.Rand(0, toVisit.Count)];
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
