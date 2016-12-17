using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class NeuralNetworkG<T, K, A, A1, E> : IGenotype<T, K, A, A1> where T : INodeRepresentation, new() where K : IConnectionRepresentation, new()
    where A : IActivationFunction, new() where A1 : IActivationFunction, new() where E : IEvaluator, new()
{
    private T nodesGenotype;
    private K connectionsGenotype;
    private Dictionary<int, Variation> variations;
    public Dictionary<int, Variation> HistoricalVariation { get { return variations; } }
    private E evaluator;
    public IEvaluator Evaluator { get { return evaluator; } set { evaluator = (E)value; } }

    private float fitness;

    public NeuralNetworkG()
    {
        nodesGenotype = new T();
        connectionsGenotype = new K();
        evaluator = new E();
        variations = new Dictionary<int, Variation>();
    }

    public T NodesGenotype
    {
        get { return nodesGenotype; }
    }

    public K ConnectionsGenotype
    {
        get { return connectionsGenotype; }
    }

    public IGenotype<T, K, A, A1> GenerateRandomly(HistoricalVariation v)
    {

        NeuralNetworkG<T, K, A, A1, E> genotype = new NeuralNetworkG<T, K, A, A1, E>();
        try
        {
            genotype.nodesGenotype = new T();
            genotype.connectionsGenotype = new K();
            genotype.nodesGenotype.GenerateRandomValue(v, genotype.HistoricalVariation);
            genotype.connectionsGenotype.GenerateRandomValue(genotype.nodesGenotype, v, genotype.HistoricalVariation);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
        return genotype;
    }

    public void Mutate(HistoricalVariation v)
    {
        try
        {
            nodesGenotype.RandomlyAddNode(ConnectionsGenotype, v, variations);
            connectionsGenotype.RandomlyDeleteConnection(v, variations);
            connectionsGenotype.RandomlyAddConnection(v, variations);
            connectionsGenotype.RandomlyChangeWeights();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public float GetFitness()
    {
        return fitness;
    }

    public void SetFitness(float value)
    {
        fitness = value;
    }

    public int CompareTo(object other)
    {

        if (GetFitness() == ((IGenotype<T, K, A, A1>)other).GetFitness())
            return 0;
        if (GetFitness() > ((IGenotype<T, K, A, A1>)other).GetFitness())
            return 1;
        return -1;
    }

    public bool Equals(IGenotype<T, K, A, A1> other)
    {
        if (nodesGenotype.Equals(other.NodesGenotype) && connectionsGenotype.Equals(other.ConnectionsGenotype))
            return true;
        else
            return false;
    }

    public IGenotype<T, K, A, A1> Clone()
    {
        NeuralNetworkG<T, K, A, A1, E> c = new NeuralNetworkG<T, K, A, A1, E>();
        foreach (int key in variations.Keys)
            c.variations.Add(key, (Variation)variations[key].Clone());
        c.nodesGenotype = (T)nodesGenotype.Clone();
        c.connectionsGenotype = (K)connectionsGenotype.Clone();

        return c;
    }

    public void RunAndEvaluate(Dictionary<int, double> inputValues, int internalPopulation, ThreadSafe.World world)
    {
        try
        {
            if (!connectionsGenotype.IsEmpty())
            {
                NeuralNetwork<Sigmoid, Sigmoid> phenotype = GetPhenotype();
                foreach (int key in inputValues.Keys)
                    phenotype.SetData(key, inputValues[key]);

                Evaluator.InitAIs(internalPopulation);
                Evaluator.InitWorld(world);

                fitness = Evaluator.Evaluate(phenotype.ExecuteNetwork());
            }
            else
                fitness = float.MinValue;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void RunAndEvaluate(Dictionary<int, double> inputValues, int internalPopulation, World world)
    {
        try
        {
            if (!connectionsGenotype.IsEmpty())
            {
                NeuralNetwork<Sigmoid, Sigmoid> phenotype = GetPhenotype();
                foreach (int key in inputValues.Keys)
                    phenotype.SetData(key, inputValues[key]);

                Evaluator.InitAIs(internalPopulation);
                Evaluator.InitWorld(world);

                fitness = Evaluator.Evaluate(phenotype.ExecuteNetwork());
            }
            else
                fitness = float.MinValue;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
    public void RunAndEvaluateNEAT(Dictionary<int, double> inputValues, int internalPopulation, ThreadSafe.World world)
    {
        List<ThreadSafe.State> states = new List<ThreadSafe.State>();
        ThreadSafe.CollectResource clr = new ThreadSafe.CollectResource(1);
        ThreadSafe.ConsumeResource csr = new ThreadSafe.ConsumeResource(1);
        ThreadSafe.AttackVillager av = new ThreadSafe.AttackVillager(1);
        ThreadSafe.StealResource sr = new ThreadSafe.StealResource(1);
        states.Add(clr);
        states.Add(csr);
        states.Add(av);
        states.Add(sr);


        Evaluator.InitAIs(internalPopulation);
        Evaluator.InitWorld(world);

        bool alive = true;
        int counter = 0;
        while (alive)
        {
            counter++;
            alive = false;

            int memberSize = Evaluator.GetMembersCount();
            for (int i = 0; i < memberSize; i++)
            {
                ThreadSafe.AIController aic = Evaluator.GetMember(i);

                //Debug.Log("Analyzing " + i + " member");
                if (aic != null && aic.GetHealth() > 0)
                {
                    alive = true;

                    try
                    {
                        inputValues = new Dictionary<int, double>();
                        inputValues.Add(0, aic.GetHealth());
                        inputValues.Add(1, aic.maxHealth);
                        if (aic.mWorld == null)
                            inputValues.Add(2, 0);
                        else
                            inputValues.Add(2, aic.mWorld.GetResourceCount());
                        inputValues.Add(3, aic.collectedResources.Count);
                        inputValues.Add(4, Evaluator.GetAliveMembersCount());

                        NeuralNetwork<Sigmoid, Sigmoid> phenotype = GetPhenotype();
                        foreach (int key in inputValues.Keys)
                            phenotype.SetData(key, inputValues[key]);

                        Dictionary<int, double> stateValues = phenotype.ExecuteNetwork();

                        float clrNum = 0;
                        float csrNum = 0;
                        float avNum = 0;
                        float srNum = 0;

                        if (stateValues.ContainsKey(5))
                            clrNum = (float)stateValues[5];

                        if (stateValues.ContainsKey(6))
                            csrNum = (float)stateValues[6];

                        if (stateValues.ContainsKey(7))
                            avNum = (float)stateValues[7];

                        if (stateValues.ContainsKey(8))
                            srNum = (float)stateValues[8];


                        float highestNum = float.MinValue;
                        ThreadSafe.State stateToRun = clr;

                        if (clrNum > highestNum)
                        {
                            stateToRun = new ThreadSafe.CollectResource(10000);
                            highestNum = clrNum;
                        }

                        if (csrNum > highestNum)
                        {
                            stateToRun = new ThreadSafe.ConsumeResource(100000);
                            highestNum = csrNum;
                        }

                        if (avNum > highestNum)
                        {
                            stateToRun = new ThreadSafe.AttackVillager(100000);
                            highestNum = avNum;
                        }

                        if (srNum > highestNum)
                        {
                            stateToRun = new ThreadSafe.StealResource(100000);
                            highestNum = srNum;
                        }


                        aic.timeAlive += 1f;

                        aic.setCurState(stateToRun);
                        if (aic.getCurState() == null)
                            throw new Exception();
                        while (aic.state != ThreadSafe.AIController.states.succesful && aic.state != ThreadSafe.AIController.states.failed && aic.getCurState() != null)
                        {

                            lock (aic)
                            {
                                aic.getCurState().execute(aic);
                            }
                        }
                        //Debug.Log(aic.GetHealth() + " " + aic.getCurState().ToString());
                        aic.state = ThreadSafe.AIController.states.init;

                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.ToString());
                    }
                }
            }


        }

        fitness = Evaluator.GetFinalFitness();
    }

    public void RunAndEvaluateNEAT(Dictionary<int, double> inputValues, int internalPopulation, World world)
    {
        List<State> states = new List<State>();
        CollectResource clr = new CollectResource(1);
        ConsumeResource csr = new ConsumeResource(1);
        AttackVillager av = new AttackVillager(1);
        StealResource sr = new StealResource(1);
        states.Add(clr);
        states.Add(csr);
        states.Add(av);
        states.Add(sr);


        Evaluator.InitAIs(internalPopulation);
        Evaluator.InitWorld(world);

        bool alive = true;
        int counter = 0;
        while (alive)
        {
            counter++;
            alive = false;

            int memberSize = Evaluator.GetMembersCount();
            for (int i = 0; i < memberSize; i++)
            {
                AIController aic = Evaluator.GetMemberR(i);

                //Debug.Log("Analyzing " + i + " member");
                if (aic != null && aic.GetHealth() > 0)
                {
                    alive = true;

                    try
                    {
                        inputValues = new Dictionary<int, double>();
                        inputValues.Add(0, aic.GetHealth());
                        inputValues.Add(1, aic.maxHealth);
                        if (aic.mWorld == null)
                            inputValues.Add(2, 0);
                        else
                            inputValues.Add(2, aic.mWorld.GetResourceCount());
                        inputValues.Add(3, aic.collectedResources.Count);
                        inputValues.Add(4, Evaluator.GetAliveMembersCount());

                        NeuralNetwork<Sigmoid, Sigmoid> phenotype = GetPhenotype();
                        foreach (int key in inputValues.Keys)
                            phenotype.SetData(key, inputValues[key]);

                        Dictionary<int, double> stateValues = phenotype.ExecuteNetwork();

                        float clrNum = 0;
                        float csrNum = 0;
                        float avNum = 0;
                        float srNum = 0;

                        if (stateValues.ContainsKey(5))
                            clrNum = (float)stateValues[5];

                        if (stateValues.ContainsKey(6))
                            csrNum = (float)stateValues[6];

                        if (stateValues.ContainsKey(7))
                            avNum = (float)stateValues[7];

                        if (stateValues.ContainsKey(8))
                            srNum = (float)stateValues[8];


                        float highestNum = float.MinValue;
                        State stateToRun = clr;

                        if (clrNum > highestNum)
                        {
                            stateToRun = new CollectResource(10000);
                            highestNum = clrNum;
                        }

                        if (csrNum > highestNum)
                        {
                            stateToRun = new ConsumeResource(100000);
                            highestNum = csrNum;
                        }

                        if (avNum > highestNum)
                        {
                            stateToRun = new AttackVillager(100000);
                            highestNum = avNum;
                        }

                        if (srNum > highestNum)
                        {
                            stateToRun = new StealResource(100000);
                            highestNum = srNum;
                        }


                        aic.timeAlive += 1f;

                        aic.setCurState(stateToRun);
                        if (aic.getCurState() == null)
                            throw new Exception();
                        while (aic.state != AIController.states.succesful && aic.state != AIController.states.failed && aic.getCurState() != null)
                        {

                            lock (aic)
                            {
                                aic.getCurState().execute(aic);
                            }
                        }
                        //Debug.Log(aic.GetHealth() + " " + aic.getCurState().ToString());
                        aic.state = AIController.states.init;

                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.ToString());
                    }
                }
            }


        }

        fitness = Evaluator.GetFinalFitness();
    }

    public NeuralNetwork<Sigmoid, Sigmoid> GetPhenotype()
    {
        NeuralNetwork<Sigmoid, Sigmoid> phenotype = new NeuralNetwork<Sigmoid, Sigmoid>(NEAT_Static.inputNodes, nodesGenotype.GetHiddenNodes(), NEAT_Static.outputNodes, ((IConnectionRepresentation<string>)connectionsGenotype).ConnectionsRepresentation);
        return phenotype;
    }

    public int Compare(object x, object y)
    {
        if (((IGenotype<T, K, A, A1>)x).GetFitness() == ((IGenotype<T, K, A, A1>)y).GetFitness())
            return 0;
        if (((IGenotype<T, K, A, A1>)x).GetFitness() > ((IGenotype<T, K, A, A1>)y).GetFitness())
            return 1;
        return -1;
    }

    public override string ToString()
    {
        string s = "";
        s += nodesGenotype.ToString()+Environment.NewLine;
        s += connectionsGenotype.ToString()+Environment.NewLine;

        return s;
    }

    public NeuralNetwork<A, A1> GetPhenotypeFromString(string s)
    {
        string[] tokens = s.Split(Environment.NewLine.ToCharArray());
        string[] hiddens = tokens[0].Split(',');
        List<int> h = new List<int>();
        foreach (string hidden in hiddens)
            if (hidden.Length > 0)
                h.Add(int.Parse(hidden));
        return (NeuralNetwork<A, A1>)new NeuralNetwork<Sigmoid, Sigmoid>(NEAT_Static.inputNodes, h.ToArray(), NEAT_Static.outputNodes, tokens[1]);
    }

    public IGenotype<T, K, A, A1> Crossover(IGenotype<T, K, A, A1> p)
    {
        NeuralNetworkG<T, K, A, A1, E> offspring = new NeuralNetworkG<T, K, A, A1, E>();
        List<int> variation_keys = new List<int>();
        foreach (int key in HistoricalVariation.Keys)
            variation_keys.Add(key);
        foreach (int key in p.HistoricalVariation.Keys)
            if (!variation_keys.Contains(key))
                variation_keys.Add(key);
        variation_keys.Sort((el, el1) => el.CompareTo(el1));
        foreach (int variation_key in variation_keys)
        {
            if (HistoricalVariation.ContainsKey(variation_key) && p.HistoricalVariation.ContainsKey(variation_key))
            {
                if (GetFitness() > p.GetFitness())
                    offspring.AddVariation(variation_key, (Variation)HistoricalVariation[variation_key].Clone(), this, p);
                else
                    offspring.AddVariation(variation_key, (Variation)p.HistoricalVariation[variation_key].Clone(), this, p);
                continue;
            }
            if (GetFitness() == p.GetFitness())
            {
                if (StaticRandom.Sample() < 0.5)
                {
                    if (HistoricalVariation.ContainsKey(variation_key))
                        offspring.AddVariation(variation_key, (Variation)HistoricalVariation[variation_key].Clone(), this, p);
                    else
                        offspring.AddVariation(variation_key, (Variation)p.HistoricalVariation[variation_key].Clone(), this, p);
                }
                continue;
            }
            if (GetFitness() > p.GetFitness() && HistoricalVariation.ContainsKey(variation_key))
            {
                if (StaticRandom.Sample() < 0.5)
                    offspring.AddVariation(variation_key, (Variation)HistoricalVariation[variation_key].Clone(), this, p);
                continue;
            }
            if (GetFitness() < p.GetFitness() && p.HistoricalVariation.ContainsKey(variation_key))
            {
                if (StaticRandom.Sample() < 0.5)
                    offspring.AddVariation(variation_key, (Variation)p.HistoricalVariation[variation_key].Clone(), this, p);
                continue;
            }
        }
        offspring.connectionsGenotype.OrganizeNodesByHiddenNodesAndConnections(NodesGenotype);
        return offspring;
    }

    private void AddVariation(int key, Variation v, IGenotype<T, K, A, A1> p, IGenotype<T, K, A, A1> p1)
    {
        if (v.IsNode())
        {
            variations.Add(key, v);
            nodesGenotype.AddNode(v.GetNodeID());
        }
        else
        {
            bool contains_from = false, contains_to = false;
            foreach (int k in variations.Keys)
            {
                if (contains_from && contains_to)
                    break;
                if (v.GetFromNode() == variations[k].GetNodeID())
                    contains_from = true;
                if (v.GetToNode() == variations[k].GetNodeID())
                    contains_to = true;
            }
            if (contains_from && contains_to)
            {
                if (!v.IsEnabled() && StaticRandom.Sample() < 0.25)
                    v.Enable();

                variations.Add(key, v);
                if (p.ConnectionsGenotype.ContainsConnection(v.GetFromNode(), v.GetToNode()) && p1.ConnectionsGenotype.ContainsConnection(v.GetFromNode(), v.GetToNode()))
                {
                    if (p.GetFitness() > p1.GetFitness())
                        connectionsGenotype.AddConnection(v.GetFromNode(), v.GetToNode(), p.ConnectionsGenotype.GetConnectionWeight(v.GetFromNode(), v.GetToNode()), v.IsEnabled());
                    else
                        connectionsGenotype.AddConnection(v.GetFromNode(), v.GetToNode(), p1.ConnectionsGenotype.GetConnectionWeight(v.GetFromNode(), v.GetToNode()), v.IsEnabled());
                }
                else
                {
                    if (p.ConnectionsGenotype.ContainsConnection(v.GetFromNode(), v.GetToNode()))
                        connectionsGenotype.AddConnection(v.GetFromNode(), v.GetToNode(), p.ConnectionsGenotype.GetConnectionWeight(v.GetFromNode(), v.GetToNode()), v.IsEnabled());
                    else
                        connectionsGenotype.AddConnection(v.GetFromNode(), v.GetToNode(), p1.ConnectionsGenotype.GetConnectionWeight(v.GetFromNode(), v.GetToNode()), v.IsEnabled());
                }

            }
        }
    }
}
