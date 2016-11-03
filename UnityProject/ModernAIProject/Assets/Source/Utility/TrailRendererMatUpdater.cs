using UnityEngine;
using System.Collections;

public class TrailRendererMatUpdater : MonoBehaviour {

    // Use this for initialization
	void Awake () {
        Shader.EnableKeyword("_Color");
        Shader.EnableKeyword("_EmissionColor");
        Color32 objColor = new Color32((byte)StaticRandom.Rand(0, 256), (byte)StaticRandom.Rand(0,256), (byte)StaticRandom.Rand(0, 256), (byte)StaticRandom.Rand(0, 256));
        Color32 objEmission = new Color32((byte)StaticRandom.Rand(0, 256), (byte)StaticRandom.Rand(0, 256), (byte)StaticRandom.Rand(0, 256), (byte)StaticRandom.Rand(0, 256));
        Material objMaterial = new Material(Shader.Find("Standard"));
        objMaterial.SetColor("_Color", objColor);
        objMaterial.SetColor("_EmissionColor", objEmission);
        GetComponent<Renderer>().material = objMaterial;
        
        gameObject.AddComponent<TrailRenderer>();
        TrailRenderer trail = GetComponent<TrailRenderer>();
        trail.time = 100000000000000000;
        trail.material = objMaterial;
        trail.startWidth = 1;
        trail.startWidth = 1;
    }

}
