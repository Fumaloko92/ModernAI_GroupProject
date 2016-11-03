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
        bool resAvailable = false;

        foreach (GatheringPlace place in gatheringPlaces)
        {
            foreach (Resource ress in place.GetComponentsInChildren<Resource>())
            {
                if (!ress.isTaken())
                {
                    resAvailable = true;
                    break;
                }
            }
            if(resAvailable)
            {
                break;
            }
        }

        while (resAvailable) 
        {
            foreach (GatheringPlace place in gatheringPlaces)
            {
                foreach (Resource ress in place.GetComponentsInChildren<Resource>())
                {
                    if (!ress.isTaken() && Random.Range(0f, 1f) < 0.5)
                    {
                        ress.gameObject.name = "Destination";
                        ress.GetComponent<MeshRenderer>().material = destinationMaterial;

                        return ress;
                    }
                }
            }
        }

        
        return null;
        
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

