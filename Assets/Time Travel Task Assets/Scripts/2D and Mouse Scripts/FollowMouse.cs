using UnityEngine;
using System.Collections;

public class FollowMouse : MonoBehaviour {

    private RectTransform rect;

    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

	// Update is called once per frame
	void Update () {

        rect.anchoredPosition = Vector3.Scale(InputManager.mainManager.mouseViewportPoint, 
            new Vector3(Camera.allCameras[0].pixelWidth, Camera.allCameras[0].pixelHeight, 1f));
	}
}
