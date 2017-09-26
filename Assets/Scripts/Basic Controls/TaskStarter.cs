using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TaskStarter : MonoBehaviour {
    private FormValidator validator;

    public KeyCode beginStudyKey;

    public KeyCode beginTestKey;

	// Use this for initialization
	void Start () {
        validator = new FormValidator();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(beginStudyKey))
            validator.BeginStudy(1);
        if (Input.GetKeyUp(beginTestKey))
            validator.BeginTest(1);
	}
}
