using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class HistoricalMarkings
{
    private List<HistoricalVariation> historicalVariations;

    public HistoricalMarkings(int population)
    {
        historicalVariations = new List<HistoricalVariation>();
        for (int i = 0; i < population; i++)
            historicalVariations.Add(new HistoricalVariation());
    }

    public HistoricalVariation GetHistoricalVariationAt(int i)
    {
        return historicalVariations[i];
    }

    public void InitializeHistoricalVariationFromPreviousOne(int i)
    {
        historicalVariations[i] = (HistoricalVariation)historicalVariations[i - 1].Clone();
    }
}

public class HistoricalVariation : ICloneable
{
    private List<Variation> variations;

    public HistoricalVariation()
    {
        variations = new List<Variation>();
    }

    public bool VariationExists(Variation v)
    {
        return variations.Contains(v);
    }

    public void AddVariation(Variation v)
    {
        lock(v)
        {
            if (!variations.Contains(v))
                variations.Add(v);
        }
    }

    public int GetNumberOfVariations()
    {
        return variations.Count;
    }

    public int GetVariationIndexOf(Variation v)
    {
        if (variations.Contains(v))
            for (int i = 0; i < variations.Count; i++)
                if (variations[i].Equals(v))
                    return i;
        return -1;
    }

    public int GetLastVariationIndex()
    {
        return variations.Count;
    }

    public Variation GetVariationAt(int index)
    {
        if (index < variations.Count)
            return variations[index];
        return null;
    }

    public object Clone()
    {
        HistoricalVariation copy = new HistoricalVariation();
        foreach (Variation v in variations)
            copy.AddVariation(new Variation(v));
        return copy;
    }
}

public class Variation : IEquatable<Variation>, ICloneable
{
    private VariationType type;

    public Variation(int id)
    {
        type = new NodeVariationType(id);
    }

    public Variation(int from, int to)
    {
        type = new ConnectionVariationType(from, to);
    }

    public Variation(Variation v)
    {
        if (v.IsNode())
            type = new NodeVariationType(v.GetNodeID());
        else
            type = new ConnectionVariationType(v.GetFromNode(), v.GetToNode());
    }

    public bool Equals(Variation other)
    {
        return type.Equals(other.type);
    }

    public void Enable()
    {
        if (IsConnection())
            ((ConnectionVariationType)type).Enabled = true;
    }

    public void Disable()
    {
        if (IsConnection())
            ((ConnectionVariationType)type).Enabled = false;
    }

    public bool IsEnabled()
    {
        if (IsConnection())
            return ((ConnectionVariationType)type).Enabled;

        return false;
    }

    public override int GetHashCode()
    {
        return type.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return Equals((Variation)obj);
    }

    public bool IsNode()
    {
        return type.GetType() == typeof(NodeVariationType);
    }

    public bool IsConnection()
    {
        return type.GetType() == typeof(ConnectionVariationType);
    }

    public int GetNodeID()
    {
        if (IsNode())
            return ((NodeVariationType)type).NodeID;
        return -1;
    }
    
    public int GetFromNode()
    {
        if (IsConnection())
            return ((ConnectionVariationType)type).From;
        return -1;
    }
    public int GetToNode()
    {
        if (IsConnection())
            return ((ConnectionVariationType)type).To;
        return -1;
    }

    public object Clone()
    {
        Variation v = new Variation(this);
        return v;
    }

    abstract class VariationType : IEquatable<VariationType>
    {
        abstract public bool Equals(VariationType other);

        public abstract override int GetHashCode();
    }

    class NodeVariationType : VariationType
    {
        private int nodeID;
        public int NodeID { get { return nodeID; } }
        public NodeVariationType(int id)
        {
            nodeID = id;
        }
        public override bool Equals(VariationType other)
        {
            if (other.GetType() == typeof(NodeVariationType))
                return Equals((NodeVariationType)other);
            return false;
        }

        public bool Equals(NodeVariationType other)
        {
            return nodeID == other.nodeID;
        }

        public override int GetHashCode()
        {
            return nodeID.GetHashCode();
        }
    }

    class ConnectionVariationType : VariationType
    {
        private int from, to;
        private bool enabled;
        public int From { get { return from; } }
        public int To { get { return to; } }
        public bool Enabled { get { return enabled; } set { enabled = value; } }

        public ConnectionVariationType(int from, int to)
        {
            this.from = from; this.to = to; enabled = true;
        }
        public override bool Equals(VariationType other)
        {
            if (other.GetType() == typeof(ConnectionVariationType))
                return Equals((ConnectionVariationType)other);
            return false;
        }

        public bool Equals(ConnectionVariationType other)
        {
            return from == other.from && to == other.to;
        }

        public override int GetHashCode()
        {
            return from.GetHashCode() ^ to.GetHashCode();
        }
    }
}

