using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// This class will be implemented by the QLearning and has to provide an implementation for the method 
/// Evaluate that will return the fitness of the element.
/// </summary>
public interface IEvaluator
{
    /// <summary>
    /// Function that returns an evaluation of the current element
    /// </summary>
    /// <param name="multipliers">The multipliers that are going to be used for the evaluation</param>
    /// <returns>Returns a value that determines how well the element performed</returns>
    float Evaluate(IDictionary<int,double> multipliers);

    void InitWorld(ThreadSafe.World world);
    void InitAIs(int AICount);
}

