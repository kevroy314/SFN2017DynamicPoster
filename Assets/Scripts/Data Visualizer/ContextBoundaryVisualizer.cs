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

    public Text effectTextLabel;

    private RectTransform leftLabelTransform;
    private RectTransform rightLabelTransform;
    private RectTransform contextBoundaryLabelTransform;

    private float leftScale;
    private float rightScale;

    private Hashtable leftItemAnimationHash;
    private Hashtable middleItemAnimationHash;
    private Hashtable rightItemAnimationHash;

    public float animationTime;
    public iTween.LoopType loopType;
    public iTween.EaseType easeType;

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

        leftItemAnimationHash = GetMovementHash("SetLeftLocation", -440f, -300f);
        middleItemAnimationHash = GetMovementHash("SetMiddleLocation", -30f, -170f);
        rightItemAnimationHash = GetMovementHash("SetRightLocation", 100f, 240f);

        iTween.ValueTo(gameObject, leftItemAnimationHash);
        iTween.ValueTo(gameObject, middleItemAnimationHash);
        iTween.ValueTo(gameObject, rightItemAnimationHash);
    }
	
    Hashtable GetMovementHash(string callbackFunctionName, float from, float to)
    {
        Hashtable tmp = new Hashtable();
        tmp.Add("from", from);
        tmp.Add("to", to);
        tmp.Add("time", animationTime);
        tmp.Add("looptype", loopType);
        tmp.Add("easetype", easeType);
        tmp.Add("onupdate", callbackFunctionName);
        return tmp;
    }

    void SetLocation(RectTransform trans, float l)
    {
        trans.anchoredPosition3D = new Vector3(l, trans.anchoredPosition3D.y, trans.anchoredPosition3D.z);
    }

    void SetLeftLocation(float l)
    {
        SetLocation(leftItemTransform, l);
    }

    void SetMiddleLocation(float l)
    {
        SetLocation(middleItemTransform, l);
    }

    void SetRightLocation(float l)
    {
        SetLocation(rightItemTransform, l);
    }

    // Update is called once per frame
    void Update () {
        leftLabelTransform.anchoredPosition3D = new Vector3((leftItemTransform.anchoredPosition3D.x + middleItemTransform.anchoredPosition3D.x) / 2f, leftLabelTransform.anchoredPosition3D.y, leftLabelTransform.anchoredPosition3D.z);
        rightLabelTransform.anchoredPosition3D = new Vector3((rightItemTransform.anchoredPosition3D.x + middleItemTransform.anchoredPosition3D.x) / 2f, rightLabelTransform.anchoredPosition3D.y, rightLabelTransform.anchoredPosition3D.z);
        contextBoundaryLabelTransform.anchoredPosition3D = new Vector3((leftLabelTransform.anchoredPosition3D.x + rightLabelTransform.anchoredPosition3D.x) / 2f, contextBoundaryLabelTransform.anchoredPosition3D.y, contextBoundaryLabelTransform.anchoredPosition3D.z);

        float leftNormDist = (middleItemTransform.anchoredPosition3D.x - leftItemTransform.anchoredPosition3D.x) * leftScale - 1;
        float rightNormDist = (rightItemTransform.anchoredPosition3D.x - middleItemTransform.anchoredPosition3D.x) * rightScale - 1;

        leftLabelText.text = leftNormDist.ToString("0.000");
        rightLabelText.text = rightNormDist.ToString("0.000");

        float cbe = (rightNormDist - leftNormDist);

        contextBoundaryLabelText.text = cbe.ToString("0.000");

        string effect = "No CB Effect";

        if (cbe < 0) effect = "Negative CB Effect";
        if (cbe > 0) effect = "Positive (Standard) CB Effect";

        effectTextLabel.text = effect;
    }
}
