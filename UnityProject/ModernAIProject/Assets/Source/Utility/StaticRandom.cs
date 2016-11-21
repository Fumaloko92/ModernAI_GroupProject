using System;

public static class StaticRandom
{
    static readonly System.Random random = new System.Random();

    /// <summary>
    /// Returns a random number between f(included) and t(excluded)
    /// </summary>
    /// <param name="f"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static int Rand(int f, int t)
    {
        lock(random)
        {
            return random.Next(f, t);
        }
    }

    public static double Sample()
    {
        lock(random)
        {
            return random.NextDouble();
        }
    }
}
