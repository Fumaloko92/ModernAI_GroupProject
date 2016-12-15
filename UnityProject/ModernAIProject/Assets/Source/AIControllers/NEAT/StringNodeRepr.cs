using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class StringNodeRepr : INodeRepresentation<string>
{
    private string nodes;

    public string NodeRepresentation { get { return nodes; } }

    private int[] hiddenNodes;

    public StringNodeRepr()
    {
        nodes = "";
        hiddenNodes = new int[0];
    }
    public void GenerateRandomValue(HistoricalVariation v, Dictionary<int, Variation> v1)
    {
        HashSet<int> nodes = new HashSet<int>();
        foreach (int input in NEAT_Static.inputNodes)
        {
            nodes.Add(input);
            Variation c = new Variation(input);
            v.AddVariation(c);
            v1.Add(v.GetVariationIndexOf(c), c);
        }
        foreach (int output in NEAT_Static.outputNodes)
        {
            nodes.Add(output);
            Variation c = new Variation(output);
            v.AddVariation(c);
            v1.Add(v.GetVariationIndexOf(c), c);
        }
        List<int> n = new List<int>();
        while (StaticRandom.Sample() < NEAT_Static.randomlyGenerateNodeProbability)
        {
            int newId = 1;
            while (nodes.Contains(newId))
            {
                newId++;
            }
            nodes.Add(newId);
            n.Add(newId);
            Variation c = new Variation(newId);
            v.AddVariation(c);
            v1.Add(v.GetVariationIndexOf(c), c);
        }
        hiddenNodes = n.ToArray();
        UpdateNodesFromCollection(nodes);
    }

    public int[] GetHiddenNodes()
    {
        return hiddenNodes;
    }

    public void RandomlyAddNode(IConnectionRepresentation connectionGenotype, HistoricalVariation v, Dictionary<int, Variation> v1)
    {
        if (StaticRandom.Sample() < NEAT_Static.addNodeProbability)
        {
            List<int> n = new List<int>(hiddenNodes);
            HashSet<int> nodes = new HashSet<int>();
            foreach (string token in this.nodes.Split(','))
                nodes.Add(int.Parse(token));
            int newId = 1;
            while (nodes.Contains(newId))
            {
                newId++;
            }
            n.Add(newId);
            Variation c, c1, c2;
            c = new Variation(newId);
            v.AddVariation(c);
            hiddenNodes = n.ToArray();
            nodes.Add(newId);
            string con = connectionGenotype.GetRandomConnection();
            string[] tokens = con.Split('*');
            int from = int.Parse(tokens[0]);
            float weight = float.Parse(tokens[1]);
            int to = int.Parse(tokens[2]);
            connectionGenotype.AddSplitConnection(from, newId, to, weight);

            c1 = new Variation(from, newId);
            c2 = new Variation(newId, to);
            v.AddVariation(c1);
            v.AddVariation(c2);
            Variation c3 = new Variation(from, to);
            v1.Remove(v.GetVariationIndexOf(c3));
            v1.Add(v.GetVariationIndexOf(c), c);
            v1.Add(v.GetVariationIndexOf(c1), c1);
            v1.Add(v.GetVariationIndexOf(c2), c2);
            UpdateNodesFromCollection(nodes);
        }



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

    public bool ContainsNode(int index)
    {
        HashSet<int> nodes = new HashSet<int>();
        foreach (string t in this.nodes.Split(','))
            nodes.Add(int.Parse(t));
        return nodes.Contains(index);
    }

    public void AddNode(int index)
    {
        HashSet<int> nodes = new HashSet<int>();
        nodes.Add(index);
        foreach (string t in this.nodes.Split(','))
            if (t.Length > 0)
                nodes.Add(int.Parse(t));

        HashSet<int> input = new HashSet<int>(NEAT_Static.inputNodes);
        HashSet<int> output = new HashSet<int>(NEAT_Static.outputNodes);
        if (!input.Contains(index) && !output.Contains(index))
        {
            List<int> h = new List<int>(hiddenNodes);
            h.Add(index);
            hiddenNodes = h.ToArray();
        }

        UpdateNodesFromCollection(nodes);
    }
}
