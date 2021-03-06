﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class NetworkGrid<T>
{
    private Dictionary<int, NetworkNode<T>> nodes;

    public T GetRandomNodeContent()
    {
        int index = StaticRandom.Rand(0, nodes.Count);
        int i = 0;
        foreach(int key in nodes.Keys)
        {
            if (i == index)
                return nodes[key].Value;
            i++;
        }
        return default(T);
    }

    public NetworkGrid()
    {
        nodes = new Dictionary<int, NetworkNode<T>>();
    }

    public NetworkGrid(T[,] matrix)
    {
        nodes = new Dictionary<int, NetworkNode<T>>();

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int k = 0; k < matrix.GetLength(1); k++)
            {
                NetworkNode<T> node = new NetworkNode<T>(matrix[i, k]);

                for (int i1 = i - 1; i1 < i + 2; i1++)
                {
                    for (int k1 = k - 1; k1 < k + 2; k1++)
                    {
                        if (i1 >= 0 && i1 < matrix.GetLength(0) && k1 >= 0 && k1 < matrix.GetLength(1) && !matrix[i, k].Equals(matrix[i1, k1]))
                        {
                            node.AddChild(new NetworkNode<T>(matrix[i1, k1]));
                        }
                    }
                }

                nodes[node.GetHashCode()] = node;
            }
        }
    }

    public List<NetworkNode<T>> GetNodesConnectedFrom(NetworkNode<T> node)
    {
        List<NetworkNode<T>> r = new List<NetworkNode<T>>();
        if (nodes.ContainsKey(node.GetHashCode()))
            r = nodes[node.GetHashCode()].ConnectedTo;
        return r;
    }

    public List<T> GetNodesConnectedFrom(T node)
    {
        List<T> ris = new List<T>();
        foreach (NetworkNode<T> n in GetNodesConnectedFrom(new NetworkNode<T>(node)))
            ris.Add(n.Value);
        return ris;
    }
}

public class NetworkNode<T>
{
    private T _value;
    private List<NetworkNode<T>> _connectedTo;

    public T Value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
        }
    }

    public List<NetworkNode<T>> ConnectedTo
    {
        get
        {
            return _connectedTo;
        }
    }

    public NetworkNode()
    {
        _connectedTo = new List<NetworkNode<T>>();
    }

    public NetworkNode(T value)
    {
        _value = value;
        _connectedTo = new List<NetworkNode<T>>();
    }

    public void AddChild(NetworkNode<T> node)
    {
        _connectedTo.Add(node);
    }

    public override bool Equals(object o)
    {
        if (o is NetworkNode<T>)
            return Equals((NetworkNode<T>)o);
        else
            return base.Equals(o);
    }
    public bool Equals(NetworkNode<T> node)
    {
        if (Value.Equals(node.Value))
            return true;
        else
            return false;
    }
    public static bool operator ==(NetworkNode<T> n1, NetworkNode<T> n2)
    {
        if (System.Object.ReferenceEquals(n1, n2))
        {
            return true;
        }
        if (n1.Value.Equals(n2.Value))
            return true;

        return false;
    }
    public static bool operator !=(NetworkNode<T> n1, NetworkNode<T> n2)
    {
        if (System.Object.ReferenceEquals(n1, n2))
        {
            return false;
        }
        if (n1.Value.Equals(n2.Value))
            return false;

        return true;
    }
    // this implementation is not necessary
    // Only the override is
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

}

