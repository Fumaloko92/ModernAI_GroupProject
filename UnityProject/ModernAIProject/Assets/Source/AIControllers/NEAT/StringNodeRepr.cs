using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class StringNodeRepr : INodeRepresentation<string>
{
    private string nodes;

    public string NodeRepresentation { get { return nodes; } }

    private int[] hiddenNodes;
    public void GenerateRandomValue()
    {
        HashSet<int> nodes = new HashSet<int>();
        foreach (int input in NEAT_Static.inputNodes)
            nodes.Add(input);

        foreach (int output in NEAT_Static.outputNodes)
            nodes.Add(output);

        List<int> n = new List<int>();
        while (StaticRandom.Rand(0, 101) / 100f < NEAT_Static.addNodeProbability)
        {
            int newId;
            while (true)
            {
                newId = (int)(StaticRandom.Sample() * int.MaxValue);
                if (!nodes.Contains(newId))
                    break;
            }
            nodes.Add(newId);
            n.Add(newId);
        }
        hiddenNodes = n.ToArray();
        UpdateNodesFromCollection(nodes);
    }

    public int[] GetHiddenNodes()
    {
        return hiddenNodes;
    }

    public void RandomlyAddNode()
    {
        List<int> n = new List<int>(hiddenNodes);
        HashSet<int> nodes = new HashSet<int>();
        foreach (string token in this.nodes.Split(','))
            nodes.Add(int.Parse(token));
        while (StaticRandom.Sample() < NEAT_Static.addNodeProbability)
        {
            int newId;
            while (true)
            {
                newId = (int)(StaticRandom.Sample() * int.MaxValue);
                if (!nodes.Contains(newId))
                    break;
            }
            nodes.Add(newId);
            n.Add(newId);
        }
        hiddenNodes = n.ToArray();
        UpdateNodesFromCollection(nodes);
    }

    public void RandomlyDeleteNode()
    {
        HashSet<int> nodes = new HashSet<int>();
        foreach (string token in this.nodes.Split(','))
            nodes.Add(int.Parse(token));
        List<int> n = new List<int>(hiddenNodes);
        for (int i = n.Count - 1; i >= 0; i--)
            if (StaticRandom.Sample() < NEAT_Static.deleteNodeProbability)
            {
                nodes.Remove(n[i]);
                n.RemoveAt(i);
            }
        hiddenNodes = n.ToArray();
        UpdateNodesFromCollection(nodes);
    }
    private void UpdateNodesFromCollection(ICollection<int> nodes)
    {
        this.nodes = "";
        foreach (int id in nodes)
            this.nodes += id + ",";

        this.nodes = this.nodes.Substring(0, this.nodes.Length - 1);
    }

    public bool Equals(INodeRepresentation<string> other)
    {
        return NodeRepresentation.Equals(other.NodeRepresentation);
    }

    public INodeRepresentation<string> Clone()
    {
        StringNodeRepr r = new StringNodeRepr();
        r.nodes = nodes;
        r.hiddenNodes = new int[hiddenNodes.Length];
        for (int i = 0; i < hiddenNodes.Length; i++)
            r.hiddenNodes[i] = hiddenNodes[i];
        return r;
    }

    INodeRepresentation INodeRepresentation.Clone()
    {
        return Clone();
    }
}
