using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class HeatmapSelector : MonoBehaviour {
    public Texture2D[] heatmapVolumesBySlices;
    public int[] segments;
    public KeyCode[] keys;

    public bool initialize = true;

    public RayMarching rm;

    private List<Texture2D> heatmaps;

    private int numberOfCompleteHeatmaps;
    private int activeTargetSegment;

    // Use this for initialization
    void Start() {
        heatmaps = new List<Texture2D>(heatmapVolumesBySlices);

        numberOfCompleteHeatmaps = (int)Mathf.Floor((float)heatmapVolumesBySlices.Length / (float)rm.volumeDepth);

        activeTargetSegment = -1;

        int[] lengths = new int[] { segments.Length, keys.Length };
        int minLength = int.MaxValue;
        for (int i = 0; i < lengths.Length; i++)
            if (lengths[i] < minLength)
                minLength = lengths[i];

        Array.Resize<KeyCode>(ref keys, minLength);
        Array.Resize<int>(ref segments, minLength);

        if (initialize && numberOfCompleteHeatmaps >= 1)
            setTargetSegment(0);

    }
	
    private void setTargetSegment(int targetIdx)
    {
        if (targetIdx < numberOfCompleteHeatmaps && targetIdx >= 0 && activeTargetSegment != targetIdx)
        {
            rm.slices = heatmaps.GetRange(64 * targetIdx, 64).ToArray();
            rm.OnDestroy();
            rm.Start();
            activeTargetSegment = targetIdx;
        }
    }

	// Update is called once per frame
	void Update () {
	    for(int i = 0; i < keys.Length; i++)
            if (Input.GetKeyUp(keys[i]))
                setTargetSegment(segments[i]);
	}
}
