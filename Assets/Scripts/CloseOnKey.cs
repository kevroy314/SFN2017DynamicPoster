using UnityEngine;
using System.Collections;

public class CloseOnKey : MonoBehaviour {

    public KeyCode closeKey;

	// Update is called once per frame
	void Update () {
	    if (Input.GetKey(closeKey))
            Application.Quit();
	}
}
