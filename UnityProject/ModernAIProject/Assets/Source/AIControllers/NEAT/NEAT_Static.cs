using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NEAT_Static{
    public static int[] inputNodes = { 0, 1, 2,3 };
    public static int[] outputNodes = { 5, 6,7,8 };

    public static float addNodeProbability = 0.02f;
    public static float deleteConnectionProbability = 0.1f;
    public static float changeWeightProbability = 0.6f;
    public static float addConnectionProbability = 0.3f;
    public static float randomlyGenerateNodeProbability = 0.2f;
    public static float randomlyGenerateConnectionProbability = 0.6f;
    public static float crossoverProbability = 0.4f;
    public static float selectionRangeForCrossover = 0.25f;

}
