using UnityEngine;
using System.Collections;

public class BoundaryColorChanger : MonoBehaviour {

    public PathRenderer pathRenderer;

    public Renderer[] boundaryRenderers;

    public float time;

	// Update is called once per frame
	void Update () {
        time = pathRenderer.CurrentTime;
        if (time < 15f)
            SetBoundaryColors(Color.yellow);
        else if(time < 30f)
            SetBoundaryColors(Color.red);
        else if (time < 45f)
            SetBoundaryColors(Color.green);
        else
            SetBoundaryColors(Color.blue);
    }

    void SetBoundaryColors(Color c)
    {
        for(int i = 0; i < boundaryRenderers.Length; i++)
            boundaryRenderers[i].material.color = c;
    }
}
