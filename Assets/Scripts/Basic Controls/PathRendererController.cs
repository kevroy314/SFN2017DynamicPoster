using UnityEngine;
using System.Collections;

public class PathRendererController : MonoBehaviour {

    public KeyCode loopToggle;
    public KeyCode increaseSpeedButton;
    public KeyCode decreaseSpeedButton;
    public KeyCode pauseToggle;
    public KeyCode resetSpeed;

    public float speedDelta = 0.01f;

    private float speedStored;
    private float initialSpeed;

    public PathRenderer pathRenderer;

	// Use this for initialization
	void Start () {
        speedStored = pathRenderer.animationSpeed;
        initialSpeed = pathRenderer.animationSpeed;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(loopToggle))
            pathRenderer.loop = !pathRenderer.loop;
        if (Input.GetKey(increaseSpeedButton))
            pathRenderer.animationSpeed += speedDelta;
        if (Input.GetKey(decreaseSpeedButton))
            pathRenderer.animationSpeed -= speedDelta;
        if (Input.GetKeyUp(pauseToggle))
        {
            if (pathRenderer.animationSpeed == 0f)
                pathRenderer.animationSpeed = speedStored;
            else
            {
                speedStored = pathRenderer.animationSpeed;
                pathRenderer.animationSpeed = 0f;
            }
        }
        if (Input.GetKeyUp(resetSpeed))
            pathRenderer.animationSpeed = initialSpeed;
    }
}
