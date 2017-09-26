using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using Chronos;

public class TemporalImagingEffect : MonoBehaviour {

    public VignetteAndChromaticAberration effectScript;
    public ColorCorrectionCurves colorScript;
    public float positiveVignett = 0f;
    public float negativeVignett = 0.25f;
    public float transitionDuration = 0.25f;
    public GlobalClock clock;
    private float previousIntensity = 0f;

	// Update is called once per frame
	void Update () {
        float intensity = Mathf.Lerp(negativeVignett, positiveVignett, (clock.localTimeScale + 1) / 2);

        if (clock.time < 1f && clock.timeScale == 0)
            colorScript.saturation = 0f;
        else if (clock.timeScale == 0)
            colorScript.saturation = 0f;
        else
            colorScript.saturation = 1f;

        if (previousIntensity != intensity) {
            Hashtable ht = new Hashtable();
            ht.Add("from", effectScript.intensity);
            ht.Add("to", intensity);
            ht.Add("time", transitionDuration);
            ht.Add("onupdate", "changeIntensity");
            iTween.ValueTo(this.gameObject, ht);
        }
        previousIntensity = intensity;
	}

    void changeIntensity(float intensity)
    {
        effectScript.intensity = intensity;
    }
}
