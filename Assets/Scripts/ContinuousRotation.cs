using UnityEngine;
using System.Collections;

public class ContinuousRotation : MonoBehaviour {
    public float rotationSpeed = 0.1f;
    public Vector3 rotationPoint = new Vector3(0f, 0f, 0f);
    public Vector3 rotationAxis = new Vector3(0f, 1f, 0f);

	// Update is called once per frame
	void FixedUpdate () {
        transform.RotateAround(rotationPoint, rotationAxis, rotationSpeed);
    }
}
