using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour {
    List<GatheringPlace> gatheringPlaces;
	
	void Awake () {
        gatheringPlaces = new List<GatheringPlace>(GetComponentsInChildren<GatheringPlace>());
    }
	
	public Vector3 GetPositionOfRandomFoodSource()
    {
        foreach (GatheringPlace place in gatheringPlaces)
            foreach (Food food in place.GetComponentsInChildren<Food>())
                if (Random.Range(0f, 1f) < 0.5)
                    return food.gameObject.transform.position;
        return Vector3.zero;
    }
}
