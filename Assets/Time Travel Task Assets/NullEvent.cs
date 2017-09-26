using UnityEngine;
using System.Collections;
using Chronos;

public class NullEvent : ClickableObject
{

    public Timeline time;
    private AudioSource audioSrc;
    private bool playedForward = false;
    private bool playedBackward = true;


    // Use this for initialization
    public void Start()
    {
        StartI();

        audioSrc = transform.parent.gameObject.GetComponent<AudioSource>();

        clickStartTime = transitionDelay;
        clickEndTime = transitionDelay + transitionDuration;
        localTime = time;
    }

    public void Update()
    {
        UpdateI();

        if (time.deltaTime > 0 && time.time >= transitionDelay && !playedForward)
        {
            audioSrc.timeSamples = 0;
            audioSrc.pitch = 1;
            if (playSoundEffect && Mathf.Abs(time.time - transitionDelay) < soundEffectTimeDistanceThreshold)
                audioSrc.Play();
            playedForward = true;
            playedBackward = false;
        }
        else if (time.deltaTime < 0 && time.time < transitionDelay + transitionDuration && !playedBackward)
        {
            audioSrc.Stop();
            audioSrc.timeSamples = audioSrc.clip.samples - 1;
            audioSrc.pitch = -1;
            if (playSoundEffect && Mathf.Abs(time.time - transitionDelay) < soundEffectTimeDistanceThreshold)
                audioSrc.Play();
            playedBackward = true;
            playedForward = false;
        }
        else if (time.deltaTime < 0 && time.time < transitionDelay)
        {
        }
    }
}
