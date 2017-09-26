using UnityEngine;
using System.Collections;

public class BubbleRenderer : MonoBehaviour {
    public KeyCode keyboardInvisibilityBubbleButton;
    public string controllerInvisibilityBubbleButton;

    private bool previousKeyState = false;
    private bool previousControllerState = false;

    private bool visible;
    private MeshRenderer meshRenderer;
    public float verticalPosition = 0f;
    public float distance = 10f;

    void Start()
    {
        visible = true;
        meshRenderer = GetComponent<MeshRenderer>();
    }
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(transform.position.x, verticalPosition, transform.position.z);
        transform.localScale = new Vector3(distance*2/10, 1f, distance*2/10);
        if (keyboardInvisibilityBubbleButton != KeyCode.None && controllerInvisibilityBubbleButton != "")
        {
            bool keyState = Input.GetKey(keyboardInvisibilityBubbleButton);
            bool controllerState = Input.GetButton(controllerInvisibilityBubbleButton);
            if ((keyState && !previousKeyState) || (controllerState && !previousControllerState))
                visible = !visible;
            previousControllerState = controllerState;
            previousKeyState = keyState;

            meshRenderer.enabled = visible;
        }
    }
}
