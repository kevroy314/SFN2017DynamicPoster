using UnityEngine;
using System.Collections;
using Chronos;

public class Foil : ClickableObject
{

    private MeshRenderer render;
    private Timeline time;
    private AudioSource audioSrc;

    // Use this for initialization
    void Start () {
        StartI();

        render = GetComponent<MeshRenderer>();
        time = GetComponent<Timeline>();

        audioSrc = transform.parent.gameObject.GetComponent<AudioSource>();

        transform.localPosition = Vector3.zero;

        render.enabled = true;

        clickStartTime = 0f;
        clickEndTime = float.MaxValue;
    }

    public Timeline Time
    {
        get { return time; }
        set { time = value; localTime = value; }
    }

    void Update()
    {
        UpdateI();
    }
}
