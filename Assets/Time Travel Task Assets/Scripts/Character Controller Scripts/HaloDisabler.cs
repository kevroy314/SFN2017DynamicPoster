using UnityEngine;
using System.Collections;

public class HaloDisabler : MonoBehaviour {
    private Behaviour b;
    private MeshRenderer r;
	// Use this for initialization
	void Start () {
        b = (Behaviour)GetComponent("Halo");
        r = GetComponent<MeshRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        b.enabled = r.isVisible;
    }
}
