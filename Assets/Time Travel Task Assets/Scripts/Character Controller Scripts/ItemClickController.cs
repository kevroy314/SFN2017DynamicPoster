using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ItemClickController : MonoBehaviour {

    public KeyCode keyClickButton = KeyCode.Space;
    public string controllerClickButton = "a";
    public float minClickDistance = 3f;
    public ItemGenerator generator;
    private ClickableObject[] clickableObjects;
    public int closestIndex = -1;
    public float closestDist = float.MaxValue;
    private bool prevClickAction = false;
    private bool firstCall = true;
    public Text itemsRemainingText;
    public bool finished = false;

    public AudioClip finishedSoundEffect;
	
	// Update is called once per frame
	void Update () {
        if (firstCall)
        {
            //Needs to be run on first update because Start() order isn't promised
            int numChildren = generator.gameObject.transform.childCount;
            List<ClickableObject> clickable = new List<ClickableObject>();
            for (int i = 0; i < numChildren; i++)
            {
                ClickableObject tmp = generator.gameObject.transform.GetChild(i).GetComponentInChildren<ClickableObject>();
                if (tmp != null)
                    clickable.Add(tmp);
            }
            clickableObjects = clickable.ToArray();
            firstCall = false;
        }
        //Find closest object
        closestIndex = -1;
        closestDist = float.MaxValue;
	    for(int i = 0; i < clickableObjects.Length; i++)
        {
            float dist = Vector3.Distance(gameObject.transform.position, clickableObjects[i].gameObject.transform.position);
            if (dist < closestDist)
            {
                closestIndex = i;
                closestDist = dist;
            }
        }

        bool clickAction = Input.GetKey(keyClickButton) || Input.GetButton(controllerClickButton);
        if(clickAction && !prevClickAction) //On click rising edge
            if(closestIndex != -1) //If an object was found
                if (clickableObjects[closestIndex].targetObject.gameObject.GetComponent<MeshRenderer>().isVisible && closestDist <= minClickDistance && clickableObjects[closestIndex].clickable) //And it is visible, within the min distance, and clickable
                    clickableObjects[closestIndex].Click(); //Click it
        prevClickAction = clickAction;

        int itemsClicked = GetNumItemsNotClicked();
        itemsRemainingText.text = itemsClicked + " Items Remaining";

        if(itemsClicked == 0 && !finished)
        {
            Debug.Log("Done!");
            finished = true;
            AudioSource src = gameObject.AddComponent<AudioSource>();
            src.volume = 1f;
            src.PlayOneShot(finishedSoundEffect, 1f);
        }
	}

    private int GetNumItemsNotClicked()
    {
        int count = 0;
        for(int i = 0; i < clickableObjects.Length; i++)
            if (!clickableObjects[i].HasBeenClicked())
                count++;
        return count;
    }
}
