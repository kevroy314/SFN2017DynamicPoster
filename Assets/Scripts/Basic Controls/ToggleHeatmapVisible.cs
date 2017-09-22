using UnityEngine;
using System.Collections;

public class ToggleHeatmapVisible : MonoBehaviour {

    public KeyCode toggleKey;

    public RayMarching heatmap;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(toggleKey))
            heatmap.enabled = !heatmap.enabled;
    }
}
