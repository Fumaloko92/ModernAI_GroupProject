using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

public class NEAT<T> where T : IGenotype, new()
{
    static private NEAT<T> instance;
    static private Thread NEAT_THREAD = null;
    static private List<List<T>> generations;
    static private int population;
    static private int number_generations;
    static private List<T> next_generation;
    static private Thread t1, t2;
    static private List<T> toEvolve;

    static public void Initialize(int n_generations, int population)
    {
        number_generations = n_generations;
        NEAT<T>.population = population;
        instance = new NEAT<T>();
        NEAT_THREAD = new Thread(instance.RunNeatLoop);
        NEAT_THREAD.Start();
    }

    private NEAT()
    {
        generations = new List<List<T>>();
        next_generation = new List<T>();
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
        if (n < generations.Count)
            return generations[n];
        else
            return null;
    }

    private void RunNeatLoop()
    {
        t1 = null; t2 = null;
        List<T> current_generation = new List<T>();
        IGenotype g = new T();
        for (int i = 0; i < population; i++)
            current_generation.Add((T)g.GenerateRandomly());
        lock (generations)
        {
            generations.Add(current_generation);
        }

        //Debug.Log("Generated starting generation");
        for (int i = 1; i < number_generations; i++)
        {
           
            toEvolve = generations[i - 1];
            // Debug.Log("Evaluating " + i + " generation");
            Debug.Log("ToEvolve size: " + toEvolve.Count);
            foreach (T elem in toEvolve)
            {
                while (t1 != null && t1.IsAlive && t2 != null && t2.IsAlive) ;
                if (t1 == null || !t1.IsAlive)
                {
                    t1 = new Thread(o => instance.NeatInnerEvalulationLoop(elem));
                    t1.IsBackground = true;
                    t1.Start();
                    continue;
                }

                t2 = new Thread(o => instance.NeatInnerEvalulationLoop(elem));
                t2.IsBackground = true;
                t2.Start();
            }
            while (t1.IsAlive || t2.IsAlive) ;
            toEvolve.Sort((el, el1) => el.GetFitness().CompareTo(el1.GetFitness()));

            int targetSize = toEvolve.Count;
            Debug.Log("Evolving " + i + " generation");
            for (int k = 0; k < targetSize; k++)
            {

                if (StaticRandom.Sample() < (float)(targetSize - k) / (float)targetSize)
                {
                    while (t1.IsAlive && t2.IsAlive) ;
                    if (!t1.IsAlive)
                    {
                        t1 = new Thread(o => instance.NeatInnerEvolvingLoop((T)toEvolve[k].Clone()));
                        t1.IsBackground = true;
                        t1.Start();
                        continue;
                    }
                    t2 = new Thread(o => instance.NeatInnerEvolvingLoop((T)toEvolve[k].Clone()));
                    t2.IsBackground = true;
                    t2.Start();
                }
                else
                {
                    while (t1.IsAlive && t2.IsAlive) ;
                    if (!t1.IsAlive)
                    {
                        t1 = new Thread(instance.NeatInnerRandomGenLoop);
                        t1.IsBackground = true;
                        t1.Start();
                        continue;
                    }
                    t2 = new Thread(instance.NeatInnerRandomGenLoop);
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
        Debug.Log("Neat loop ended!");
    }

    private void NeatInnerEvalulationLoop(T elem)
    {

            elem.RunAndEvaluate();

    }

    private void NeatInnerEvolvingLoop(T elem)
    {
        elem.Mutate();
        lock (next_generation)
        {
            next_generation.Add(elem);
        }
    }

    private void NeatInnerRandomGenLoop()
    {
        IGenotype g = new T();
        lock (next_generation)
        {
            next_generation.Add((T)g.GenerateRandomly());
        }
    }

}
