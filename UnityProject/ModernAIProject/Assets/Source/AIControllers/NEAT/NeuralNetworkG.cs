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

        while(alive)
        {
            alive = false;
            
            int memberSize = Evaluator.GetMembersCount();
            for(int i = 0; i < memberSize; i++)
            {
                if(Evaluator.GetMember(i).GetHealth() > 0)
                {
                    alive = true;

                    try
                    {
                        NeuralNetwork<Sigmoid, Sigmoid> phenotype = GetPhenotype();
                        foreach (int key in inputValues.Keys)
                            phenotype.SetData(key, inputValues[key]);

                        IDictionary<int, double> stateValues = phenotype.ExecuteNetwork();

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
                            stateToRun = clr;
                            highestNum = clrNum;
                        }

                        if (csrNum > highestNum)
                        {
                            stateToRun = csr;
                            highestNum = csrNum;
                        }

                        if (avNum > highestNum)
                        {
                            stateToRun = av;
                            highestNum = avNum;
                        }

                        if (srNum > highestNum)
                        {
                            stateToRun = sr;
                            highestNum = srNum;
                        }


                        Evaluator.GetMember(i).timeAlive += 1f;

                        if (Evaluator.GetMember(i).getCurState() == null)
                        {
                            Evaluator.GetMember(i).setCurState(stateToRun);
                        }
                        else
                        {
                            while (Evaluator.GetMember(i).state != ThreadSafe.AIController.states.succesful && Evaluator.GetMember(i).state != ThreadSafe.AIController.states.failed && Evaluator.GetMember(i).getCurState() != null)
                            {
                                Evaluator.GetMember(i).getCurState().execute(Evaluator.GetMember(i));
                            }

                            Evaluator.GetMember(i).setCurState(null);
                            Evaluator.GetMember(i).state = ThreadSafe.AIController.states.init;
                        }
                    }
                    catch(Exception e)
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

    /* public IGenotype<T, K, A, A1> Crossover(IGenotype<T, K, A, A1> p)
     {
         NeuralNetworkG<T, K, A, A1, E> offspring = new NeuralNetworkG<T, K, A, A1, E>();

         int length = history.GetNumberOfVariations();
         try
         {
             for (int i = 0; i < length; i++)
             {
                 Variation v = history.GetVariationAt(i);
                 if (v.IsNode())
                 {
                     if (p.NodesGenotype.ContainsNode(v.GetNodeID()) && nodesGenotype.ContainsNode(v.GetNodeID()))
                     {
                         offspring.nodesGenotype.AddNode(v.GetNodeID());
                     }
                     else
                     {
                         if (StaticRandom.Sample() < 0.5 && (p.NodesGenotype.ContainsNode(v.GetNodeID()) || NodesGenotype.ContainsNode(v.GetNodeID())))
                         {
                             offspring.nodesGenotype.AddNode(v.GetNodeID());
                         }

                     }
                     continue;
                 }
                 if (v.IsConnection())
                 {
                     if (p.ConnectionsGenotype.ContainsConnection(v.GetFromNode(), v.GetToNode()) && connectionsGenotype.ContainsConnection(v.GetFromNode(), v.GetToNode()))
                     {
                         float weight;
                         if (StaticRandom.Sample() < 0.5)
                             weight = p.ConnectionsGenotype.GetConnectionWeight(v.GetFromNode(), v.GetToNode());
                         else
                             weight = connectionsGenotype.GetConnectionWeight(v.GetFromNode(), v.GetToNode());
                         offspring.connectionsGenotype.AddEnabledConnection(v.GetFromNode(), v.GetToNode(), weight);
                     }
                     else
                     {
                         if (offspring.nodesGenotype.ContainsNode(v.GetFromNode()) && offspring.nodesGenotype.ContainsNode(v.GetToNode()))
                         {
                             if (GetFitness() == p.GetFitness())
                             {
                                 float weight;
                                 weight = p.ConnectionsGenotype.GetConnectionWeight(v.GetFromNode(), v.GetToNode());
                                 if (weight == float.MinValue)
                                     weight = ConnectionsGenotype.GetConnectionWeight(v.GetFromNode(), v.GetToNode());
                                 if (weight != float.MinValue && StaticRandom.Sample() < 0.5)
                                 {
                                     offspring.connectionsGenotype.AddEnabledConnection(v.GetFromNode(), v.GetToNode(), weight);
                                 }
                                 continue;
                             }

                             if (GetFitness() < p.GetFitness() && connectionsGenotype.ContainsConnection(v.GetFromNode(), v.GetToNode()))
                             {
                                 float weight;
                                 weight = connectionsGenotype.GetConnectionWeight(v.GetFromNode(), v.GetToNode());
                                 offspring.connectionsGenotype.AddEnabledConnection(v.GetFromNode(), v.GetToNode(), weight);
                                 continue;
                             }

                             if (GetFitness() > p.GetFitness() && p.ConnectionsGenotype.ContainsConnection(v.GetFromNode(), v.GetToNode()))
                             {
                                 float weight;
                                 weight = p.ConnectionsGenotype.GetConnectionWeight(v.GetFromNode(), v.GetToNode());
                                 offspring.connectionsGenotype.AddEnabledConnection(v.GetFromNode(), v.GetToNode(), weight);
                             }
                         }
                     }

                 }
             }
         }
         catch (Exception e)
         {
             Debug.Log(e.ToString());
         }
         offspring.connectionsGenotype.OrganizeNodesByHiddenNodesAndConnections(offspring.nodesGenotype);
         return offspring;
     }
     */

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
