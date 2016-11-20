using UnityEngine;
using System.Collections;
using System;

public interface IGenotype : IComparer
{
    IGenotype GenerateRandomly();

    IGenotype Clone();

    void Mutate();
    float GetFitness();
    void SetFitness(float value);
    void RunAndEvaluate();
    NeuralNetwork<Sigmoid, Sigmoid> GetPhenotype();
}

public interface IGenotype<T, K,A,A1> : IGenotype, IEquatable<IGenotype<T, K,A,A1>>
    where T : INodeRepresentation, new() where K : IConnectionRepresentation, new()
    where A : IActivationFunction, new() where A1 : IActivationFunction, new()
{
    T NodesGenotype { get; }
    K ConnectionsGenotype { get; }
    new IGenotype<T, K,A,A1> GenerateRandomly();
    new void Mutate();
    new float GetFitness();
    new void SetFitness(float value);
    new IGenotype<T, K,A,A1> Clone();
    new void RunAndEvaluate();
    new NeuralNetwork<Sigmoid, Sigmoid> GetPhenotype();
}
