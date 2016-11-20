using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour {
    private List<GatheringPlace> gatheringPlaces;
    private GameObject terrain;

    void Awake () {
        gatheringPlaces = new List<GatheringPlace>(GetComponentsInChildren<GatheringPlace>());
        terrain = GameObject.FindGameObjectWithTag("Terrain");
    }

    public Resource GetRandomResource(Material destinationMaterial)
    {
        List<Resource> availableRes = GetAvailableResources(); //get all available resources

        if(availableRes.Count > 0)
        {
            Resource resource = availableRes[StaticRandom.Rand(0,availableRes.Count)];

            resource.gameObject.name = "Destination";
            resource.GetComponent<MeshRenderer>().material = destinationMaterial;

            return resource;
        }
        else
        {
            return null;
        }
    }

    //gets all the resources which are still available
    List<Resource> GetAvailableResources()
    {
        List<Resource> availableRes = new List<Resource>();

        foreach (GatheringPlace place in gatheringPlaces)
        {
            foreach (Resource ress in place.GetComponentsInChildren<Resource>())
            {
                if (!ress.isTaken())
                {
                    availableRes.Add(ress);
                }
            }
        }

        return availableRes;
    }

    public bool resourcesAvailable()
    {
        bool available = false;

        foreach (GatheringPlace place in gatheringPlaces)
        {
            foreach (Resource ress in place.GetComponentsInChildren<Resource>())
            {
                if (!ress.isTaken())
                {
                    available = true;
                    break;
                }
            }
        }

        return available;
    }
    public void RemoveFromResourcePool(Resource resource)
    {
        foreach(GatheringPlace place in gatheringPlaces)
        {
            foreach(Resource ress in place.GetComponentsInChildren<Resource>())
            {
                if(ress.GetInstanceID().Equals(resource.GetInstanceID()))
                {
                    //remove resource from world - disabled for testing purposes
                    /*
                    Debug.Log("removed resource: " + resource.GetInstanceID());
                    resource.SetTaken(true);
                    place.RemoveResource(resource);
                    resource.gameObject.name = "Taken Resource";
                    resource.GetComponent<MeshRenderer>().material.color = Color.red;

                    //testing
                    Destroy(resource.gameObject);*/
                }
            }

        }
    }

    
}

