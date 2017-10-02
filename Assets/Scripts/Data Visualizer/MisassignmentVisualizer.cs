using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MisassignmentVisualizer : MonoBehaviour {

    public float min = -530f;
    public float max = 530f;

    public float normalizedLocation = 0f;

    public Text displayText;

    private RectTransform trans;

    private Hashtable movementLoopHash;

    public Image[] itemLocationRenderers;
    public Vector2[] itemLocationRanges;
    public Color[] defaultColors;
    public Color[] onCoverColors;
    public string[] onCoverTexts;
    public string defaultText;

    public iTween.LoopType loopType = iTween.LoopType.loop;
    public iTween.EaseType easeType = iTween.EaseType.linear;
    public float animationTime = 10f;

	// Use this for initialization
	void Start () {
        trans = GetComponent<RectTransform>();

        SetPosition(normalizedLocation);

        movementLoopHash = new Hashtable();
        movementLoopHash.Add("from", 0f);
        movementLoopHash.Add("to", 1f);
        movementLoopHash.Add("time", animationTime);
        movementLoopHash.Add("looptype", loopType);
        movementLoopHash.Add("easetype", easeType);
        movementLoopHash.Add("onupdate", "SetNormalizedLocation");

        //pingPong 
        iTween.ValueTo(this.gameObject, movementLoopHash);
    }
	
	// Update is called once per frame
	void Update () {
        SetPosition(normalizedLocation);

        // Set colors and text
        bool atLeastOneCovered = false;
        for(int i = 0; i < itemLocationRanges.Length; i++)
        {
            if (normalizedLocation > itemLocationRanges[i].x && normalizedLocation < itemLocationRanges[i].y)
            {
                atLeastOneCovered = true;
                itemLocationRenderers[i].color = onCoverColors[i];
                displayText.text = onCoverTexts[i];
            }
            else
            {
                itemLocationRenderers[i].color = defaultColors[i];
            }
        }
        if (!atLeastOneCovered) displayText.text = defaultText;
    }

    void SetNormalizedLocation(float l)
    {
        normalizedLocation = l;
    }

    void SetPosition(float normalizedLocation)
    {
        trans.anchoredPosition3D = new Vector3(Mathf.Lerp(0, max - min, normalizedLocation) + min, trans.anchoredPosition3D.y, trans.anchoredPosition3D.z);
    }
}
