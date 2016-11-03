using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour {
    private List<GatheringPlace> gatheringPlaces;
    private GameObject terrain;
    private Vector3[,] grid;

    public Vector3[,] Grid
    {
        get
        {
            return grid;
        }
        private set
        {
            grid = value;
        }
    }

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
    public void RemoveFromResourcePool(Resource resource)
    {
        foreach(GatheringPlace place in gatheringPlaces)
        {
            foreach(Resource ress in place.GetComponentsInChildren<Resource>())
            {
                if(ress.GetInstanceID().Equals(resource.GetInstanceID()))
                {
                    Debug.Log("removed resource: " + resource.GetInstanceID());
                    resource.SetTaken(true);
                    place.RemoveResource(resource);
                    resource.gameObject.name = "Taken Resource";
                    resource.GetComponent<MeshRenderer>().material.color = Color.red;
                }
            }

        }
    }

    public void InitializeGrid(int gridHeight, int gridWidth, GameObject placeholder, bool visible)
    {
        grid = new Vector3[gridHeight,gridWidth];
        Vector3 pos = terrain.transform.position;
        
        float step_i = terrain.GetComponent<Terrain>().terrainData.size.z / (gridHeight-1);
        float step_k = terrain.GetComponent<Terrain>().terrainData.size.x / (gridWidth-1);
        for(int i=0;i<gridHeight;i++)
            for(int k=0;k<gridWidth;k++)
            {
                Vector3 v = new Vector3(i*step_i,0,k*step_k);
                v += pos;
                v.y = terrain.GetComponent<Terrain>().SampleHeight(v);
                GameObject g = (GameObject)Instantiate(placeholder, v,Quaternion.identity);
                g.name = "[" + i + "," + k + "]";
                grid[i, k] = g.transform.position;
                if (!visible)
                    g.GetComponent<Renderer>().enabled = false;
            }
    }
}

