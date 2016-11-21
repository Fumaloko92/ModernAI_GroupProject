﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class StringConnectionRepr : IConnectionRepresentation<string>
{
    private string connections;

    private Dictionary<int, Dictionary<int, int>> layers;
    private Dictionary<int, Dictionary<int, float>> connectionsDict;

    public string ConnectionsRepresentation
    {
        get
        {
            return connections;
        }
    }

    public void GenerateRandomValue(INodeRepresentation nodes)
    {
        connectionsDict = new Dictionary<int, Dictionary<int, float>>();
        layers = new Dictionary<int, Dictionary<int, int>>();

        layers[0] = new Dictionary<int, int>();
        for (int i = 0; i < NEAT_Static.inputNodes.Length; i++)
            layers[0].Add(i, NEAT_Static.inputNodes[i]);

        layers[1] = new Dictionary<int, int>();
        for (int i = 0; i < NEAT_Static.outputNodes.Length; i++)
            layers[1].Add(i, NEAT_Static.outputNodes[i]);

        OrganizeHiddenNodes(nodes.GetHiddenNodes());

        for (int layer = layers.Count - 1; layer > 0; layer--)
        {

            foreach (int to in layers[layer].Values)
            {
                do
                {
                    foreach (int from in layers[layer - 1].Values)
                    {
                        if (StaticRandom.Rand(0, 101) < 50)
                        {
                            if (connectionsDict.ContainsKey(to))
                                connectionsDict[to].Add(from, (float)StaticRandom.Sample());
                            else
                            {
                                Dictionary<int, float> froms = new Dictionary<int, float>();
                                froms.Add(from, (float)StaticRandom.Sample());
                                connectionsDict[to] = froms;
                            }
                        }
                    }
                } while (!connectionsDict.ContainsKey(to) || connectionsDict[to].Count == 0);
            }
        }
        UpdateGenotype();


    }

    private void UpdateGenotype()
    {
        connections = "";
        foreach (int to in connectionsDict.Keys)
        {
            string temp = "";
            foreach (int from in connectionsDict[to].Keys)
            {
                temp += from + "*" + connectionsDict[to][from] + "*" + to + ",";
            }
            connections += temp;
        }

        connections = connections.Substring(0, connections.Length - 1);
    }

    private void OrganizeHiddenNodes(int[] hiddenNodes)
    {
        foreach (int hidden in hiddenNodes)
        {
            RandomlyAssignLayer(hidden);
        }
    }

    private int RandomlyAssignLayer(int hidden)
    {
        int layerIndex = StaticRandom.Rand(1, layers.Count);
        if (layerIndex == layers.Count - 1)
        {
            layers[layerIndex + 1] = layers[layerIndex];
            Dictionary<int, int> newLayer = new Dictionary<int, int>();
            newLayer.Add(0, hidden);
            layers[layerIndex] = newLayer;
            return layerIndex;
        }

        if (StaticRandom.Sample() < 0.5)
        {

            for (int i = layers.Count; i > layerIndex; i--)
                layers[i] = layers[i - 1];
            Dictionary<int, int> newLayer = new Dictionary<int, int>();
            newLayer.Add(0, hidden);
            layers[layerIndex] = newLayer;
            return layerIndex;
        }

        layers[layerIndex].Add(layers[layerIndex].Count, hidden);
        return layerIndex;
    }

    public void RandomlyChangeWeights()
    {
        Dictionary<int, List<int>> keys = new Dictionary<int, List<int>>();
        foreach (int to in connectionsDict.Keys)
        {
            List<int> l = new List<int>();
            foreach (int from in connectionsDict[to].Keys)
            {
                l.Add(from);        
            }
            keys.Add(to, l);
        }

        foreach(int to in keys.Keys)
            foreach(int from in keys[to])
                if (StaticRandom.Sample() < NEAT_Static.changeWeightProbability)
                {
                    if (StaticRandom.Sample() < 0.5f)
                        connectionsDict[to][from] += (float)StaticRandom.Sample();
                    else
                        connectionsDict[to][from] -= (float)StaticRandom.Sample();

                }
        UpdateGenotype();
    }

    public void RandomlyAddConnection()
    {
        for (int i = layers.Count - 1; i > 0; i--)
        {
            foreach (int to in layers[i].Values)
            {
                for (int k = i - 1; k >= 0; k--)
                {
                    foreach (int from in layers[k].Values)
                    {
                        if (!connectionsDict.ContainsKey(to) || !connectionsDict[to].ContainsKey(from))
                        {
                            if (StaticRandom.Sample() < 0.4)
                            {
                                Dictionary<int, float> froms;
                                if (connectionsDict.ContainsKey(to))
                                    froms = connectionsDict[to];
                                else
                                    froms = new Dictionary<int, float>();
                                froms.Add(from, (float)StaticRandom.Sample());
                                connectionsDict[to] = froms;

                            }
                        }
                    }
                }
            }
        }
        UpdateGenotype();
    }

    public void UpdateConnections(INodeRepresentation nodes)
    {
        int[] hiddenNodes = nodes.GetHiddenNodes();
        foreach (int hidden in hiddenNodes)
        {
            if (!connectionsDict.ContainsKey(hidden))
            {
                int layerIndex = RandomlyAssignLayer(hidden);
                int lower_node = layers[layerIndex - 1][StaticRandom.Rand(0, layers[layerIndex - 1].Count)];
                int upper_node = layers[layerIndex + 1][StaticRandom.Rand(0, layers[layerIndex + 1].Count)];
                Dictionary<int, float> lower_cons, upper_cons;
                lower_cons = new Dictionary<int, float>();

                if (connectionsDict.ContainsKey(upper_node))
                {
                    upper_cons = connectionsDict[upper_node];
                }
                else
                    upper_cons = new Dictionary<int, float>();

                lower_cons.Add(lower_node, (float)StaticRandom.Sample());
                upper_cons.Add(hidden, (float)StaticRandom.Sample());

                connectionsDict[upper_node] = upper_cons;
                connectionsDict[hidden] = lower_cons;
            }
        }
        Dictionary<int, int> toRemove = new Dictionary<int, int>();
        HashSet<int> all_nodes = new HashSet<int>(nodes.GetHiddenNodes());
        all_nodes.IntersectWith(NEAT_Static.inputNodes);
        all_nodes.IntersectWith(NEAT_Static.outputNodes);
        foreach (int to in connectionsDict.Keys)
        {
            if (!all_nodes.Contains(to))
            {
                toRemove.Add(to, -1);
                continue;
            }
            foreach (int from in connectionsDict[to].Keys)
            {
                if (!all_nodes.Contains(from))
                {
                    toRemove.Add(to, from);
                }
            }

        }
        foreach (int to in toRemove.Keys)
            if (toRemove[to] == -1)
                connectionsDict.Remove(to);
            else
            {
                Dictionary<int, float> d = connectionsDict[to];
                d.Remove(toRemove[to]);
                connectionsDict[to] = d;
            }

        UpdateGenotype();
    }

    public bool Equals(IConnectionRepresentation<string> other)
    {
        return ConnectionsRepresentation.Equals(other.ConnectionsRepresentation);
    }

    public IConnectionRepresentation<string> Clone()
    {
        StringConnectionRepr c = new StringConnectionRepr();
        c.connections = connections;
        c.connectionsDict = new Dictionary<int, Dictionary<int, float>>();
        foreach (int key in connectionsDict.Keys)
        {
            Dictionary<int, float> copy = new Dictionary<int, float>();
            foreach (int key1 in connectionsDict[key].Keys)
                copy.Add(key1, connectionsDict[key][key1]);
            c.connectionsDict.Add(key, copy);
        }
        c.layers = new Dictionary<int, Dictionary<int, int>>();
        foreach (int key in layers.Keys)
        {
            Dictionary<int, int> copy = new Dictionary<int, int>();
            foreach (int key1 in layers[key].Keys)
                copy.Add(key1, layers[key][key1]);
            c.layers.Add(key, copy);
        }
        return c;
    }

    IConnectionRepresentation IConnectionRepresentation.Clone()
    {
        return Clone();
    }
}
