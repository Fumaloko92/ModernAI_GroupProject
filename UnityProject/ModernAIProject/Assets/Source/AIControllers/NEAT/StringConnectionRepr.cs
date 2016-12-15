using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Threading;

public class StringConnectionRepr : IConnectionRepresentation<string>
{
    private string connections;

    private Dictionary<int, Dictionary<int, int>> layers;
    private Dictionary<int, Dictionary<int, ConnectionData>> connectionsDict;

    public string ConnectionsRepresentation
    {
        get
        {
            return connections;
        }
    }

    public StringConnectionRepr()
    {
        connections = "";
        layers = new Dictionary<int, Dictionary<int, int>>();
        connectionsDict = new Dictionary<int, Dictionary<int, ConnectionData>>();
    }

    public void GenerateRandomValue(INodeRepresentation nodes, HistoricalVariation v, Dictionary<int, Variation> v1)
    {
        connectionsDict = new Dictionary<int, Dictionary<int, ConnectionData>>();
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
                        if (StaticRandom.Sample() < NEAT_Static.randomlyGenerateConnectionProbability && !(connectionsDict.ContainsKey(to) && connectionsDict[to].ContainsKey(from)))
                        {
                            if (connectionsDict.ContainsKey(to))
                                connectionsDict[to].Add(from, new ConnectionData((float)StaticRandom.Sample()));
                            else
                            {
                                Dictionary<int, ConnectionData> froms = new Dictionary<int, ConnectionData>();
                                froms.Add(from, new ConnectionData((float)StaticRandom.Sample()));
                                connectionsDict[to] = froms;
                            }
                            if (from == to)
                                Debug.Log("Problema");

                            Variation c = new Variation(from, to);
                            v.AddVariation(c);
                            v1.Add(v.GetVariationIndexOf(c), c);

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
                if (connectionsDict[to][from].enabled)
                    temp += from + "*" + connectionsDict[to][from].ToString() + "*" + to + ",";
            }
            connections += temp;
        }
        if (connections.Length > 0)
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

        foreach (int to in keys.Keys)
            foreach (int from in keys[to])
                if (StaticRandom.Sample() < NEAT_Static.changeWeightProbability)
                {
                    ConnectionData d = connectionsDict[to][from];
                    if (StaticRandom.Sample() < 0.5f)
                        d.weight += (float)StaticRandom.Sample();
                    else
                        d.weight -= (float)StaticRandom.Sample();
                    connectionsDict[to][from] = d;
                }
        UpdateGenotype();
    }

    public void RandomlyAddConnection(HistoricalVariation v, Dictionary<int, Variation> v1)
    {
        if (StaticRandom.Sample() < NEAT_Static.addConnectionProbability)
        {
            for (int i = layers.Count - 1; i > 0; i--)
            {
                foreach (int to in layers[i].Values)
                {
                    List<int> possible_froms = new List<int>();
                    for (int k = i - 1; k >= 0; k--)
                    {
                        foreach (int from in layers[k].Values)
                        {
                            if (!connectionsDict.ContainsKey(to) || !connectionsDict[to].ContainsKey(from))
                            {
                                possible_froms.Add(from);
                                /*Dictionary<int, ConnectionData> froms;
                                if (connectionsDict.ContainsKey(to))
                                    froms = connectionsDict[to];
                                else
                                    froms = new Dictionary<int, ConnectionData>();
                                froms.Add(from, new ConnectionData((float)StaticRandom.Sample()));
                                connectionsDict[to] = froms;
                                return;*/
                            }
                        }
                    }
                    if (possible_froms.Count == 0)
                        continue;
                    if (StaticRandom.Sample() < 0.5f)
                    {
                        int f = possible_froms[StaticRandom.Rand(0, possible_froms.Count)];
                        Dictionary<int, ConnectionData> froms;
                        if (connectionsDict.ContainsKey(to))
                            froms = connectionsDict[to];
                        else
                            froms = new Dictionary<int, ConnectionData>();
                        froms.Add(f, new ConnectionData((float)StaticRandom.Sample()));
                        connectionsDict[to] = froms;
                        Variation c = new Variation(f, to);
                        v.AddVariation(c);
                        int k = v.GetVariationIndexOf(c);
                        if (v1.ContainsKey(k))
                            v1[k] = c;
                        else
                            v1.Add(k, c);
                        UpdateGenotype();
                        return;
                    }
                }
            }
        }
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
                Dictionary<int, ConnectionData> lower_cons, upper_cons;
                lower_cons = new Dictionary<int, ConnectionData>();

                if (connectionsDict.ContainsKey(upper_node))
                {
                    upper_cons = connectionsDict[upper_node];
                }
                else
                    upper_cons = new Dictionary<int, ConnectionData>();

                lower_cons.Add(lower_node, new ConnectionData((float)StaticRandom.Sample()));
                upper_cons.Add(hidden, new ConnectionData((float)StaticRandom.Sample()));

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
                Dictionary<int, ConnectionData> d = connectionsDict[to];
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
        c.connectionsDict = new Dictionary<int, Dictionary<int, ConnectionData>>();
        foreach (int key in connectionsDict.Keys)
        {
            Dictionary<int, ConnectionData> copy = new Dictionary<int, ConnectionData>();
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

    public void RandomlyDeleteConnection(HistoricalVariation v, Dictionary<int, Variation> v1)
    {
        if (StaticRandom.Sample() < NEAT_Static.deleteConnectionProbability)
        {
            List<int> keys = new List<int>();
            foreach (int key in connectionsDict.Keys)
                keys.Add(key);

            int to_key = keys[StaticRandom.Rand(0, keys.Count)];

            Dictionary<int, ConnectionData> froms = connectionsDict[to_key];
            keys = new List<int>();
            foreach (int key in froms.Keys)
                keys.Add(key);

            int from_key = keys[StaticRandom.Rand(0, keys.Count)];
            connectionsDict[to_key][from_key] = new ConnectionData(connectionsDict[to_key][from_key], false);
            Variation c = new Variation(from_key, to_key);
            c.Disable();
            v1[v.GetVariationIndexOf(c)] = c;
            UpdateGenotype();
        }
    }

    public string GetRandomConnection()
    {
        string[] cons = connections.Split(',');
        return cons[StaticRandom.Rand(0, cons.Length)];
    }

    public void AddSplitConnection(int from, int mid, int to, float weight)
    {
        string keys = "Layer indexes:";
        foreach (int layer in layers.Keys)
            keys += layer + ", ";
        try
        {
            int mid_layer = -1;
            int from_layer = -1, to_layer = -1;
            foreach (int layer in layers.Keys)
            {
                if (layers[layer].ContainsValue(from))
                {
                    from_layer = layer;
                    mid_layer = layer + 1;
                    continue;
                }
                if (layers[layer].ContainsValue(to))
                {
                    to_layer = layer;
                    continue;
                }
            }
            if (to_layer - from_layer == 1)
            {
                Dictionary<int, int> new_layer = new Dictionary<int, int>();
                new_layer.Add(0, mid);
                for (int i = layers.Count - 1; i >= mid_layer; i--)
                {
                    if (layers.ContainsKey(i + 1))
                    {
                        layers[i + 1] = layers[i];
                    }
                    else
                    {
                        layers.Add(i + 1, layers[i]);
                    }
                }
                layers[mid_layer] = new_layer;
            }
            else
            {
                Dictionary<int, int> layer = layers[mid_layer];

                layer.Add(layer.Count, mid);
                layers[mid_layer] = layer;
            }
            Dictionary<int, ConnectionData> froms = new Dictionary<int, ConnectionData>();
            froms.Add(from, new ConnectionData(1f));
            connectionsDict.Add(mid, froms);
            if (connectionsDict.ContainsKey(to))
                froms = connectionsDict[to];
            else
                froms = new Dictionary<int, ConnectionData>();
            froms.Add(mid, new ConnectionData(weight));
            if (connectionsDict.ContainsKey(to))
                connectionsDict[to] = froms;
            else
                connectionsDict.Add(to, froms);

            connectionsDict[to].Remove(from);
            UpdateGenotype();
        }
        catch (Exception e)
        {
            string s = "EXCEPTION LOG" + Thread.CurrentThread.ManagedThreadId + System.Environment.NewLine;
            s += "connections string: " + connections + System.Environment.NewLine;
            s += keys + System.Environment.NewLine;
            s += "From: " + from + " layer: " + FindLayerByNodeID(from) + System.Environment.NewLine;
            s += "Mid: " + mid + " layer: " + FindLayerByNodeID(mid) + System.Environment.NewLine;
            s += "To: " + to + " layer: " + FindLayerByNodeID(to) + Environment.NewLine;
            foreach (int layer in layers.Keys)
            {
                s += "Layer " + layer + ": ";
                foreach (int id in layers[layer].Values)
                    s += id + " ";
                s += Environment.NewLine;
            }
            s += Environment.NewLine + e.ToString();
            Debug.Log(s);

            //Debug.Log(e.ToString());
        }
    }

    public bool IsEmpty()
    {
        return connections.Length == 0;
    }

    public bool ContainsConnection(int from, int to)
    {
        return connectionsDict.ContainsKey(to) && connectionsDict[to].ContainsKey(from);
    }

    public void AddConnection(int from, int to, float weight, bool enabled)
    {
        if (!connectionsDict.ContainsKey(to) || !connectionsDict[to].ContainsKey(from))
        {
            Dictionary<int, ConnectionData> froms;
            if (connectionsDict.ContainsKey(to))
                froms = connectionsDict[to];
            else
                froms = new Dictionary<int, ConnectionData>();
            froms.Add(from, new ConnectionData(weight, enabled));
            connectionsDict[to] = froms;
            UpdateGenotype();
        }
    }

    public float GetConnectionWeight(int from, int to)
    {
        if (ContainsConnection(from, to))
        {
            return connectionsDict[to][from].weight;
        }

        return float.MinValue;
    }
    /*
    public void OrganizeNodesByHiddenNodesAndConnections(INodeRepresentation nodes)
    {
        layers = new Dictionary<int, Dictionary<int, int>>();
        Dictionary<int, int> inputLayer = new Dictionary<int, int>();
        Dictionary<int, int> outputLayer = new Dictionary<int, int>();
        for (int i = 0; i < NEAT_Static.inputNodes.Length; i++)
            inputLayer.Add(i, NEAT_Static.inputNodes[i]);
        for (int i = 0; i < NEAT_Static.outputNodes.Length; i++)
            outputLayer.Add(i, NEAT_Static.outputNodes[i]);
        layers.Add(0, inputLayer);
        layers.Add(1, outputLayer);
        Queue<int> hiddenQueue = new Queue<int>(nodes.GetHiddenNodes());
        //PrintConnections();
        HashSet<int> visited = new HashSet<int>();
        while (hiddenQueue.Count > 0)
        {
            int h = hiddenQueue.Dequeue();
            Dictionary<int, HashSet<int>> toRemove = new Dictionary<int, HashSet<int>>();
            if (connectionsDict.ContainsKey(h))
            {
                int max = -1;
                bool breakLoop = false;
                foreach (int from in connectionsDict[h].Keys)
                {
                    int l = FindLayerByNodeID(from);
                    if (l != -1)
                    {
                        if (l > max)
                            max = l;
                    }
                    else
                    {
                        if (!visited.Contains(h))
                        {
                            visited.Add(h);
                            hiddenQueue.Enqueue(h);

                        }
                        else
                        {
                            HashSet<int> rem;
                            if (toRemove.ContainsKey(h))
                                rem = toRemove[h];
                            else
                                rem = new HashSet<int>();
                            rem.Add(from);
                            toRemove.Add(h, rem);
                        }
                        breakLoop = true;
                        break;
                    }
                }
                if (toRemove.Count > 0)
                {
                    foreach (int to in toRemove.Keys)
                        foreach (int from in toRemove[to])
                        {
                            connectionsDict[to].Remove(from);
                            if (connectionsDict[to].Count == 0)
                                connectionsDict.Remove(to);
                        }
                    toRemove.Clear();
                }
                if (breakLoop)
                    continue;
                if (max + 1 == layers.Count - 1)
                {
                    // PrintLayers("1b" + System.Environment.NewLine);
                    layers.Add(layers.Count, layers[layers.Count - 1]);
                    Dictionary<int, int> newLayer = new Dictionary<int, int>();
                    newLayer.Add(0, h);
                    layers[max + 1] = newLayer;
                    //PrintLayers("1" + System.Environment.NewLine);
                }
                else
                {
                    Dictionary<int, int> layer = layers[max + 1];
                    layer.Add(layer.Count, h);
                    layers[max + 1] = layer;
                    //PrintLayers("2" + System.Environment.NewLine);
                }
            }
            else
            {
                if (layers.Count == 2)
                {
                    layers.Add(2, layers[1]);
                    Dictionary<int, int> newLayer = new Dictionary<int, int>();
                    newLayer.Add(0, h);
                    layers[1] = newLayer;
                    //PrintLayers("3" + System.Environment.NewLine);
                }
                else
                {
                    Dictionary<int, int> layer = layers[1];
                    layer.Add(layers[1].Count, h);
                    layers[1] = layer;
                    //PrintLayers("4" + System.Environment.NewLine);
                }
            }
        }
    }
    */

    public void OrganizeNodesByHiddenNodesAndConnections(INodeRepresentation nodes)
    {
        layers = new Dictionary<int, Dictionary<int, int>>();
        Dictionary<int, int> inputLayer = new Dictionary<int, int>();
        Dictionary<int, int> outputLayer = new Dictionary<int, int>();
        for (int i = 0; i < NEAT_Static.inputNodes.Length; i++)
            inputLayer.Add(i, NEAT_Static.inputNodes[i]);
        for (int i = 0; i < NEAT_Static.outputNodes.Length; i++)
            outputLayer.Add(i, NEAT_Static.outputNodes[i]);
        layers.Add(0, inputLayer);
        layers.Add(1, outputLayer);
        foreach (int hidden in nodes.GetHiddenNodes())
            AssignLayerToNodeBasedOnConnections(hidden);
        /*string s = "";
        s += "Nodes: ";
        foreach (int n in nodes.GetHiddenNodes())
            s += n + " ";
        s += "Result of organization" + Environment.NewLine;
        foreach (int layer in layers.Keys)
        {
            s += "Layer " + layer + ": ";
            foreach (int index in layers[layer].Keys)
                s += layers[layer][index] + " ";
            s += Environment.NewLine;
        }*/
        //Debug.Log(s);
    }

    private void AssignLayerToNodeBasedOnConnections(int nodeID)
    {
        if (!connectionsDict.ContainsKey(nodeID))
        {
            int min = int.MaxValue;
            foreach (int to in connectionsDict.Keys)
                foreach (int from in connectionsDict[to].Keys)
                    if (from == min)
                    {
                        if (FindLayerByNodeID(to) == -1)
                            AssignLayerToNodeBasedOnConnections(to);
                        int l = FindLayerByNodeID(to);
                        if (min > l)
                            l = min;
                    }
            if (min == int.MaxValue)
                min = 1;
            if (min == 1)
            {
                for (int i = layers.Count - 1; i > 0; i--)
                    if (layers.ContainsKey(i + 1))
                        layers[i + 1] = layers[i];
                    else
                        layers.Add(i + 1, layers[i]);
                Dictionary<int, int> newLayer = new Dictionary<int, int>();
                newLayer.Add(0, nodeID);
                layers[1] = newLayer;
            }
            else
            {
                Dictionary<int, int> layer = layers[min - 1];
                layer.Add(layer.Count, nodeID);
                layers[min - 1] = layer;
            }
        }
        else
        {
            int max = int.MinValue;
            foreach (int from in connectionsDict[nodeID].Keys)
            {
                if (FindLayerByNodeID(from) == -1)
                    AssignLayerToNodeBasedOnConnections(from);
                int l = FindLayerByNodeID(from);
                if (l > max)
                    max = l;
            }
            if (max == int.MaxValue)
                max = layers.Count - 2;
            if (max + 1 == layers.Count - 1)
            {
                layers.Add(layers.Count, layers[max + 1]);
                Dictionary<int, int> newLayer = new Dictionary<int, int>();
                newLayer.Add(0, nodeID);
                layers[max + 1] = newLayer;
            }
            else
            {
                Dictionary<int, int> layer = layers[max + 1];
                layer.Add(layer.Count, nodeID);
                layers[max + 1] = layer;
            }
        }
    }

    private int FindLayerByNodeID(int nodeID)
    {
        int layer = -1;
        foreach (int key in layers.Keys)
            foreach (int id in layers[key].Values)
                if (nodeID == id)
                {
                    layer = key;
                    break;
                }
        return layer;
    }

    private void PrintLayers(string init)
    {
        string s = init;
        foreach (int k in layers.Keys)
        {
            s += k + ":";
            foreach (int id in layers[k].Values)
                s += id + " ";
            s += System.Environment.NewLine;
        }

        Debug.Log(s);
    }

    private void PrintConnections()
    {
        string s = "";
        foreach (int to in connectionsDict.Keys)
        {
            s += to + ":";
            foreach (int from in connectionsDict[to].Keys)
                s += from + " ";
            s += System.Environment.NewLine;
        }
        Debug.Log(s);
    }
}

class ConnectionData
{
    public float weight;
    public bool enabled;

    public ConnectionData(float w, bool e)
    {
        weight = w; enabled = e;
    }

    public ConnectionData(float w)
    {
        weight = w; enabled = true;
    }

    public ConnectionData(ConnectionData d, float w)
    {
        weight = d.weight + w;
        enabled = true;
    }

    public ConnectionData(ConnectionData d, bool e)
    {
        weight = d.weight;
        enabled = e;
    }

    public override string ToString()
    {
        return "" + weight;
    }
}