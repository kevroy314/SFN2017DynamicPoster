using UnityEngine;
using System.Collections;
using Chronos;

public class TimeController : MonoBehaviour {
    public string controllerTimeButtonString = "a";
    public KeyCode keyboardTimeButton = KeyCode.LeftControl;
    public GlobalClock clock;
    public float downTimeValue = -1f;
    public float upTimeValue = 1f;
    public float transitionDuration = 0.25f;
    private bool previousButtonState = false;
    public float simulationEndTimeLimit = 10f;
    public bool controlEnabled = true;

	// Update is called once per frame
	void Update () {
        if (controlEnabled)
        {
            bool currentButtonState = Input.GetButton(controllerTimeButtonString) || Input.GetKey(keyboardTimeButton);

            if (clock.time > simulationEndTimeLimit)
                clock.localTimeScale = 0f;

            if (!currentButtonState && clock.time < simulationEndTimeLimit)
                clock.localTimeScale = upTimeValue;
            else if (currentButtonState && clock.time > 0.5)
                clock.localTimeScale = downTimeValue;
            else
                clock.localTimeScale = 0;

            previousButtonState = currentButtonState;
        }
    }
}
