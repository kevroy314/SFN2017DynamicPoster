using UnityEngine;
using System.Collections;

public class RandomStartLocation : MonoBehaviour {
    public string playerPrefsString = "previousRandomTestPositionIndex";

    // Use this for initialization
    void Start () {
        string configFile;
        if (CharacterConfigurationLoader.getConfigFileNameFromPlayerPrefs && PlayerPrefs.HasKey(CharacterConfigurationLoader.configFilePlayerPrefsString))
            configFile = PlayerPrefs.GetString(CharacterConfigurationLoader.configFilePlayerPrefsString);
        else
            configFile = CharacterConfigurationLoader.configFile;
        INIParser ini = new INIParser();
        ini.Open(Application.dataPath + '/' + configFile);
        bool randomStartLocationEnabled = (float)ini.ReadValue("Character", "RandomStartLocationEnabled", 1) != 0;
        if (!randomStartLocationEnabled) return;
        int numStartPos = ini.ReadValue("Global", "NumStartPos", 0);
        string itemKey = "StartPos";
        string itemKeyRot = "StartRot";
        Vector3[] startPositions = new Vector3[numStartPos];
        Quaternion[] startRotations = new Quaternion[numStartPos];
        for(int i = 0; i < numStartPos; i++)
        {
            float x = (float)ini.ReadValue("StartPos", itemKey + i + "X", 0.0);
            float z = (float)ini.ReadValue("StartPos", itemKey + i + "Z", 0.0);
            float yRot = (float)ini.ReadValue("StartPos", itemKeyRot + i + "Y", 0.0);
            startPositions[i] = new Vector3(x, 2f, z);
            startRotations[i] = Quaternion.Euler(0f, yRot, 0f);
        }
        ini.Close();

        if(numStartPos == 0)
        {
            startPositions = new Vector3[1];
            startRotations = new Quaternion[1];
            startPositions[0] = Vector3.zero;
            startRotations[0] = Quaternion.identity;
        }


        if (!PlayerPrefs.HasKey(playerPrefsString))
            PlayerPrefs.SetInt(playerPrefsString, 0);

        int locationIndex = PlayerPrefs.GetInt(playerPrefsString);
        Debug.Log("Starting in location " + locationIndex);
        transform.localPosition = startPositions[locationIndex];
        transform.localRotation = startRotations[locationIndex];

        int nextIndex = (locationIndex + 1) % startPositions.Length;
        PlayerPrefs.SetInt(playerPrefsString, nextIndex);
    }
}
