using UnityEngine;
using System.Collections;

public class AmbientConfigurationLoader : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        string configFile;
        if (CharacterConfigurationLoader.getConfigFileNameFromPlayerPrefs && PlayerPrefs.HasKey(CharacterConfigurationLoader.configFilePlayerPrefsString))
            configFile = PlayerPrefs.GetString(CharacterConfigurationLoader.configFilePlayerPrefsString);
        else
            configFile = CharacterConfigurationLoader.configFile;
        INIParser ini = new INIParser();
        ini.Open(Application.dataPath + '/' + configFile);
        gameObject.GetComponent<AudioSource>().enabled = ini.ReadValue("Global", "EnableAmbient", 1) != 0;
        gameObject.GetComponent<AudioSource>().volume = (float)ini.ReadValue("Global", "AmbientVolume", 0.0);
        ini.Close();
    }
}
