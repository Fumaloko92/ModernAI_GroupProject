using UnityEngine;
using System.Collections;

public class NEAT_Static{
    public static int[] inputNodes = { 0, 1, 2, 3 };
    public static int[] outputNodes = { 4, 5, 6, 7 };

    public static float addNodeProbability = 0.2f;
    public static float deleteNodeProbability = 0.2f;
    public static float changeWeightProbability = 0.6f;
}
