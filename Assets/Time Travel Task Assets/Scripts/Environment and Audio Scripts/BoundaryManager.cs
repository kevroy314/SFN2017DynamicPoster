using UnityEngine;
using System.Collections;
using Chronos;
using System;

public class BoundaryManager : MonoBehaviour {
    private string configFile = CharacterConfigurationLoader.configFile;
    public Timeline time;
    public MeshRenderer[] renderers;
    private Color[] boundaryColors;
    private float[] delays;
    public float transitionDuration = 2f;
    public int currentState = 0;

    private Color[] profile;
    public float profileInterval = 0.003f;

    public AnimationCurve r;
    public AnimationCurve g;
    public AnimationCurve b;

    public bool invertTimeline = false;

    // Use this for initialization
    void Start() {
        if (PlayerPrefs.HasKey("LoaderInversion"))
            invertTimeline = PlayerPrefs.GetInt("LoaderInversion") != 0;

        if (CharacterConfigurationLoader.getConfigFileNameFromPlayerPrefs && PlayerPrefs.HasKey(CharacterConfigurationLoader.configFilePlayerPrefsString))
            configFile = PlayerPrefs.GetString(CharacterConfigurationLoader.configFilePlayerPrefsString);
        else
            configFile = CharacterConfigurationLoader.configFile;
        INIParser ini = new INIParser();
        ini.Open(Application.dataPath + '/' + configFile);

        int numBoundaryColors = ini.ReadValue("Global", "NumBoundaryColors", 4);
        float duration = (float)ini.ReadValue("Global", "BoundaryColorTransitionDuration", transitionDuration);

        transitionDuration = duration;
        boundaryColors = new Color[numBoundaryColors];
        delays = new float[numBoundaryColors];
        string key = "Boundary";
        for (int i = 0; i < numBoundaryColors; i++)
        {
            string colorString = ini.ReadValue("BoundaryColors", key + i + "Color", "");
            float delay = (float)ini.ReadValue("BoundaryColors", key + i + "Delay", 0.0);
            string[] colorStringSplit = colorString.Split(new string[] { "(", ")", "," }, System.StringSplitOptions.RemoveEmptyEntries);
            Color color = renderers[0].material.color;
            try
            {
                if (colorString != "")
                    color = new Color(float.Parse(colorStringSplit[0]) / 255f, float.Parse(colorStringSplit[1]) / 255f, float.Parse(colorStringSplit[2]) / 255f);
            }
            catch (System.Exception) { }
            boundaryColors[i] = color;
            delays[i] = delay;
        }

        ini.Close();

        if (invertTimeline)
            Array.Reverse(boundaryColors);

        float endTime = delays[delays.Length - 1] + transitionDuration / 2 + 1;
        int numPoints = (int)Mathf.Ceil(endTime / profileInterval);
        profile = new Color[numPoints];
        int state = 0;
        for (int i = 0; i < profile.Length; i++)
        {
            float t = i * profileInterval;
            profile[i] = boundaryColors[state];
            if (state + 1 < boundaryColors.Length && t > delays[state + 1])
                state++;
        }
        //Add color lerps
        for (int i = 1; i < boundaryColors.Length; i++)
        {
            int startIndex = Mathf.FloorToInt((delays[i] - (transitionDuration/2)) / profileInterval);
            int endIndex = Mathf.CeilToInt((delays[i] + (transitionDuration / 2)) / profileInterval);
            for (int j = startIndex; j <= endIndex; j++) {
                float lerp = (((float)j - (float)startIndex) / ((float)endIndex - (float)startIndex));
                profile[j] = Color.Lerp(boundaryColors[i - 1], boundaryColors[i], lerp);
            }
        }

        for (int i = 0; i < profile.Length; i++)
        {
            r.AddKey(i * profileInterval, profile[i].r);
            g.AddKey(i * profileInterval, profile[i].g);
            b.AddKey(i * profileInterval, profile[i].b);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(time.deltaTime > 0)
            while (currentState * profileInterval < time.time)
                currentState++;
        else if (time.deltaTime < 0)
            while (currentState * profileInterval > time.time)
                currentState--;
        if (currentState > profile.Length - 1)
            currentState = profile.Length - 1;
        if (currentState < 0)
            currentState = 0;
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.color = profile[currentState];
    }

    public int getCurrentState()
    {
        return currentState;
    }

    public Color getCurrentColor()
    {
        return renderers[0].material.color;
    }
}
