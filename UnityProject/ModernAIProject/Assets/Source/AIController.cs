using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class AIController : MonoBehaviour {
    private NavMeshAgent myAgent;
    private World world;

    public Vector3 targetPosition;
    protected List<Resource> collectedResources;

    void Start()
    {
        myAgent = GetComponent<NavMeshAgent>();
        Vector3 v = world.GetPositionOfRandomFoodSource();
        v.y = 0;
        targetPosition = v;
    }   

    public void InitWorld(World world)
    {
        this.world = world;
    }
	
	// Update is called once per frame
	void Update () {
        myAgent.SetDestination(targetPosition);
        if(myAgent.remainingDistance <= 0.5)
        {
            Vector3 v = world.GetPositionOfRandomFoodSource();
            v.y = 0;
            targetPosition = v;
        }
    }
}
