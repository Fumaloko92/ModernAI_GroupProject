using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GatheringPlace : MonoBehaviour {
    private List<Resource> resources;

	void Awake () {
        resources = new List<Resource>(GetComponentsInChildren<Resource>());
	}
	
    public void RemoveResource(Resource resource)
    {
        resources.Remove(resource);
    }
	// Update is called once per frame
	void Update () {
	
	}
}
