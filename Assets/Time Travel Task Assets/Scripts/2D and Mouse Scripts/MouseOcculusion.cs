using UnityEngine;
using System.Collections;

public class MouseOcculusion : MonoBehaviour {
    public BoundaryManager boundaries;
    public bool changeColorWithBoundaries = true;
    private MeshRenderer render;
    public KeyCode keyboardInvisibilityBubbleButton = KeyCode.None;
    public float distance = 10f;
    public Material showTexture;
    public Material hideTexture;
    private bool showState = true;
	// Use this for initialization
	void Start () {
        render = GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.localScale = new Vector3(distance, distance, distance);
        Vector3 screenMouse = InputManager.mainManager.mouseWorldPosition;
        transform.position = new Vector3(screenMouse.x, transform.position.y, screenMouse.z);
        if (changeColorWithBoundaries)
            render.material.color = boundaries.renderers[0].material.color;
        if (InputManager.mainManager.GetButton(InputManager.ButtonType.InvisibilityBubble, InputManager.ButtonState.RisingEdge))
            if (showState)
            {
                render.material = hideTexture;
                showState = false;
            }
            else
            {
                render.material = showTexture;
                showState = true;
            }
    }
}
