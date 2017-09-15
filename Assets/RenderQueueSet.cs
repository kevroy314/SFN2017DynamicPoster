using UnityEngine;
using System.Collections;

public class RenderQueueSet : MonoBehaviour {

    void Start()
    {
        // get all renderers in this object and its children:
        Renderer[] renders = GetComponentsInChildren<Renderer>();
        foreach (Renderer rendr in renders)
            rendr.material.renderQueue = 2002; // set their renderQueue
    }
}
