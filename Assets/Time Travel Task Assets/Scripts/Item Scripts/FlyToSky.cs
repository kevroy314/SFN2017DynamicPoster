using UnityEngine;
using System.Collections;
using Chronos;

public class FlyToSky : ClickableObject
{

    public Vector3 startPos = new Vector3(0f, 0f, 0f);
    public Vector3 endPos = new Vector3(0f, 10f, 0f);

    private MeshRenderer render;
    public Timeline time;
    private AudioSource audioSrc;
    private bool playedForward;
    private bool playedBackward;

    public bool enableBumpStart = false;

    private Hashtable goToStartHash;
    private Hashtable goToEndHash;

    // Use this for initialization
    public void Start()
    {
        StartI();

        render = GetComponent<MeshRenderer>();
        //time = GetComponent<Timeline>();

        audioSrc = transform.parent.gameObject.GetComponent<AudioSource>();

        goToStartHash = new Hashtable();
        goToStartHash.Add("position", startPos);
        goToStartHash.Add("islocal", true);
        goToStartHash.Add("time", transitionDuration);
        goToStartHash.Add("easetype", iTween.EaseType.easeOutBounce);
        goToEndHash = new Hashtable();
        goToEndHash.Add("position", endPos);
        goToEndHash.Add("islocal", true);
        goToEndHash.Add("time", transitionDuration);
        goToEndHash.Add("easetype", iTween.EaseType.easeInBounce);

        clickStartTime = transitionDelay;
        clickEndTime = transitionDelay + transitionDuration;
        localTime = time;

        transform.localPosition = startPos;
        render.enabled = true;
        playedBackward = true;
        playedForward = false;

        if (enableBumpStart)
            BumpState(); //If item is created in the middle, we may need to sync it to the current timeline state
    }

    public void Update()
    {
        UpdateI();

        if (time.deltaTime > 0 && time.time >= transitionDelay && !playedForward)
        {
            iTween.MoveTo(gameObject, goToEndHash);
            audioSrc.timeSamples = 0;
            audioSrc.pitch = 1;
            if (playSoundEffect && Mathf.Abs(time.time - transitionDelay) < soundEffectTimeDistanceThreshold)
                audioSrc.Play();
            playedForward = true;
            playedBackward = false;
        }
        if (time.deltaTime > 0 && time.time > transitionDelay + transitionDuration && render.enabled)
        {
            render.enabled = false;
        }
        if (time.deltaTime < 0 && time.time < transitionDelay + transitionDuration && !render.enabled)
        {
            render.enabled = true;
        }
        if (time.deltaTime < 0 && time.time < transitionDelay + transitionDuration && !playedBackward)
        {
            iTween.MoveTo(gameObject, goToStartHash);
            audioSrc.Stop();
            audioSrc.timeSamples = audioSrc.clip.samples - 1;
            audioSrc.pitch = -1;
            if (playSoundEffect && Mathf.Abs(time.time - transitionDelay) < soundEffectTimeDistanceThreshold)
                audioSrc.Play();
            playedBackward = true;
            playedForward = false;
        }
    }

    public void BumpState()
    {
        if (time.time >= transitionDelay)
        {
            gameObject.transform.localPosition = endPos;
            playedForward = true;
            playedBackward = false;
            render.enabled = false;
        }
        if (time.time < transitionDelay)
        {
            gameObject.transform.localPosition = startPos;
            playedBackward = true;
            playedForward = false;
            render.enabled = true;
        }
    }
}
