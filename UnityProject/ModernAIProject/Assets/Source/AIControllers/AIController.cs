using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class AIController : MonoBehaviour {
    protected NavMeshAgent myAgent;
    protected World world;
    protected List<Resource> collectedResources;
    protected Vector3 destination;
    protected Vector3 targetPosition;


    protected void InitializeAgent()
    {
        myAgent = GetComponent<NavMeshAgent>();
    }



    public void InitWorld(World world)
    {
        this.world = world;
    }
	
}
