using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public interface IConnectionRepresentation
{
    void RandomlyChangeWeights();

    void RandomlyAddConnection(HistoricalVariation v, Dictionary<int, Variation> v1);

    void GenerateRandomValue(INodeRepresentation nodes, HistoricalVariation v, Dictionary<int, Variation> v1);

    void UpdateConnections(INodeRepresentation nodes);

    void AddSplitConnection(int from, int mid, int to, float weight);
    string GetRandomConnection();
    void RandomlyDeleteConnection(HistoricalVariation v, Dictionary<int, Variation> v1);
    IConnectionRepresentation Clone();
    void AddConnection(int from, int to, float weight, bool enabled);
    float GetConnectionWeight(int from, int to);
    bool ContainsConnection(int from, int to);
    bool IsEmpty();
    void OrganizeNodesByHiddenNodesAndConnections(INodeRepresentation nodes);
}

public interface IConnectionRepresentation<T> : IEquatable<IConnectionRepresentation<T>>, IConnectionRepresentation
{
    new void GenerateRandomValue(INodeRepresentation nodes, HistoricalVariation v, Dictionary<int, Variation> v1);

    new void UpdateConnections(INodeRepresentation nodes);

    new void RandomlyChangeWeights();

    new void RandomlyAddConnection(HistoricalVariation v, Dictionary<int, Variation> v1);

    new void RandomlyDeleteConnection(HistoricalVariation v, Dictionary<int, Variation> v1);
    T ConnectionsRepresentation { get; }

    new void AddSplitConnection(int from, int mid, int to, float weight);

    new string GetRandomConnection();

    new bool ContainsConnection(int from, int to);
    new IConnectionRepresentation<T> Clone();

    new void AddConnection(int from, int to, float weight, bool enabled);

    new float GetConnectionWeight(int from, int to);
    new bool IsEmpty();

    new void OrganizeNodesByHiddenNodesAndConnections(INodeRepresentation nodes);
}
