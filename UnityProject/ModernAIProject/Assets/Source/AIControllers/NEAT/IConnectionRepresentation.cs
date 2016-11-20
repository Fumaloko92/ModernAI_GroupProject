using UnityEngine;
using System.Collections;
using System;

public interface IConnectionRepresentation
{
    void RandomlyChangeWeights();

    void RandomlyAddConnection();

    void GenerateRandomValue(INodeRepresentation nodes);

    void UpdateConnections(INodeRepresentation nodes);

    IConnectionRepresentation Clone();
}

public interface IConnectionRepresentation<T> : IEquatable<IConnectionRepresentation<T>>, IConnectionRepresentation
{
    new void GenerateRandomValue(INodeRepresentation nodes);

    new void UpdateConnections(INodeRepresentation nodes);

    new void RandomlyChangeWeights();

    new void RandomlyAddConnection();

    T ConnectionsRepresentation { get; }

    new IConnectionRepresentation<T> Clone();
}
