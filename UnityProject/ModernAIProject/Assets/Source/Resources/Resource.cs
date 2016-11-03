using UnityEngine;
using System.Collections;

public abstract class Resource : MonoBehaviour {

    bool taken = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public Vector3 GetPositionInWorld()
    {
        Vector3 pos = transform.position;
        pos.y = Terrain.activeTerrain.SampleHeight(pos);

        return pos;
    }
    public void SetTaken(bool taken)
    {
        this.taken = taken;
    }
    public bool isTaken()
    {
        return taken;
    }
}
