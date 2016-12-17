using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public interface IGenotype<T, K,A,A1> : IEquatable<IGenotype<T, K,A,A1>>
    where T : INodeRepresentation, new() where K : IConnectionRepresentation, new()
    where A : IActivationFunction, new() where A1 : IActivationFunction, new()
{
    T NodesGenotype { get; }
    K ConnectionsGenotype { get; }
    Dictionary<int,Variation> HistoricalVariation { get; }
    IGenotype<T, K,A,A1> GenerateRandomly(HistoricalVariation v);

    IGenotype<T, K, A, A1> Crossover(IGenotype<T, K, A, A1> p);
    IEvaluator Evaluator { get; set; }
    void Mutate(HistoricalVariation v);
    float GetFitness();
    void SetFitness(float value);

    IGenotype<T, K,A,A1> Clone();
    void RunAndEvaluate(Dictionary<int, double> inputValues,int internalPopulation, ThreadSafe.World world);
    void RunAndEvaluateNEAT(Dictionary<int, double> inputValues, int internalPopulation, ThreadSafe.World world);
    void RunAndEvaluateNEAT(Dictionary<int, double> inputValues, int internalPopulation, World world);
    NeuralNetwork<Sigmoid, Sigmoid> GetPhenotype();
    string ToString();
    NeuralNetwork<A,A1> GetPhenotypeFromString(string s);
}
