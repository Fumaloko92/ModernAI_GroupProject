using UnityEngine;
using System.Collections;
using System;

public interface IConnectionRepresentation
{
    void RandomlyChangeWeights();

    void RandomlyAddConnection();

    void GenerateRandomValue(INodeRepresentation nodes);

    void UpdateConnections(INodeRepresentation nodes);

    void AddSplitConnection(int from, int mid, int to, float weight);
    string GetRandomConnection();
    void RandomlyDeleteConnection();
    IConnectionRepresentation Clone();

    bool IsEmpty();
}

public interface IConnectionRepresentation<T> : IEquatable<IConnectionRepresentation<T>>, IConnectionRepresentation
{
    new void GenerateRandomValue(INodeRepresentation nodes);

    new void UpdateConnections(INodeRepresentation nodes);

    new void RandomlyChangeWeights();

    new void RandomlyAddConnection();

    new void RandomlyDeleteConnection();
    T ConnectionsRepresentation { get; }

    new void AddSplitConnection(int from, int mid, int to, float weight);

    new string GetRandomConnection();

    new IConnectionRepresentation<T> Clone();

    new bool IsEmpty();
}
