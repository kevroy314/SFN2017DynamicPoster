using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ContextBoundaryVisualizer : MonoBehaviour {

    public RectTransform leftItemTransform;
    public RectTransform middleItemTransform;
    public RectTransform rightItemTransform;

    public GameObject leftLabelGameObject;
    public GameObject rightLabelGameObject;
    public GameObject contextBoundaryLabelGameObject;

    private Text leftLabelText;
    private Text rightLabelText;
    private Text contextBoundaryLabelText;

    private RectTransform leftLabelTransform;
    private RectTransform rightLabelTransform;
    private RectTransform contextBoundaryLabelTransform;

    private float leftScale;
    private float rightScale;
    private float leftOffset;
    private float rightOffset;

    // Use this for initialization
    void Start () {
        leftLabelText = leftLabelGameObject.GetComponent<Text>();
        rightLabelText = rightLabelGameObject.GetComponent<Text>();
        contextBoundaryLabelText = contextBoundaryLabelGameObject.GetComponent<Text>();

        leftLabelTransform = leftLabelGameObject.GetComponent<RectTransform>();
        rightLabelTransform = rightLabelGameObject.GetComponent<RectTransform>();
        contextBoundaryLabelTransform = contextBoundaryLabelGameObject.GetComponent<RectTransform>();

        leftScale = 1f / (middleItemTransform.anchoredPosition3D.x - leftItemTransform.anchoredPosition3D.x);
        rightScale = 1f / (rightItemTransform.anchoredPosition3D.x - middleItemTransform.anchoredPosition3D.x);

        leftOffset = leftItemTransform.anchoredPosition3D.x;
        rightOffset = middleItemTransform.anchoredPosition3D.x;

        // TODO: Add looped iTween animation of squish and expand
    }
	
	// Update is called once per frame
	void Update () {
        leftLabelTransform.anchoredPosition3D = new Vector3((leftItemTransform.anchoredPosition3D.x + middleItemTransform.anchoredPosition3D.x) / 2f, leftLabelTransform.anchoredPosition3D.y, leftLabelTransform.anchoredPosition3D.z);
        rightLabelTransform.anchoredPosition3D = new Vector3((rightItemTransform.anchoredPosition3D.x + middleItemTransform.anchoredPosition3D.x) / 2f, rightLabelTransform.anchoredPosition3D.y, rightLabelTransform.anchoredPosition3D.z);
        contextBoundaryLabelTransform.anchoredPosition3D = new Vector3((leftLabelTransform.anchoredPosition3D.x + rightLabelTransform.anchoredPosition3D.x) / 2f, contextBoundaryLabelTransform.anchoredPosition3D.y, contextBoundaryLabelTransform.anchoredPosition3D.z);

        // TODO: Fix this
        float leftNormDist = (middleItemTransform.anchoredPosition3D.x - leftItemTransform.anchoredPosition3D.x - leftOffset) * leftScale;
        float rightNormDist = (rightItemTransform.anchoredPosition3D.x - middleItemTransform.anchoredPosition3D.x - rightOffset) * rightScale;

        leftLabelText.text = leftNormDist.ToString("0.000");
        rightLabelText.text = rightNormDist.ToString("0.000");

        contextBoundaryLabelText.text = (1 - rightNormDist - leftNormDist).ToString("0.000");
    }
}
