using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class InputManager : MonoBehaviour {

    public static InputManager mainManager;


    public Vector3 mouseScreenPosition;
    public Vector3 mouseWorldPosition;
    public Vector3 mouseViewportPoint;
    public Ray mouseScreenRay;

    private float mouseZ;
    public float sensitivity = 1f;
    public float minDelta = 0.1f;

    public bool restrictToWindow = true;

    private string[] alternativeXAxisMappings = new string[] { "Mouse X", "Horizontal"};
    private string[] alternativeYAxisMappings = new string[] { "Mouse Y", "Vertical"};

    public Dictionary<ButtonType, KeyCode> keys = new Dictionary<ButtonType, KeyCode>()
                {
                    { ButtonType.Place, KeyCode.Space },
                    { ButtonType.Pick, KeyCode.Space },
                    { ButtonType.PickAll, KeyCode.P },
                    { ButtonType.NextItem, KeyCode.Q },
                    { ButtonType.NextEvent, KeyCode.E },
                    { ButtonType.NextState, KeyCode.A },
                    { ButtonType.InvisibilityBubble, KeyCode.O }
                };

    public Dictionary<ButtonType, string> buttons = new Dictionary<ButtonType, string>()
                {
                    { ButtonType.Place, "a" },
                    { ButtonType.Pick, "a" },
                    { ButtonType.PickAll, "back" },
                    { ButtonType.NextItem, "y" },
                    { ButtonType.NextEvent, "b" },
                    { ButtonType.NextState, "" },
                    { ButtonType.InvisibilityBubble, "" }
                };

    public Dictionary<ButtonType, int> mouse = new Dictionary<ButtonType, int>()
                {
                    { ButtonType.Place, 0 },
                    { ButtonType.Pick, 0 },
                    { ButtonType.PickAll, -1 },
                    { ButtonType.NextItem, -1 },
                    { ButtonType.NextEvent, -1 },
                    { ButtonType.NextState, -1 },
                    { ButtonType.InvisibilityBubble, -1 }
                };

    public Dictionary<ButtonType, bool> buttonDown;
    public bool[] debugButtonDowns;
    private Dictionary<ButtonType, bool> prevButtonDown;

    // Use this for initialization
    void Start() {
        if (mainManager == null)
            mainManager = this;

        mouseZ = Input.mousePosition.z;
        mouseScreenPosition = new Vector3(Screen.width / 2f, Screen.height / 2f, mouseZ);
        mouseWorldPosition = Camera.allCameras[0].ScreenToWorldPoint(mouseScreenPosition);

        Cursor.visible = false;

        buttonDown = new Dictionary<global::InputManager.ButtonType, bool>();
        prevButtonDown = new Dictionary<ButtonType, bool>();

        foreach (ButtonType type in Enum.GetValues(typeof(ButtonType)))
        {
            buttonDown.Add(type, false);
            prevButtonDown.Add(type, false);
        }

        debugButtonDowns = new bool[buttonDown.Count];
    }
	
	// Update is called once per frame
	void Update () {
        float deltaX = 0f;
        float deltaY = 0f;

        foreach (string alternativeXAxis in alternativeXAxisMappings)
            deltaX += Input.GetAxis(alternativeXAxis);

        foreach (string alternativeYAxis in alternativeYAxisMappings)
            deltaY += Input.GetAxis(alternativeYAxis);

        if (Mathf.Abs(deltaX) < minDelta) deltaX = 0;
        if (Mathf.Abs(deltaY) < minDelta) deltaY = 0;

        mouseScreenPosition = new Vector3(mouseScreenPosition.x + deltaX * sensitivity, mouseScreenPosition.y + deltaY * sensitivity, mouseZ);

        if(restrictToWindow)
        {
            if (mouseScreenPosition.x < Camera.allCameras[0].rect.xMin*Screen.width)
                mouseScreenPosition.x = Camera.allCameras[0].rect.xMin * Screen.width;
            if (mouseScreenPosition.y < Camera.allCameras[0].rect.yMin*Screen.height)
                mouseScreenPosition.y = Camera.allCameras[0].rect.yMin * Screen.height;
            if (mouseScreenPosition.x > Camera.allCameras[0].rect.xMax * Screen.width)
                mouseScreenPosition.x = Camera.allCameras[0].rect.xMax * Screen.width;
            if (mouseScreenPosition.y > Camera.allCameras[0].rect.yMax * Screen.height)
                mouseScreenPosition.y = Camera.allCameras[0].rect.yMax * Screen.height;
        }

        mouseWorldPosition = Camera.allCameras[0].ScreenToWorldPoint(mouseScreenPosition);
        mouseScreenRay = Camera.allCameras[0].ScreenPointToRay(mouseScreenPosition);
        mouseViewportPoint = Camera.allCameras[0].ScreenToViewportPoint(mouseScreenPosition);

        prevButtonDown = new Dictionary<ButtonType, bool>(buttonDown);

        int count = 0;
        foreach(ButtonType type in Enum.GetValues(typeof(ButtonType)))
        {
            bool key = Input.GetKey(keys[type]);
            bool btn = Input.GetButton(buttons[type]);
            bool mos = Input.GetMouseButton(mouse[type]!=-1?mouse[type]:0);
            buttonDown[type] = (keys[type]!=KeyCode.None ?key  :false) || 
                               (buttons[type]!=""        ?btn  :false) || 
                               (mouse[type]!=-1          ?mos  :false);
            debugButtonDowns[count] = buttonDown[type];
            count++;
        }
    }

    public bool isButtonDown(ButtonType type)
    {
        return buttonDown[type];
    }

    public bool isButtonUp(ButtonType type)
    {
        return !buttonDown[type];
    }

    public bool isButtonRisingEdge(ButtonType type)
    {
        return buttonDown[type] && !prevButtonDown[type];
    }

    public bool isButtonFallingEdge(ButtonType type)
    {
        return !buttonDown[type] && prevButtonDown[type];
    }

    public enum ButtonType { Place, Pick, PickAll, NextItem, NextEvent, NextState, InvisibilityBubble };
    public enum ButtonState { Down, Up, RisingEdge, FallingEdge }

    public bool GetButton(ButtonType type, ButtonState state)
    {
        switch (state)
        {
            case ButtonState.Down:
                return isButtonDown(type);
            case ButtonState.Up:
                return isButtonUp(type);
            case ButtonState.RisingEdge:
                bool tmp = isButtonRisingEdge(type);
                Debug.Log(tmp);
                return tmp;
            case ButtonState.FallingEdge:
                return isButtonFallingEdge(type);
        }
        return false;
    }

    void OnDisable()
    {
        Cursor.visible = true;
    }
}
