using UnityEngine;

public class Item2DAdjuster : MonoBehaviour
{
    private Vector3 itemScale = new Vector3(5f, 5f, 5f);
    public Color itemColorAdjust = new Color(0.5f, 0.5f, 0.5f, 1f);
    
    void Start()
    {
        string configFile;
        if (CharacterConfigurationLoader.getConfigFileNameFromPlayerPrefs && PlayerPrefs.HasKey(CharacterConfigurationLoader.configFilePlayerPrefsString))
            configFile = PlayerPrefs.GetString(CharacterConfigurationLoader.configFilePlayerPrefsString);
        else
            configFile = CharacterConfigurationLoader.configFile;
        INIParser ini = new INIParser();
        ini.Open(Application.dataPath + '/' + configFile);
        float itemScaleVal = (float)ini.ReadValue("Global", "ItemScale", 5.0);
        itemScale = new Vector3(itemScaleVal, itemScaleVal, itemScaleVal);
        ini.Close();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            child.localScale = itemScale;
            MeshRenderer render = child.GetComponentInChildren<MeshRenderer>();
            render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            if(render!= null) render.material.color = itemColorAdjust;
        }
    }
}
