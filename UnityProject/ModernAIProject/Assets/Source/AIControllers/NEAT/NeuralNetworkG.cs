using UnityEngine;
using System.Collections;
using System;

public class NeuralNetworkG<T, K, A, A1, E> : IGenotype where T : INodeRepresentation, new() where K : IConnectionRepresentation, new()
    where A : IActivationFunction, new() where A1 : IActivationFunction, new() where E : IEvaluator, new()
{
    private T nodesGenotype;
    private K connectionsGenotype;
    private E evaluator;
    public IEvaluator Evaluator { get { return evaluator; } set { evaluator = (E)value; } }

    private float fitness;

    public NeuralNetworkG()
    {

    }

    public T NodesGenotype
    {
        get { return nodesGenotype; }
    }

    public K ConnectionsGenotype
    {
        get { return connectionsGenotype; }
    }

    public IGenotype GenerateRandomly()
    {

        NeuralNetworkG<T, K, A, A1,E> genotype = new NeuralNetworkG<T, K, A, A1,E>();
        try
        {
            genotype.nodesGenotype = new T();
            genotype.connectionsGenotype = new K();
            genotype.nodesGenotype.GenerateRandomValue();
            genotype.connectionsGenotype.GenerateRandomValue(genotype.nodesGenotype);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
        return genotype;
    }

    public void Mutate()
    {
        try
        {
            nodesGenotype.RandomlyAddNode();
            nodesGenotype.RandomlyDeleteNode();
            connectionsGenotype.RandomlyAddConnection();
            connectionsGenotype.RandomlyChangeWeights();
        }
        catch(Exception e)
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

    new public bool Equals(object other)
    {
        if (nodesGenotype.Equals(((IGenotype<T, K, A, A1>)other).NodesGenotype) && connectionsGenotype.Equals(((IGenotype<T, K, A, A1>)other).ConnectionsGenotype))
            return true;
        else
            return false;
    }

    public IGenotype Clone()
    {
        NeuralNetworkG<T, K, A, A1,E> c = new NeuralNetworkG<T, K, A, A1,E>();
        c.nodesGenotype = (T)nodesGenotype.Clone();
        c.connectionsGenotype = (K)connectionsGenotype.Clone();
        return c;
    }

    public void RunAndEvaluate()
    {
        try {
            NeuralNetwork<Sigmoid, Sigmoid> phenotype = GetPhenotype();
            fitness = Evaluator.Evaluate(phenotype.ExecuteNetwork());
        }catch(Exception e)
        {
            Debug.Log(e.ToString());
        }
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
}
