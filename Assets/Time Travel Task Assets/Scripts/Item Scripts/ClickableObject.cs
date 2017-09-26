using UnityEngine;
using System.Collections;
using Chronos;

public class ClickableObject : MonoBehaviour
{
    public float soundEffectTimeDistanceThreshold = 2f;
    public float transitionDelay = 10f;
    public float transitionDuration = 1f;
    public float clickStartTime = 0f;
    public float clickEndTime = 1f;
    public bool clickable = false;
    public GameObject targetObject;
    private MeshRenderer targetRender;
    private Component halo;
    private bool previousClickable = false;
    private Hashtable pulseHash;
    public Timeline localTime;
    private bool hasBeenClicked = false;
    private AudioSource soundEffect;
    public Texture2D clickTexture;
    public Texture2D mainTexture;
    public bool playSoundEffect = true;
    public bool changeTexture = true;

    // Use this for initialization
    public void StartI()
    {
        targetObject.transform.localPosition = new Vector3(0f, -0.9f, 0f);
        targetRender = targetObject.GetComponent<MeshRenderer>();
        halo = targetObject.GetComponent("Halo");
        pulseHash = new Hashtable();
        pulseHash.Add("amount", new Vector3(0.02f, 0.02f, 0.02f));
        pulseHash.Add("time", 1f);
        pulseHash.Add("eastype", iTween.EaseType.easeInSine);
        pulseHash.Add("looptype", iTween.LoopType.loop);
        soundEffect = targetObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    public void UpdateI()
    {

        if (localTime.time > clickStartTime && localTime.time < clickEndTime)
        {
            clickable = true;
            if (changeTexture)
            {
                gameObject.GetComponent<MeshRenderer>().material.mainTexture = clickTexture;
                ItemGenerator.flipTexture(gameObject);
            }
        }
        else
        {
            clickable = false;
            if (changeTexture)
            {
                gameObject.GetComponent<MeshRenderer>().material.mainTexture = mainTexture;
                ItemGenerator.flipTexture(gameObject);
            }
        }

        if (hasBeenClicked)
            return;

        if (clickable && !previousClickable)
        {
            iTween.PunchScale(targetObject, pulseHash);
            targetRender.enabled = true;
            halo.GetType().GetProperty("enabled").SetValue(halo, true, null);
        }
        else if (!clickable)
        {
            targetRender.enabled = false;
            halo.GetType().GetProperty("enabled").SetValue(halo, false, null);
            iTween.Stop(targetObject);
        }

        previousClickable = clickable;
    }

    public void Click()
    {
        if (!hasBeenClicked)
        {
            targetRender.enabled = false;
            halo.GetType().GetProperty("enabled").SetValue(halo, false, null);
            iTween.Stop(targetObject);
            soundEffect.PlayOneShot(soundEffect.clip);
            hasBeenClicked = true;
        }
    }

    public bool HasBeenClicked()
    {
        return hasBeenClicked;
    }
}
