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

    public Vector3 GetRandomFoodSource(Material destinationMaterial)
    {
        while (true) { 
            foreach (GatheringPlace place in gatheringPlaces)
                foreach (Food food in place.GetComponentsInChildren<Food>())
                    if (Random.Range(0f, 1f) < 0.5) {
                        food.gameObject.name = "Destination";
                        food.GetComponent<MeshRenderer>().material = destinationMaterial;
                        Vector3 pos = food.gameObject.transform.position;
                        pos.y = terrain.GetComponent<Terrain>().SampleHeight(pos) + 5;
                        pos.y = Terrain.activeTerrain.SampleHeight(pos);
                        return pos;
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
                v.y = terrain.GetComponent<Terrain>().SampleHeight(v)+5;
                GameObject g = (GameObject)Instantiate(placeholder, v,Quaternion.identity);
                g.name = "[" + i + "," + k + "]";
                grid[i, k] = g.transform.position;
                if (!visible)
                    g.GetComponent<Renderer>().enabled = false;
            }
    }
}

