using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
using System.IO;

public class NEAT<T, T1, K, A, A1> where T : IGenotype<T1, K, A, A1>, new() where T1 : INodeRepresentation, new() where K : IConnectionRepresentation, new()
    where A : IActivationFunction, new() where A1 : IActivationFunction, new()
{
    static private NEAT<T, T1, K, A, A1> instance;
    static private Thread NEAT_THREAD = null;
    static private List<List<T>> generations;
    static private int population;
    static private int internalPopulation;
    static private int number_generations;
    static private List<T> next_generation;
    static private Thread t1, t2;
    static private List<T> toEvolve;
    static private int retrievableGen;
    static volatile private HistoricalMarkings historicalMarkings;
    static public  bool finished;
    static private bool serialized;

    static public void Initialize(int n_generations, int population, int internalPopulation, Dictionary<int, double> inputValues, ThreadSafe.World world)
    {
        number_generations = n_generations;

        NEAT<T, T1, K, A, A1>.population = population;
        NEAT<T, T1, K, A, A1>.internalPopulation = internalPopulation;

        retrievableGen = 0;
        instance = new NEAT<T, T1, K, A, A1>();
        NEAT_THREAD = new Thread(o => instance.RunNeatLoop(inputValues, world));
        NEAT_THREAD.Start();
        finished = false;
        serialized = false;
    }

    private NEAT()
    {
        generations = new List<List<T>>();
        next_generation = new List<T>();
        historicalMarkings = new HistoricalMarkings(number_generations);
    }

    public static void AbortThreads()
    {

        t1.Abort();
        t2.Abort();
        NEAT_THREAD.Abort();
        GC.Collect();
    }

    static public List<T> GetGenerationNumber(int n)
    {
        if (n < retrievableGen)
            return generations[n - 1];
        else
            return null;
    }

    static public void Serialize()
    {
        if (!serialized)
        {
            string name = "log.csv";
            string serialization = "";
            serialization += "GEN_NUMB;BEST_FITNESS;MID_FITNESS;WORST_FITNESS" + Environment.NewLine;
            int i = 1;
            foreach (List<T> generation in generations)
            {
                serialization += i + ";" + generation[0].GetFitness() + ";" + generation[(generation.Count - 1) / 2].GetFitness() + ";" + generation[generation.Count - 1].GetFitness() + ";" + Environment.NewLine;
                i++;
            }
            File.WriteAllText(name, serialization);
            serialized = true;
        }
    }

    private void RunNeatLoop(Dictionary<int, double> inputValues, ThreadSafe.World world)
    {
        t1 = null; t2 = null;
        List<T> current_generation = new List<T>();
        IGenotype<T1, K, A, A1> g = new T();
        for (int i = 0; i < population; i++)
            current_generation.Add((T)g.GenerateRandomly(historicalMarkings.GetHistoricalVariationAt(0)));
        lock (generations)
        {
            generations.Add(current_generation);
        }

        //Debug.Log("Generated starting generation");
        for (int i = 1; i < number_generations; i++)
        {

            toEvolve = generations[i - 1];
            // Debug.Log("Evaluating " + i + " generation");
            foreach (T elem in toEvolve)
            {
                while (t1 != null && t1.IsAlive && t2 != null && t2.IsAlive) ;
                if (t1 == null || !t1.IsAlive)
                {
                    t1 = new Thread(o => instance.NeatInnerEvalulationLoop(elem, inputValues, world));
                    t1.IsBackground = true;
                    t1.Start();
                    continue;
                }

                t2 = new Thread(o => instance.NeatInnerEvalulationLoop(elem, inputValues, world));
                t2.IsBackground = true;
                t2.Start();
            }
            while (t1.IsAlive || t2.IsAlive) ;
            toEvolve.Sort((el, el1) => el1.GetFitness().CompareTo(el.GetFitness()));

            generations[i - 1] = toEvolve;
            retrievableGen++;
            int targetSize = toEvolve.Count;
            historicalMarkings.InitializeHistoricalVariationFromPreviousOne(i);
            Debug.Log("Evolving " + i + " generation");
            for (int k = 0; k < targetSize; k++)
            {

                if (StaticRandom.Sample() < (float)(targetSize - k) / (float)targetSize)
                {
                    while (t1.IsAlive && t2.IsAlive) ;
                    T clone = (T)toEvolve[k].Clone();
                    if (StaticRandom.Sample() < NEAT_Static.crossoverProbability)
                    {
                        int selectedLength = (int)(toEvolve.Count * NEAT_Static.selectionRangeForCrossover);
                        T clone1 = (T)toEvolve[StaticRandom.Rand(0, selectedLength)].Clone();
                        if (!t1.IsAlive)
                        {
                            t1 = new Thread(o => instance.NeatInnerCrossoverLoop(clone, clone1, i));
                            t1.IsBackground = true;
                            t1.Start();
                            continue;
                        }
                        t2 = new Thread(o => instance.NeatInnerCrossoverLoop(clone, clone1, i));
                        t2.IsBackground = true;
                        t2.Start();
                    }
                    else
                    {
                        if (!t1.IsAlive)
                        {
                            t1 = new Thread(o => instance.NeatInnerEvolvingLoop(clone, i));
                            t1.IsBackground = true;
                            t1.Start();
                            continue;
                        }
                        t2 = new Thread(o => instance.NeatInnerEvolvingLoop(clone, i));
                        t2.IsBackground = true;
                        t2.Start();
                    }
                }
                else
                {
                    while (t1.IsAlive && t2.IsAlive) ;
                    if (!t1.IsAlive)
                    {
                        t1 = new Thread(o => instance.NeatInnerRandomGenLoop(i));
                        t1.IsBackground = true;
                        t1.Start();
                        continue;
                    }
                    t2 = new Thread(o => instance.NeatInnerRandomGenLoop(i));
                    t2.IsBackground = true;
                    t2.Start();
                }
            }
            while (t1.IsAlive || t2.IsAlive) ;
            lock (generations)
            {
                generations.Add(next_generation);
                lock (next_generation)
                {
                    next_generation = new List<T>();
                }
            }
            Debug.Log(i + " generation done!");
        }
        toEvolve = generations[generations.Count-1];
        // Debug.Log("Evaluating " + i + " generation");
        foreach (T elem in toEvolve)
        {
            while (t1 != null && t1.IsAlive && t2 != null && t2.IsAlive) ;
            if (t1 == null || !t1.IsAlive)
            {
                t1 = new Thread(o => instance.NeatInnerEvalulationLoop(elem, inputValues, world));
                t1.IsBackground = true;
                t1.Start();
                continue;
            }

            t2 = new Thread(o => instance.NeatInnerEvalulationLoop(elem, inputValues, world));
            t2.IsBackground = true;
            t2.Start();
        }
        while (t1.IsAlive || t2.IsAlive) ;
        toEvolve.Sort((el, el1) => el1.GetFitness().CompareTo(el.GetFitness()));
        generations[generations.Count - 1] = toEvolve;
        finished = true;
        Debug.Log("Neat loop ended!");
    }

    private void NeatInnerEvalulationLoop(T elem, Dictionary<int, double> inputValues, ThreadSafe.World world)
    {
        elem.RunAndEvaluate(inputValues, internalPopulation, world);
    }

    private void NeatInnerEvolvingLoop(T elem, int index)
    {
        elem.Mutate(historicalMarkings.GetHistoricalVariationAt(index));
        lock (next_generation)
        {
            next_generation.Add(elem);
        }
    }

    private void NeatInnerCrossoverLoop(T elem, T elem1, int index)
    {
        T offspring = (T)elem.Crossover(elem1);
        lock (next_generation)
        {
            next_generation.Add(offspring);
        }
    }

    private void NeatInnerRandomGenLoop(int index)
    {
        IGenotype<T1, K, A, A1> g = new T();
        lock (next_generation)
        {
            next_generation.Add((T)g.GenerateRandomly(historicalMarkings.GetHistoricalVariationAt(index)));
        }
    }
}
