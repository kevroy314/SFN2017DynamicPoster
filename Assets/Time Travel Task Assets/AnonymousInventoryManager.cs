using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using Chronos;

public class AnonymousInventoryManager : MonoBehaviour {
    
    public enum AnonymousInventoryMode {Start=0, LocationTime=1, Rewind=2, Identity=3};
    public AnonymousInventoryMode currentMode = AnonymousInventoryMode.Start;
    public AnonymousInventoryMode startMode = AnonymousInventoryMode.Start;
    private bool modeFinished = false;
    private string savedEventButton;
    private KeyCode savedEventKey;
    public Material anonymousMaterial;
    public ItemGenerator generator;

    public Image displayImage;
    public Image typeDisplayImage;
    public Material emptyMaterial;
    public Timeline time;
    public GlobalClock globalClock;
    public TimeController timeController;
    public float pickDistance = 4f;
    public float placeDistance = 3f;
    public float minObjectPlaceDistance = 1f;

    public GameObject fallPrefabItem;
    public GameObject flyPrefabItem;
    public GameObject foilPrefabItem;
    public GameObject nullPrefabItem;

    public AudioClip soundEffect;
    public AudioClip multiSoundEffect;
    private AudioSource audioSrc;

    public Material upMaterial;
    public Material downMaterial;
    public Material infinityMaterial;
    public Material nullMaterial;

    private ClickableObject[] clickableObjects;
    private LinkedList<int> objectHeldList;

    public int currentItemTypeIndex = 0;

    public bool placeWithMouse = true;
    public float mousePlaceHeight = 5f;
    public float forceTransparency = 1f;

    public Camera targetingCamera;

    // Use this for initialization
    void Start () {
        currentMode = startMode;
        displayImage.material = anonymousMaterial;
        typeDisplayImage.material = nullMaterial;
        audioSrc = gameObject.AddComponent<AudioSource>();
        audioSrc.clip = soundEffect;
    }
	
    void InitializeObjectLists()
    {
        //Populate list of object children
        int numChildren = generator.gameObject.transform.childCount;
        List<ClickableObject> clickable = new List<ClickableObject>();
        objectHeldList = new LinkedList<int>();
        for (int i = 0; i < numChildren; i++)
        {
            ClickableObject tmp = generator.gameObject.transform.GetChild(i).GetComponentInChildren<ClickableObject>();
            if (tmp != null)
            {
                clickable.Add(tmp);
                tmp.gameObject.transform.parent.gameObject.SetActive(false);
            }
        }
        //Add objects to held-list
        clickableObjects = clickable.ToArray();
        for (int i = 0; i < clickableObjects.Length; i++)
            objectHeldList.AddFirst(i);
    }

    void OverrideItemMaterials(Material m)
    {
        foreach (ClickableObject obj in clickableObjects)
        {
            obj.changeTexture = false;
            obj.gameObject.GetComponent<MeshRenderer>().material = m;
        }
    }

    bool DetectPlacedButtonRisingEdge()
    {
        return InputManager.mainManager.GetButton(InputManager.ButtonType.Place, InputManager.ButtonState.RisingEdge);
    }

    int DetectSelectionHit()
    {
        Vector3 comparisonDistance = gameObject.transform.position;
        Vector3 mouse = InputManager.mainManager.mouseWorldPosition;
        comparisonDistance = new Vector3(mouse.x, mousePlaceHeight * 2, mouse.z);
        RaycastHit hit;
        var cameraCenter = targetingCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, targetingCamera.nearClipPlane));
        if (placeWithMouse)
            cameraCenter = InputManager.mainManager.mouseWorldPosition;
        Physics.SphereCast(cameraCenter, 0.5f, targetingCamera.transform.forward, out hit, 1000f, 1 << LayerMask.NameToLayer("UI"));
        int hitIndex = -1;
        if (hit.transform != null)
        {
            GameObject hitObj = hit.transform.gameObject;
            Debug.Log("Hit: " + hitObj.name);
            for (int i = 0; i < clickableObjects.Length; i++)
                if (hitObj == clickableObjects[i].gameObject)
                    hitIndex = i;
        }
        return hitIndex;
    }

    bool AttemptPerformPickup(int hitIndex)
    {
        if (hitIndex >= 0 && (!placeWithMouse || clickableObjects[hitIndex].gameObject.GetComponent<MeshRenderer>().isVisible)) //And it is visible, within the min distance, and clickable
        {
            clickableObjects[hitIndex].gameObject.transform.parent.gameObject.SetActive(false);
            objectHeldList.AddFirst(hitIndex);
            // Log item number, event type, location, time, 
            //itemLogger.logMessage("Item Picked Up : " + clickableObjects[closestIndex].gameObject.transform.parent.name + " type ");
            Debug.Log(hitIndex);
            audioSrc.pitch = 1f;
            audioSrc.clip = soundEffect;
            audioSrc.Play();
            return true;
        }
        return false;
    }

    public ClickableObject CopyObject<T>(ClickableObject source)
    {
        Texture2D prevTexture = source.mainTexture;
        Texture2D prevClickTexture = source.clickTexture;
        bool prevPlaySound = source.playSoundEffect;
        Transform prevParent = source.gameObject.transform.parent.transform.parent;
        int prevItemNum = int.Parse(source.gameObject.transform.parent.gameObject.name.Substring(4));
        float prevDelay = source.transitionDelay;
        GameObject obj;
        Vector3 placePosition = source.gameObject.transform.parent.transform.localPosition;

        if (typeof(T) == typeof(FallFromSky))
            obj = ItemGenerator.GenerateFall(fallPrefabItem, prevParent, placePosition, prevTexture, prevClickTexture, prevDelay, time, prevItemNum, prevPlaySound);
        else if (typeof(T) == typeof(FlyToSky))
            obj = ItemGenerator.GenerateFly(flyPrefabItem, prevParent, placePosition, prevTexture, prevClickTexture, prevDelay, time, prevItemNum, prevPlaySound);
        else if (typeof(T) == typeof(Foil))
            obj = ItemGenerator.GenerateFoil(foilPrefabItem, prevParent, placePosition, prevTexture, prevClickTexture, prevDelay, time, prevItemNum, prevPlaySound);
        else if (typeof(T) == typeof(NullEvent))
            obj = ItemGenerator.GenerateNull(nullPrefabItem, prevParent, placePosition, anonymousMaterial, prevTexture, prevClickTexture, prevDelay, time, prevItemNum, prevPlaySound);
        else
            return null;

        Debug.Log(obj.transform.localPosition.y);

        return obj.GetComponentInChildren<ClickableObject>();
    }

    bool AttemptPlaceItem()
    {
        Debug.Log("No Hit, Placing.");
        if (objectHeldList.Count > 0)
        {
            //Find the index of the item being placed
            int index = objectHeldList.First.Value;
            
            //Validate the placement
            bool validPlacement = true;
            if (time.time > 59.99 && currentItemTypeIndex != 0)
            {
                Debug.Log("Invalidated Placement Due To Event Type and Time");
                validPlacement = false;
            }
            if (validPlacement)
            {
                //Log debug information
                int prevItemNum = int.Parse(clickableObjects[index].gameObject.transform.parent.gameObject.name.Substring(4));
                Debug.Log("Valid Placement");
                Debug.Log(prevItemNum);

                //Determine the place position
                Vector3 placePosition = Vector3.zero;
                if (placeWithMouse)
                {
                    Vector3 mouse = InputManager.mainManager.mouseWorldPosition;
                    placePosition = new Vector3(mouse.x, mousePlaceHeight, mouse.z);
                }

                // Check boundary conditions and adjust the place position accordingly
                Vector3 originalPlacement = placePosition;
                if (placePosition.x < -18.5f)
                    placePosition = new Vector3(-18.5f, placePosition.y, placePosition.z);
                else if (placePosition.x > 18.5f)
                    placePosition = new Vector3(18.5f, placePosition.y, placePosition.z);
                if (placePosition.z < -18.5f)
                    placePosition = new Vector3(placePosition.x, placePosition.y, -18.5f);
                else if (placePosition.z > 18.5f)
                    placePosition = new Vector3(placePosition.x, placePosition.y, 18.5f);
                if (originalPlacement != placePosition)
                    Debug.Log("Placed within wall, placement has been shifted.");

                //Copy the object as a null event
                ClickableObject obj = CopyObject<NullEvent>(clickableObjects[index]);
                obj.gameObject.transform.parent.transform.localPosition = placePosition;
                obj.transitionDelay = time.time;

                //Get the old object
                GameObject oldObj = clickableObjects[index].gameObject.transform.parent.gameObject;

                //Replace the old object in the list with the new one
                clickableObjects[index] = obj.GetComponentInChildren<ClickableObject>();

                //Destroy the old object
                DestroyImmediate(oldObj);

                //Play the placement sound
                audioSrc.pitch = 1f;
                audioSrc.clip = soundEffect;
                audioSrc.Play();

                //Remove the item from the inventory
                objectHeldList.RemoveFirst();

                return true;
            }
            else
            {
                Debug.Log("No valid placement.");
            }
        }
        return false;
    }

    bool AttemptAssignItem(int hitIndex)
    {
        if (hitIndex >= 0 && (!placeWithMouse || clickableObjects[hitIndex].gameObject.GetComponent<MeshRenderer>().isVisible)) //And it is visible, within the min distance, and clickable
        {
            if (clickableObjects[hitIndex].gameObject.GetComponent<NullEvent>() != null)
            { // The event is unassigned, assign it
                Debug.Log("assign");

                if (objectHeldList.Count <= 0)
                    return false;

                int index = objectHeldList.First.Value;

                //Replace the clicked item with the correct identity and type (maintaining position and delay)
                ClickableObject obj;
                if (currentItemTypeIndex == 2)
                {
                    obj = CopyObject<FallFromSky>(clickableObjects[index]);
                    obj.GetComponent<FallFromSky>().enableBumpStart = true;
                }
                else if (currentItemTypeIndex == 1)
                {
                    obj = CopyObject<FlyToSky>(clickableObjects[index]);
                    obj.GetComponent<FlyToSky>().enableBumpStart = true;
                }
                else
                {
                    obj = CopyObject<Foil>(clickableObjects[index]);
                }
                obj.transform.parent.transform.localPosition = clickableObjects[hitIndex].gameObject.transform.parent.transform.localPosition;
                obj.transitionDelay = clickableObjects[hitIndex].transitionDelay;

                if (index != hitIndex)
                { //Only neccessary to make a new object if we're swapping with an old one
                    //Replace the original item with a new null event (maintaining position and delay)
                    ClickableObject obj2 = CopyObject<NullEvent>(clickableObjects[hitIndex]);
                    obj2.transform.parent.transform.localPosition = clickableObjects[index].gameObject.transform.parent.transform.localPosition;
                    obj2.transitionDelay = clickableObjects[index].transitionDelay;

                    //Get the old replacement object
                    GameObject oldObj2 = clickableObjects[hitIndex].gameObject.transform.parent.gameObject;

                    //Replace the old object in the list with the new one
                    clickableObjects[hitIndex] = obj2;

                    //Delete the old object
                    Debug.Log("Destroy" + oldObj2.name);
                    DestroyImmediate(oldObj2);
                }

                //Get the old object
                GameObject oldObj = clickableObjects[index].gameObject.transform.parent.gameObject;
                
                //Replace the old object in the list with the new one
                clickableObjects[index] = obj;

                //Delete the old object and remove the inventory index
                Debug.Log("Destroy" + oldObj.name);
                DestroyImmediate(oldObj);
                objectHeldList.RemoveFirst();
            }
            else
            { // The event was already assigned previous, clear its assignment
                Debug.Log("unassign");

                //Generate a new object (replacing the old with a null event)
                ClickableObject obj = CopyObject<NullEvent>(clickableObjects[hitIndex]);

                //Get the old object
                GameObject oldObj = clickableObjects[hitIndex].gameObject.transform.parent.gameObject;

                //Replace the old object in the list with the new one
                clickableObjects[hitIndex] = obj;

                //Delete the old object and add the inventory index
                DestroyImmediate(oldObj);
                objectHeldList.AddFirst(hitIndex);
            }
            
            Debug.Log(hitIndex);
            audioSrc.pitch = 1f;
            audioSrc.clip = soundEffect;
            audioSrc.Play();
            return true;
        }
        return false;
    }

    bool InventoryIsEmpty()
    {
        return objectHeldList.Count <= 0;
    }

    bool DetectNextStateButtonRisingEdge()
    {
        return InputManager.mainManager.GetButton(InputManager.ButtonType.NextState, InputManager.ButtonState.RisingEdge);
    }

    void SetImageMaterial(Image i, Material m)
    {
        if (i.material != m)
            i.material = m;
    }

    void SetImageMaterial(Image i, Texture2D t)
    {
        if (i.material.mainTexture != t)
        {
            Material m = new Material(i.material);
            m.mainTexture = t;
            i.material = m;
        }
    }

    void SetImageTexture(Image i, Texture2D t)
    {
        if (i.material.mainTexture != t)
            i.material.mainTexture = t;
    }

    void UpdateEventType(bool allowChange=true)
    {
        bool nextItemTypeState = InputManager.mainManager.GetButton(InputManager.ButtonType.NextEvent, InputManager.ButtonState.RisingEdge);
        if (nextItemTypeState && allowChange)
        {
            currentItemTypeIndex = (currentItemTypeIndex + 1) % 3;
        }
        switch (currentItemTypeIndex)
        {
            case 0:
                SetImageMaterial(typeDisplayImage, infinityMaterial);
                break;
            case 1:
                SetImageMaterial(typeDisplayImage, upMaterial);
                break;
            case 2:
                SetImageMaterial(typeDisplayImage, downMaterial);
                break;
            default:
                typeDisplayImage.material = null;
                break;
        }
    }

    void UpdateInventoryState(bool allowChange=true)
    {
        bool nextInputState = InputManager.mainManager.GetButton(InputManager.ButtonType.NextItem, InputManager.ButtonState.RisingEdge);
        if (nextInputState && objectHeldList.Count != 0 && allowChange)
        {
            objectHeldList.AddLast(objectHeldList.First.Value);
            objectHeldList.RemoveFirst();
        }

        if (objectHeldList.Count != 0)
            SetImageMaterial(displayImage, clickableObjects[objectHeldList.First.Value].gameObject.GetComponent<ClickableObject>().mainTexture);
        else
            SetImageMaterial(displayImage, emptyMaterial);
    }

    void ReinitializeInventoryForIdentifyPhase()
    {
        timeController.controlEnabled = false;
        globalClock.localTimeScale = -10;
        displayImage.material = anonymousMaterial;
        typeDisplayImage.material = nullMaterial;

        // Generate random list of indicies
        List<int> tmp = new List<int>();
        for (int i = 0; i < clickableObjects.Length; i++)
            tmp.Add(i);
        InventoryManager.Shuffle<int>(tmp);
        //Reinitialize object held list
        objectHeldList = new LinkedList<int>();
        for (int i = 0; i < tmp.Count; i++)
            objectHeldList.AddFirst(tmp[i]);

        //Update the UI
        UpdateInventoryState(false);
        UpdateEventType(false);
    }

    // Update is called once per frame
    /// <summary>
    /// 
    /// </summary>
    void Update () {
        if (currentMode == AnonymousInventoryMode.Start)
        {
            InitializeObjectLists();
            OverrideItemMaterials(anonymousMaterial);


            //Automatically move on to location/time
            currentMode = AnonymousInventoryMode.LocationTime;
        }
        else if (currentMode == AnonymousInventoryMode.LocationTime)
        {
            if (objectHeldList.Count == 0)
                SetImageMaterial(displayImage, emptyMaterial);
            else
                SetImageMaterial(displayImage, anonymousMaterial);
            if (DetectPlacedButtonRisingEdge())
                if(!AttemptPerformPickup(DetectSelectionHit())) //No object is being picked up, drop the current item
                    AttemptPlaceItem();
            if (InventoryIsEmpty() && DetectNextStateButtonRisingEdge())
            {
                currentMode = AnonymousInventoryMode.Rewind;
                ReinitializeInventoryForIdentifyPhase();
            }
        }
        else if (currentMode == AnonymousInventoryMode.Rewind)
        {
            if (time.time <= 0) {
                timeController.controlEnabled = true;
                currentMode = AnonymousInventoryMode.Identity;
            }
        }
        else if (currentMode == AnonymousInventoryMode.Identity)
        {
            UpdateEventType();
            UpdateInventoryState();
            if (DetectPlacedButtonRisingEdge())
                AttemptAssignItem(DetectSelectionHit());
        }
    }
}
