using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.VR;
using System;

public class FormValidator : MonoBehaviour {

    public InputField subIDText;
    public Dropdown trialDropDown;
    public Button practiceButton;
    public Button studyButton;
    public Button testButton;
    public Toggle inversionToggle;
    public Dropdown mode;

    void Start()
    {
        foreach (Camera c in Camera.allCameras)
            c.ResetAspect();
    }

	// Update is called once per frame
	void Update () {
	    if(subIDText.text.Length == 3 && trialDropDown.value != 0)
        {
            practiceButton.interactable = true;
            studyButton.interactable = true;
            testButton.interactable = true;
        }
        else
        {
            practiceButton.interactable = false;
            studyButton.interactable = false;
            testButton.interactable = false;
        }
	}

    private void SetPlayerPrefValues(int phase)
    {
        try
        {
            PlayerPrefs.SetString("sub", subIDText.text.Trim());
            PlayerPrefs.SetInt("trial", trialDropDown.value);
            PlayerPrefs.SetInt("phase", phase);
            PlayerPrefs.SetInt("inv", inversionToggle.isOn ? 1 : 0);
        }
        catch (Exception)
        {
            PlayerPrefs.SetString("sub", "999");
            PlayerPrefs.SetInt("trial", 0);
            PlayerPrefs.SetInt("phase", 0);
            PlayerPrefs.SetInt("inv", 0);
        }
    }

    public void BeginPractice()
    {
        int phase = -1;
        switch (mode.value)
        {
            case 0:
                phase = 0;
                VRSettings.enabled = true;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "practice.config");
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(1);
                break;
            case 1:
                phase = 3;
                VRSettings.enabled = false;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "practiceVE.config");
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(1);
                break;
            case 2:
                phase = 6;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "practice2d.config");
                VRSettings.enabled = false;
                foreach (Camera c in Camera.allCameras)
                    c.aspect = 1f;
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(3);
                break;
        }
    }

    public void BeginStudy()
    {
        int phase = -1;
        switch (mode.value)
        {
            case 0:
                phase = 1;
                VRSettings.enabled = true;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "study.config");
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(1);
                break;
            case 1:
                phase = 4;
                VRSettings.enabled = false;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "studyVE.config");
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(1);
                break;
            case 2:
                phase = 7;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "study2d.config");
                VRSettings.enabled = false;
                foreach (Camera c in Camera.allCameras)
                    c.aspect = 1f;
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(3);
                break;
        }
    }

    public void BeginTest()
    {
        int phase = -1;
        switch (mode.value)
        {
            case 0:
                phase = 2;
                VRSettings.enabled = true;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "test.config");
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(2);
                break;
            case 1:
                phase = 5;
                VRSettings.enabled = false;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "testVE.config");
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(2);
                break;
            case 2:
                phase = 8;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "test2d.config");
                VRSettings.enabled = false;
                foreach (Camera c in Camera.allCameras)
                    c.aspect = 1f;
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(4);
                break;
        }
    }


    public void BeginPractice(int mode)
    {
        int phase = -1;
        switch (mode)
        {
            case 0:
                phase = 0;
                VRSettings.enabled = true;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "practice.config");
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(1);
                break;
            case 1:
                phase = 3;
                VRSettings.enabled = false;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "practiceVE.config");
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(1);
                break;
            case 2:
                phase = 6;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "practice2d.config");
                VRSettings.enabled = false;
                foreach (Camera c in Camera.allCameras)
                    c.aspect = 1f;
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(3);
                break;
        }
    }

    public void BeginStudy(int mode)
    {
        int phase = -1;
        switch (mode)
        {
            case 0:
                phase = 1;
                VRSettings.enabled = true;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "study.config");
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(1);
                break;
            case 1:
                phase = 4;
                VRSettings.enabled = false;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "studyVE.config");
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(1);
                break;
            case 2:
                phase = 7;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "study2d.config");
                VRSettings.enabled = false;
                foreach (Camera c in Camera.allCameras)
                    c.aspect = 1f;
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(3);
                break;
        }
    }

    public void BeginTest(int mode)
    {
        int phase = -1;
        switch (mode)
        {
            case 0:
                phase = 2;
                VRSettings.enabled = true;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "test.config");
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(2);
                break;
            case 1:
                phase = 5;
                VRSettings.enabled = false;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "testVE.config");
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(2);
                break;
            case 2:
                phase = 8;
                PlayerPrefs.SetString(CharacterConfigurationLoader.configFilePlayerPrefsString, "test2d.config");
                VRSettings.enabled = false;
                foreach (Camera c in Camera.allCameras)
                    c.aspect = 1f;
                SetPlayerPrefValues(phase);
                SceneManager.LoadScene(4);
                break;
        }
    }
}
