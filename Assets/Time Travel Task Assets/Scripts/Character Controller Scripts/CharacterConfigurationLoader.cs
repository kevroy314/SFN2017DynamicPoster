using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class CharacterConfigurationLoader : MonoBehaviour {

    public BinaryLogger binaryLogger;
    public MouseOcculusion mouseOcculsion;

    public static string configFile = "simulation.config";
    public static bool getConfigFileNameFromPlayerPrefs = true;
    public static string configFilePlayerPrefsString = "configFile";

    // Use this for initialization
    void Start() {
        transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
        if (getConfigFileNameFromPlayerPrefs && PlayerPrefs.HasKey(configFilePlayerPrefsString))
            configFile = PlayerPrefs.GetString(configFilePlayerPrefsString);

        FirstPersonController controller = GetComponent<FirstPersonController>();
        TimeController tcontroller = GetComponent<TimeController>();
        AudioSource audio = GetComponent<AudioSource>();
        ItemClickController clicker = GetComponent<ItemClickController>();
        InventoryManager inventory = GetComponent<InventoryManager>();
        BubbleRenderer bubble = GetComponentInChildren<BubbleRenderer>();
        TemporalImagingEffect vig = transform.parent.gameObject.GetComponentInChildren<TemporalImagingEffect>();
        CullLayerByDistance culler = transform.parent.gameObject.GetComponentInChildren<CullLayerByDistance>();
        AugmentedController augControl = GetComponent<AugmentedController>();

        binaryLogger.keys = new System.Collections.Generic.List<KeyCode>();
        binaryLogger.buttons = new System.Collections.Generic.List<string>();

        INIParser ini = new INIParser();
        ini.Open(Application.dataPath + '/' + configFile);

        float forwardTimeSpeed = (float)ini.ReadValue("Character", "TimeForwardSpeed", 1.0);
        float backwardTimeSpeed = (float)ini.ReadValue("Character", "TimeBackwardSpeed", -1.0);
        float timeTransitionDuration = (float)ini.ReadValue("Character", "TimeTransitionDuration", 0.25);

        float endTime = (float)ini.ReadValue("Global", "EndTime", 120);

        float invisibilityDistance = (float)ini.ReadValue("Global", "ItemInvisibilityDistance", 1000);

        string timeKeyString = ini.ReadValue("Character", "KeyboardTimeButton", "LeftControl");
        KeyCode timeKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), timeKeyString);
        string controllerTimeButton = ini.ReadValue("Character", "ControllerTimeButton", "x");
        binaryLogger.keys.Add(timeKey);
        binaryLogger.buttons.Add(controllerTimeButton);

        bool stepSoundEnabled = ini.ReadValue("Character", "StepSoundEnabled", 1) != 0;

        float vignetteStrength = (float)ini.ReadValue("Character", "VignetteStrength", 0.25);

        float walkSpeed = (float)ini.ReadValue("Character", "WalkSpeed", 5.0);
        float mouseLookSensitivity = (float)ini.ReadValue("Character", "MouseLookSensitivity", 2.0);

        string keyboardClickButtonString = ini.ReadValue("Character", "KeyboardClickButton", "Space");
        KeyCode keyboardClickButton = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyboardClickButtonString);
        string controllerClickButton = ini.ReadValue("Character", "ControllerClickButton", "a");
        binaryLogger.keys.Add(keyboardClickButton);
        binaryLogger.buttons.Add(controllerClickButton);

        float itemClickDistance = (float)ini.ReadValue("Character", "ItemClickDistance", 3.0);

        string keyboardNextItemButtonString = ini.ReadValue("Character", "KeyboardNextItemButton", "Q");
        KeyCode keyboardNextItemButton = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyboardNextItemButtonString);
        string controllerNextItemButton = ini.ReadValue("Character", "ControllerNextItemButton", "y");
        binaryLogger.keys.Add(keyboardNextItemButton);
        binaryLogger.buttons.Add(controllerNextItemButton);

        float itemPlaceDistance = (float)ini.ReadValue("Character", "ItemPlaceDistance", 3.0);

        string keyboardPickUpAllButtonString = ini.ReadValue("Character", "KeyboardPickUpAllButton", "P");
        KeyCode keyboardPickUpAllButton = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyboardPickUpAllButtonString);
        string controllerPickUpAllButton = ini.ReadValue("Character", "ControllerPickUpAllButton", "back");
        binaryLogger.keys.Add(keyboardPickUpAllButton);
        binaryLogger.buttons.Add(controllerPickUpAllButton);

        string keyboardNextItemTypeButtonString = ini.ReadValue("Character", "KeyboardNextItemTypeButton", "E");
        KeyCode keyboardNextItemTypeButton = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyboardNextItemTypeButtonString);
        string controllerNextItemTypeButton = ini.ReadValue("Character", "ControllerNextItemTypeButton", "b");
        binaryLogger.keys.Add(keyboardNextItemTypeButton);
        binaryLogger.buttons.Add(controllerNextItemTypeButton);

        string keyboardInvisibilityBubbleButtonString = ini.ReadValue("Character", "KeyboardInvisibilityBubbleButton", "");
        KeyCode keyboardInvisibilityBubbleButton;
        if (keyboardInvisibilityBubbleButtonString == "")
            keyboardInvisibilityBubbleButton = KeyCode.None;
        else
            keyboardInvisibilityBubbleButton = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyboardInvisibilityBubbleButtonString);
        string controllerInvisibilityBubbleButton = ini.ReadValue("Character", "ControllerInvisibilityBubbleButton", "");
        binaryLogger.keys.Add(keyboardInvisibilityBubbleButton);
        binaryLogger.buttons.Add(controllerInvisibilityBubbleButton);

        string toggleInvisibilityButtonString = ini.ReadValue("Global", "ToggleInvisibilityButton", "");
        KeyCode toggleInvisibilityButton;
        if (toggleInvisibilityButtonString == "")
            toggleInvisibilityButton = KeyCode.None;
        else
            toggleInvisibilityButton = (KeyCode)System.Enum.Parse(typeof(KeyCode), toggleInvisibilityButtonString);
        binaryLogger.keys.Add(toggleInvisibilityButton);

        bool invisibilityOnAtStart = ini.ReadValue("Global", "InvisibilityOnAtStart", 1) != 0;
        ////////////////////
        string keyboardRotationButtonString = ini.ReadValue("Character", "KeyboardRotationButton", "R");
        KeyCode keyboardRotationButton = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyboardRotationButtonString);
        binaryLogger.keys.Add(keyboardRotationButton);

        float defaultLookAngle = (float)ini.ReadValue("Character", "DefaultLookAngle", 0.2);
        float headHeight = (float)ini.ReadValue("Character", "HeadHeight", 0.9);
        float forwardSpeed = (float)ini.ReadValue("Character", "ForwardSpeed", 3);
        float minAxisTilt = (float)ini.ReadValue("Character", "MinAxisTilt", 0.01);
        float minMovementOffset = (float)ini.ReadValue("Character", "MinMovementOffset", 0.1);

        bool rotationEnabled = ini.ReadValue("Character", "RotationEnabled", 0) != 0;


        if (augControl != null)
        {
            augControl.rotationButtonKey = keyboardRotationButton;
            augControl.defaultLookAngle = defaultLookAngle;
            augControl.headHeight = headHeight;
            augControl.forwardSpeed = forwardSpeed;
            augControl.minAxisTilt = minAxisTilt;
            augControl.minMovementOffset = minMovementOffset;
            augControl.rotationEnabled = rotationEnabled;
        }

        if (clicker != null) {
            clicker.keyClickButton = keyboardClickButton;
            clicker.controllerClickButton = controllerClickButton;
            clicker.minClickDistance = itemClickDistance;
        }

        if (inventory != null)
        {
            //inventory.placeKeyCode = keyboardClickButton;
            InputManager.mainManager.keys[InputManager.ButtonType.Place] = keyboardClickButton;
            InputManager.mainManager.keys[InputManager.ButtonType.Pick] = keyboardClickButton;
            //inventory.placeButtonString = controllerClickButton;
            InputManager.mainManager.buttons[InputManager.ButtonType.Place] = controllerClickButton;
            InputManager.mainManager.buttons[InputManager.ButtonType.Pick] = controllerClickButton;
            //inventory.nextKeyCode = keyboardNextItemButton;
            InputManager.mainManager.keys[InputManager.ButtonType.NextItem] = keyboardNextItemButton;
            //inventory.nextButtonString = controllerNextItemButton;
            InputManager.mainManager.buttons[InputManager.ButtonType.NextItem] = controllerNextItemButton;
            inventory.placeDistance = itemPlaceDistance;
            //inventory.pickUpAllCode = keyboardPickUpAllButton;
            InputManager.mainManager.keys[InputManager.ButtonType.PickAll] = keyboardPickUpAllButton;
            //inventory.pickUpAllButtonString = controllerPickUpAllButton;
            InputManager.mainManager.buttons[InputManager.ButtonType.PickAll] = controllerPickUpAllButton;
            //inventory.nextItemTypeKeyCode = keyboardNextItemTypeButton;
            InputManager.mainManager.keys[InputManager.ButtonType.NextEvent] = keyboardNextItemTypeButton;
            //inventory.nextItemTypeButtonString = controllerNextItemTypeButton;
            InputManager.mainManager.buttons[InputManager.ButtonType.NextEvent] = controllerNextItemTypeButton;
        }

        if (tcontroller != null)
        {
            tcontroller.simulationEndTimeLimit = endTime;

            tcontroller.upTimeValue = forwardTimeSpeed;
            tcontroller.downTimeValue = backwardTimeSpeed;
            tcontroller.transitionDuration = timeTransitionDuration;

            tcontroller.controllerTimeButtonString = controllerTimeButton;
            tcontroller.keyboardTimeButton = timeKey;
        }
        if (vig != null)
        {
            vig.negativeVignett = vignetteStrength;
            vig.transitionDuration = timeTransitionDuration;
        }

        if (audio != null)
        {
            audio.enabled = stepSoundEnabled;
        }

        if (controller != null)
        {
            controller.setMovementSpeed(walkSpeed);
            controller.setMouseSensitivity(mouseLookSensitivity);
        }

        if (culler != null)
        {
            culler.dist = invisibilityDistance;
            culler.toggleInvisibilityButton = toggleInvisibilityButton;
            culler.invisibilityEnabled = invisibilityOnAtStart;
        }

        if(bubble != null)
        {
            bubble.keyboardInvisibilityBubbleButton = keyboardInvisibilityBubbleButton;
            bubble.controllerInvisibilityBubbleButton = controllerInvisibilityBubbleButton;
            bubble.distance = invisibilityDistance;
        }

        if (mouseOcculsion != null)
        {
            InputManager.mainManager.keys[InputManager.ButtonType.InvisibilityBubble] = toggleInvisibilityButton;
            mouseOcculsion.distance = invisibilityDistance;
        }

        ini.Close();
    }
}
