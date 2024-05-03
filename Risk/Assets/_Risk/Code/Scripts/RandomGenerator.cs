using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGenerator
{
    private readonly string[] prefixes = new string[] { "Jam", "Han", "Solo", "Rom", "Ste" };
    private readonly string[] subfixes = new string[] { "es", "son", "mer", "son", "ve" };
    private readonly int seed = 1111;

    public string GenerateName()
    {
        return prefixes[Random.Range(0, prefixes.Length)] + subfixes[Random.Range(0, subfixes.Length)];
    }

    #region Singleton
    private static RandomGenerator _instance;
    public static RandomGenerator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new RandomGenerator();
            }
            return _instance;
        }
    }

    private RandomGenerator()
    {
        Random.InitState(seed);
    }
    #endregion
}
