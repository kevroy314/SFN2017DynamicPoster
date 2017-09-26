using UnityEngine;
using System.Collections;

public class Camera2DAdjuster : MonoBehaviour
{
    public float cameraHeight = 100f;
    public BoundaryManager manager;
    public bool changeLightingToMatchBoundary = false;
    public bool overrideVignetteSettings = true;
    public float vignetteOverrideValue = 0.25f;
    private Camera cam;
    // Use this for initialization
    void Start()
    {
        cam = GetComponent<Camera>();
        if(overrideVignetteSettings)
            GetComponent<TemporalImagingEffect>().negativeVignett = vignetteOverrideValue;
    }

    // Update is called once per frame
    void Update()
    {
        cam.transform.position = new Vector3(cam.transform.position.x, cameraHeight, cam.transform.position.z);
        if (changeLightingToMatchBoundary)
        {
            RenderSettings.ambientGroundColor = manager.renderers[0].material.color;
            RenderSettings.ambientSkyColor = manager.renderers[0].material.color;
            RenderSettings.ambientEquatorColor = manager.renderers[0].material.color;
        }
    }
}
