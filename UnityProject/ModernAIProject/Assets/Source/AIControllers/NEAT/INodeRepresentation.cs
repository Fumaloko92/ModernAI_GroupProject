using UnityEngine;
using System.Collections;
using System;

public interface INodeRepresentation
{
    void GenerateRandomValue();

    int[] GetHiddenNodes();

    void RandomlyAddNode(IConnectionRepresentation connectionGenotype);

    INodeRepresentation Clone();
}


public interface INodeRepresentation<T> : IEquatable<INodeRepresentation<T>>, INodeRepresentation
{
    new void GenerateRandomValue();

    new int[] GetHiddenNodes();

    new void RandomlyAddNode(IConnectionRepresentation connectionGenotype);

    T NodeRepresentation { get; }

    new INodeRepresentation<T> Clone();
}
