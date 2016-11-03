using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class AIController : MonoBehaviour {
    protected bool activeAgent = true;
    protected NavMeshAgent myAgent;
    protected World world;
    protected List<Resource> collectedResources;
    protected Vector3 destination;
    protected Vector3 targetPosition;
    protected Resource targetResource;

    protected void InitializeAgent()
    {
        myAgent = GetComponent<NavMeshAgent>();
        collectedResources = new List<Resource>();
    }



    public void InitWorld(World world)
    {
        this.world = world;
    }
	
}
