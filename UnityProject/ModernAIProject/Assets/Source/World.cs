using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour {

    private List<Vector3> resourcePositions;
    public List<Vector3> ResourcePositions { get { return resourcePositions; } }

    private List<Resource> ressources = new List<Resource>();
    private int resourceCount;
    public int ResourceCount { get { return resourceCount; } }

    private GameObject terrain;
    private Vector2 terrainSize;
    public Vector2 TerrainSize { get { return terrainSize; } }

    void Awake () {

        terrain = GameObject.FindGameObjectWithTag("Terrain");
        terrainSize = new Vector2(terrain.GetComponent<Terrain>().terrainData.size.x, terrain.GetComponent<Terrain>().terrainData.size.z);

        resourceCount = 1000;// StaticRandom.Rand(50, 1000);
        resourcePositions = new List<Vector3>();
        while (resourceCount > 0)
        {
            

            Vector3 pos = new Vector3((float)StaticRandom.Sample() * terrainSize.x, 0, (float)StaticRandom.Sample() * terrainSize.y);
            pos = new Vector3(pos.x, terrain.GetComponent<Terrain>().SampleHeight(pos), pos.z);


            resourcePositions.Add(pos);

            GameObject resObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            resObject.transform.position = pos;
            resObject.AddComponent<Food>();

            ressources.Add(resObject.GetComponent<Resource>());

            resourceCount--;
        }
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

    public Resource GetRandomResource()
    {
        List<Resource> availableRes = GetAvailableResources(); //get all available resources

        if (availableRes.Count > 0)
        {
            Resource resource = availableRes[StaticRandom.Rand(0, availableRes.Count)];
            
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

        foreach (Resource ress in ressources)
        {
            if (!ress.isTaken())
            {
                availableRes.Add(ress);
            }
        }

        return availableRes;
    }

    public bool resourcesAvailable()
    {
        bool available = false;

        foreach (Resource ress in ressources)
        {
            if (!ress.isTaken())
            {
                available = true;
                break;
            }
        }

        return available;
    }
    public void RemoveFromResourcePool(Resource resource)
    {
        foreach(Resource ress in ressources)
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

