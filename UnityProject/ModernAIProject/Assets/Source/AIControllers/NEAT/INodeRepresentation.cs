using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public interface INodeRepresentation
{
    void GenerateRandomValue(HistoricalVariation v, Dictionary<int, Variation> v1);

    int[] GetHiddenNodes();

    void RandomlyAddNode(IConnectionRepresentation connectionGenotype, HistoricalVariation v, Dictionary<int, Variation> v1);

    bool ContainsNode(int index);

    void AddNode(int index);
    INodeRepresentation Clone();

    string ToString();
}


public interface INodeRepresentation<T> : IEquatable<INodeRepresentation<T>>, INodeRepresentation
{
    new void GenerateRandomValue(HistoricalVariation v, Dictionary<int, Variation> v1);

    new int[] GetHiddenNodes();

    new bool ContainsNode(int index);
    new void RandomlyAddNode(IConnectionRepresentation connectionGenotype, HistoricalVariation v, Dictionary<int, Variation> v1);

    new void AddNode(int index);
    T NodeRepresentation { get; }

    new INodeRepresentation<T> Clone();

    new string ToString();
}
