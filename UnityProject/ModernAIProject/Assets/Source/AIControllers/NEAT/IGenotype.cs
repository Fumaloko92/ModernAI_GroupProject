using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public interface IGenotype : IComparer
{
    IGenotype GenerateRandomly();
    IGenotype Clone();
    IEvaluator Evaluator { get; set; }
    void Mutate();
    float GetFitness();
    void SetFitness(float value);
    void RunAndEvaluate(Dictionary<int, double> inputValues, ThreadSafe.World world);
    NeuralNetwork<Sigmoid, Sigmoid> GetPhenotype();
}

public interface IGenotype<T, K,A,A1> : IGenotype, IEquatable<IGenotype<T, K,A,A1>>
    where T : INodeRepresentation, new() where K : IConnectionRepresentation, new()
    where A : IActivationFunction, new() where A1 : IActivationFunction, new()
{
    T NodesGenotype { get; }
    K ConnectionsGenotype { get; }
    new IGenotype<T, K,A,A1> GenerateRandomly();
    new IEvaluator Evaluator { get; set; }
    new void Mutate();
    new float GetFitness();
    new void SetFitness(float value);
    new IGenotype<T, K,A,A1> Clone();
    new void RunAndEvaluate(Dictionary<int, double> inputValues, ThreadSafe.World world);
    new NeuralNetwork<Sigmoid, Sigmoid> GetPhenotype();
}
