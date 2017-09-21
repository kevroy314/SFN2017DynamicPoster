using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemLocationRenderer : MonoBehaviour {

    public Vector3[] locations;
    public Color[] colors;
    public EventType[] types;

    public Vector3 locationScale = new Vector3(1f, 1f, 1f);
    public Vector3 locationOffset = new Vector3(0f, 0f, 0f);

    public float dotSize = 0.01f;
    public float lineSize = 0.005f;

    public Material itemMaterial;

    public bool overrideTransparency = true;
    public float overrideTransparencyAlpha = 0.5f;

    public enum EventType { Up, Down, Stationary };

	// Use this for initialization
	void Start () {
        //Pad Colors if needed
        List<Color> colorsPadded = new List<Color>(colors);
        for (int i = 0; i < locations.Length - colors.Length; i++)
            colorsPadded.Add(Color.white);
        colors = colorsPadded.ToArray();

        //Pad Types if needed
        List<EventType> typesPadded = new List<EventType>(types);
        for (int i = 0; i < locations.Length - types.Length; i++)
            typesPadded.Add(EventType.Stationary);
        types = typesPadded.ToArray();

	    for(int i = 0; i < locations.Length; i++)
        {
            locations[i] = Vector3.Scale(locations[i], locationScale);
            locations[i] += locationOffset;

            if (overrideTransparency)
                colors[i].a = overrideTransparencyAlpha;

            GameObject dotObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            GameObject lineObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            dotObj.transform.localScale = new Vector3(dotSize, dotSize, dotSize);
            dotObj.GetComponent<Renderer>().material = itemMaterial;
            lineObj.GetComponent<Renderer>().material = itemMaterial;
            dotObj.GetComponent<Renderer>().material.color = colors[i];
            lineObj.GetComponent<Renderer>().material.color = colors[i];
            dotObj.transform.position = new Vector3(locations[i].x, locations[i].z, locations[i].y);
            float lineLength = 2f;
            float lineOffset = 0f;
            if (types[i] == EventType.Down)
            {
                lineLength = 2f - locations[i].z;
                lineOffset = locations[i].z + lineLength / 2f;
                lineLength /= 2f;
            }
            else if (types[i] == EventType.Up)
            {
                lineLength = locations[i].z - locationOffset.z;
                lineOffset = locations[i].z - lineLength / 2f;
                lineLength /= 2f;
            }
            if (types[i] == EventType.Stationary)
            {
                lineLength = 2f;
                lineOffset = 0f;
                dotObj.SetActive(false);
            }
            // Debug.Log("" + i + " : lineLength=" + lineLength + ", lineOffset=" + lineOffset);
            lineObj.transform.position = new Vector3(locations[i].x, lineOffset, locations[i].y);
            lineObj.transform.localScale = new Vector3(lineSize, lineLength, lineSize);

            lineObj.transform.parent = transform;
            dotObj.transform.parent = transform;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
