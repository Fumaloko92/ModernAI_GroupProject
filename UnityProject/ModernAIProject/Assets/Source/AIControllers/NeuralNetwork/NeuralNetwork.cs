using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

public class NeuralNetwork<T, K> where T : IActivationFunction, new() where K : IActivationFunction, new()
{
    private Dictionary<int, Node> nodes;
    private Dictionary<int, Connection> connections;
    private HashSet<int> inputIds;
    private T hiddenNeuronAF;
    private K outputNeuronAF;

    public NeuralNetwork()
    {
        inputIds = new HashSet<int>();
        nodes = new Dictionary<int, Node>();
        connections = new Dictionary<int, Connection>();
        hiddenNeuronAF = new T();
        outputNeuronAF = new K();
    }

    public NeuralNetwork(int[] inputNodes, int[] hiddenNodes, int[] outputNodes, string connections)
    {
        inputIds = new HashSet<int>();
        this.nodes = new Dictionary<int, Node>();
        this.connections = new Dictionary<int, Connection>();
        hiddenNeuronAF = new T();
        outputNeuronAF = new K();
        FromArrayToNodes(inputNodes, outputNodes, hiddenNodes);
        FromStringToConnections(connections);
    }

    public void SetData(int nodeID, double val)
    {
        if (nodes.ContainsKey(nodeID.GetHashCode()))
            nodes[nodeID].Value = val;
    }

    private void FromArrayToNodes(int[] inputNodes, int[] outputNodes, int[] hiddenNodes)
    {


        foreach (int node in inputNodes)
        {
            Node n = new InputNode(node);
            inputIds.Add(node);
            nodes[n.GetHashCode()] = n;
        }

        foreach (int node in outputNodes)
        {
            Node n = new OutputNode(node, outputNeuronAF.ActivateValue);
            nodes[n.GetHashCode()] = n;
        }

        foreach (int node in hiddenNodes)
        {
            Node n = new HiddenNode(node, hiddenNeuronAF.ActivateValue);
            nodes[n.GetHashCode()] = n;
        }
    }

    private void FromStringToConnections(string connections)
    {
        
            string[] cons = connections.Split(',');
            foreach (string connection in cons)
            {
                string[] details = connection.Split('*');
            try
            {
                AddConnection(int.Parse(details[0]), int.Parse(details[2]), double.Parse(details[1]));
            }
            catch (Exception e)
            {
                Debug.Log(connection);
            }
        }
       
    }

    private void AddNode(Node node)
    {
        nodes[node.GetHashCode()] = node;
    }

    public void AddNode(int id, NodeType type, double value)
    {
        Node n;
        switch (type)
        {
            /*case NodeType.Bias:
            //TODO
            break;*/
            default:
                n = new InputNode(id);
                inputIds.Add(id);
                break;
        }
        n.Value = value;
        AddNode(n);
    }

    public void AddNode(int id, NodeType type)
    {
        Node n;
        switch (type)
        {
            case NodeType.Hidden:
                n = new HiddenNode(id, hiddenNeuronAF.ActivateValue);
                break;
            /*case NodeType.Bias:
            //TODO
            break;*/
            default:
                n = new OutputNode(id, outputNeuronAF.ActivateValue);
                break;
        }
        AddNode(n);
    }
    private void AddConnection(Connection connection)
    {
        if (nodes.ContainsKey(connection.From.GetHashCode()) && nodes.ContainsKey(connection.To.GetHashCode()))
            connections[connection.GetHashCode()] = connection;
    }

    private void AddConnection(Node from, Node to, double weight)
    {
        Connection c = new Connection(from, to, weight);
        AddConnection(c);
    }

    public void AddConnection(int from, int to, double weight)
    {
        if (nodes.ContainsKey(from.GetHashCode()) && nodes.ContainsKey(to.GetHashCode()))
            AddConnection(nodes[from.GetHashCode()], nodes[to.GetHashCode()], weight);
    }

    private HashSet<int> GetFromNeuronsConnectedTo(int id)
    {
        HashSet<int> ris = new HashSet<int>();
        foreach (int key in connections.Keys)
            if (connections[key].To.Id == id)
                ris.Add(connections[key].From.Id);
        return ris;
    }

    private HashSet<int> GetToNeuronsConnectedFrom(int id)
    {
        HashSet<int> ris = new HashSet<int>();
        foreach (int key in connections.Keys)
            if (connections[key].From.Id == id)
                ris.Add(connections[key].To.Id);
        return ris;
    }

    public Dictionary<int, double> ExecuteNetwork()
    {
        Dictionary<int, double> ris = new Dictionary<int, double>();
        Queue<int> toProcess = new Queue<int>();
        foreach (int node in inputIds)
        {
            foreach (Connection con in connections.Values)
                if (con.From.Id == node)
                    toProcess.Enqueue(node);
        }
        HashSet<int> processed = new HashSet<int>();
        int requeues = 0;
        int newNodesCounter = toProcess.Count;
        while (toProcess.Count > 0 && newNodesCounter>0)
        {
            newNodesCounter--;
            int processing = toProcess.Dequeue();
            bool reQueue = false;
            double sum = 0;
            bool isInput = true;
            foreach (int id in GetFromNeuronsConnectedTo(processing))
            {
                isInput = false;
                if (!processed.Contains(id))
                {
                    reQueue = true;
                    break;
                }
                else
                    sum += nodes[id.GetHashCode()].GetValue * connections[id.GetHashCode() ^ processing.GetHashCode()].Weight;
            }
 
            if (!reQueue)
            {
                processed.Add(processing);
                if (!isInput)
                    nodes[processing.GetHashCode()].Value = sum;
                if (nodes[processing.GetHashCode()].GetType() == typeof(OutputNode))
                    ris[processing] = nodes[processing.GetHashCode()].GetValue;
            }
            else {
                requeues++;
                toProcess.Enqueue(processing);
                continue;
            }
            foreach (int id in GetToNeuronsConnectedFrom(processing))
                if (!processed.Contains(id))
                {
                    newNodesCounter++;
                    toProcess.Enqueue(id);
                }
            newNodesCounter += requeues;
            requeues = 0;
        }
        return ris;
    }
}

public interface IActivationFunction
{
    double ActivateValue(double value);
}

public class Sigmoid : IActivationFunction
{
    public double ActivateValue(double value)
    {
        return 1 / (1 + Math.Pow(Math.E, -value));
    }
}

abstract class Node
{
    protected int id;
    protected double value;

    public double Value
    {
        set { this.value = value; }
    }

    public abstract double GetValue
    {
        get;
    }

    public int Id
    {
        get { return id; }
    }

    public override bool Equals(object obj)
    {
        return ((Node)obj).id == id;
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }
}

class InputNode : Node
{
    public InputNode(int id)
    {
        this.id = id;
    }

    public override double GetValue
    {
        get { return value; }
    }
}

class HiddenNode : Node
{
    private Func<double, double> activationFunction;

    public HiddenNode(int id, Func<double, double> activationFunction)
    {
        this.id = id;
        this.activationFunction = activationFunction;
    }

    public override double GetValue
    {
        get { return activationFunction(value); }
    }
}

class OutputNode : Node
{
    private Func<double, double> activationFunction;

    public OutputNode(int id, Func<double, double> activationFunction)
    {
        this.id = id;
        this.activationFunction = activationFunction;
    }

    public override double GetValue
    {
        get { return activationFunction(value); }
    }
}

public enum NodeType
{
    Input, Hidden, Output, Bias
}

class Connection
{
    private Node from;
    private Node to;
    private double weight;

    public Node From
    {
        get { return from; }
    }

    public Node To
    {
        get { return to; }
    }

    public double Weight
    {
        get { return weight; }
    }

    public Connection(Node f, Node t, double w)
    {
        from = f; to = t; weight = w;
    }

    public override bool Equals(object obj)
    {
        return from == ((Connection)obj).from && to == ((Connection)obj).to;
    }

    public override int GetHashCode()
    {
        return from.GetHashCode() ^ to.GetHashCode();
    }
}
