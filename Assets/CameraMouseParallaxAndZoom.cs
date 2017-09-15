using UnityEngine;
using System.Collections;

public class CameraMouseParallaxAndZoom : MonoBehaviour {
    public float horizontalParallax = 0.1f;
    public float verticalParallax = 0.1f;

    public Camera cam;
    public Transform target;

    public Vector2 offset;
    public float distance;

    public float defaultDistance = -5f;

    public float minDistance;
    public float maxDistance;
    public float zoomSpeed;

    private float defaultFoV = 60;

    void Start() {
        defaultFoV = cam.fieldOfView;
        distance = defaultFoV;
    }

    // Update is called once per frame
    void Update () {
        offset = new Vector2((Input.mousePosition.x - cam.pixelWidth / 2f) / cam.pixelWidth * 2f, (Input.mousePosition.y - cam.pixelHeight / 2f) / cam.pixelHeight * 2f);
        cam.transform.LookAt(target);
        distance += -Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
        cam.transform.position = new Vector3(defaultDistance, offset.y * verticalParallax, offset.x * horizontalParallax);
        if (Input.GetMouseButton(2))
            distance = defaultFoV;
        cam.fieldOfView = distance;
	}
}
