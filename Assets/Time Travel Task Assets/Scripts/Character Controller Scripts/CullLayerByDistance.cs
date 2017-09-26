using UnityEngine;
using System.Collections;

public class CullLayerByDistance : MonoBehaviour {

    public int layer = 0;
    public float dist = 1000;
    public bool cullSpherically = true;
    private Camera cam;
    private float[] distances;
    public KeyCode toggleInvisibilityButton;
    private bool prevKeyState = false;
    public bool invisibilityEnabled = true;

    // Use this for initialization
    void Start () {
        cam = GetComponent<Camera>();
        cam.layerCullSpherical = cullSpherically;
        distances = new float[32];
	}
	
	// Update is called once per frame
	void Update () {
        cam.layerCullSpherical = cullSpherically;
        if (invisibilityEnabled)
        {
            distances[layer] = dist;
            cam.layerCullDistances = distances;
        }
        else
        {
            distances[layer] = int.MaxValue;
            cam.layerCullDistances = distances;
        }
        if (toggleInvisibilityButton != KeyCode.None)
        {
            bool keyState = Input.GetKey(toggleInvisibilityButton);
            if (keyState && !prevKeyState)
                invisibilityEnabled = !invisibilityEnabled;
            prevKeyState = keyState;
        }
	}
}
