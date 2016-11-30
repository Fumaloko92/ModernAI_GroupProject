using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NEAT_Static{
    public static int[] inputNodes = { 0, 1, 2 };
    public static int[] outputNodes = { 5, 6 };

    public static float addNodeProbability = 0.2f;
    public static float deleteConnectionProbability = 0.2f;
    public static float changeWeightProbability = 0.6f;
    public static float addConnectionProbability = 0.4f;
    public static float randomlyGenerateNodeProbability = 0.3f;
}
