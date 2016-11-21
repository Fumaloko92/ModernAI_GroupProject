using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Genotype = NeuralNetworkG<StringNodeRepr, StringConnectionRepr, Sigmoid, Sigmoid>;
using Neat = NEAT<NeuralNetworkG<StringNodeRepr, StringConnectionRepr,Sigmoid,Sigmoid>>;

public class Test : MonoBehaviour {
	// Use this for initialization
	void Start () {
        Neat.Initialize(100, 1000);
        Debug.Log("Generated");
    }
	
   /* IEnumerator wait()
    {
        while (true)
        {
            Neat.RunGeneration(generation);
            generation = Neat.EvolveGeneration(generation);
            yield return new WaitForSeconds(5f);
        }
    }*/

	// Update is called once per frame
	void Update () {
        List<Genotype> gen = Neat.GetGenerationNumber(50);
        if (gen != null)
            Debug.Log("There is something");
    }

    void OnApplicationQuit()
    {
        Neat.AbortThreads();
    }
}
