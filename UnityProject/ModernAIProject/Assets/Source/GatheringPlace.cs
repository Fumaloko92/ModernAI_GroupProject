using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GatheringPlace : MonoBehaviour {
    private List<Resource> resources;

	void Awake () {
        resources = new List<Resource>(GetComponentsInChildren<Resource>());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
