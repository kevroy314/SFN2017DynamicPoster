using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Chronos;

public class InventoryManager : MonoBehaviour
{
    public GameObject fallPrefabItem;
    public GameObject flyPrefabItem;
    public GameObject foilPrefabItem;

    public AudioClip soundEffect;
    public AudioClip multiSoundEffect;
    private AudioSource audioSrc;

    public Image displayImage;
    public Material emptyMaterial;
    public Timeline time;
    public float pickDistance = 4f;
    public float placeDistance = 3f;
    public float minObjectPlaceDistance = 1f;

    public Material upMaterial;
    public Material downMaterial;
    public Material infinityMaterial;

    public Image typeDisplayImage;

    public ItemGenerator generator;
    private ClickableObject[] clickableObjects;
    private LinkedList<int> objectHeldList;

    public int currentItemTypeIndex = 0;

    private bool firstCall = true;

    public bool placeWithMouse = false;
    public float mousePlaceHeight = 5f;
    public float forceTransparency = 1f;

    public Camera targetingCamera;

    // Use this for initialization
    void Start()
    {
        displayImage.material = emptyMaterial;
        audioSrc = gameObject.AddComponent<AudioSource>();
        audioSrc.clip = soundEffect;
    }

    private static System.Random rng = new System.Random();

    public ClickableObject[] Objects
    {
        get { return clickableObjects; }
    }

    public static List<T> Shuffle<T>(IList<T> list)
    {
        List<T> newList = new List<T>(list);
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return newList;
    }

    public int[] InventoryState
    {
        get
        {
            int[] state = new int[clickableObjects.Length]; //Make state array
            for (int i = 0; i < state.Length; i++) //Fill will -1 by default
                state[i] = -1;
            objectHeldList.CopyTo(state, 0); //Copy inventory index list
            for (int i = 0; i < state.Length; i++) //Iterate through inventory index list and replace with item number
                if (state[i] != -1)
                    state[i] = int.Parse(clickableObjects[state[i]].transform.parent.gameObject.name.Substring(4, 2));
            return state;
        }
    }

    public int ActiveInventoryItemNumber
    {
        get
        {
            if (objectHeldList.First != null)
                return int.Parse(clickableObjects[objectHeldList.First.Value].transform.parent.gameObject.name.Substring(4, 2));
            else
                return -1;
        }
    }

    public int ActiveEventIndex
    {
        get
        {
            return currentItemTypeIndex;
        }
        set
        {
            currentItemTypeIndex = value;
        }
    }

    void Update()
    {
        if (firstCall)
        {
            //Needs to be run on first update because Start() order isn't promised
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
            Shuffle<ClickableObject>(clickable);
            clickableObjects = clickable.ToArray();
            for (int i = 0; i < clickableObjects.Length; i++)
                objectHeldList.AddFirst(i);
            firstCall = false;
        }
        Vector3 comparisonDistance = gameObject.transform.position;
        if (placeWithMouse)
        {
            Vector3 mouse = InputManager.mainManager.mouseWorldPosition;
            comparisonDistance = new Vector3(mouse.x, mousePlaceHeight*2, mouse.z);
        }

        if (InputManager.mainManager.GetButton(InputManager.ButtonType.NextEvent, InputManager.ButtonState.RisingEdge))
        {
            currentItemTypeIndex = (currentItemTypeIndex + 1) % 3;
            switch (currentItemTypeIndex)
            {
                case 0:
                    typeDisplayImage.material = infinityMaterial;
                    break;
                case 1:
                    typeDisplayImage.material = upMaterial;
                    break;
                case 2:
                    typeDisplayImage.material = downMaterial;
                    break;
                default:
                    typeDisplayImage.material = null;
                    break;
            }
        }

        else if (InputManager.mainManager.GetButton(InputManager.ButtonType.Place, InputManager.ButtonState.RisingEdge))
        {
            RaycastHit hit;
            var cameraCenter = targetingCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, targetingCamera.nearClipPlane));
            if(placeWithMouse)
                cameraCenter = InputManager.mainManager.mouseScreenPosition;
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
            }
            else //No object is being picked up, drop the current item
            {
                Debug.Log("No Hit, Placing.");
                if (objectHeldList.Count > 0)
                {
                    int index = objectHeldList.First.Value;
                    Texture2D prevTexture = clickableObjects[index].mainTexture;
                    Texture2D prevClickTexture = clickableObjects[index].clickTexture;
                    bool prevPlaySound = clickableObjects[index].playSoundEffect;
                    Transform prevParent = clickableObjects[index].gameObject.transform.parent.transform.parent;
                    FallFromSky fallScript = clickableObjects[index].gameObject.GetComponent<FallFromSky>();
                    FlyToSky flyScript = clickableObjects[index].gameObject.GetComponent<FlyToSky>();
                    int prevItemNum = int.Parse(clickableObjects[index].gameObject.transform.parent.gameObject.name.Substring(4));
                    GameObject obj;
                    Vector3 placePosition = transform.position + (transform.forward * placeDistance);
                    bool validPlacement = true;
                    for(int i = 0; i < clickableObjects.Length; i++) {
                        if (i == index) continue;
                        float dist = Vector3.Distance(placePosition, clickableObjects[i].transform.parent.position);
                        //If object is enabled and visible and within the minPlaceDistance, invalidate the placement
                        if (clickableObjects[i].isActiveAndEnabled && clickableObjects[i].GetComponent<MeshRenderer>().isVisible && dist < minObjectPlaceDistance)
                            validPlacement = false; //Invalid object placement, do nothing
                        if (placePosition == clickableObjects[i].transform.parent.position) //As an additional check, if the objects are in precisely the same place, invalidate as well
                            validPlacement = false;
                    }
                     if(time.time > 59.99 && currentItemTypeIndex != 0)
                    {
                        Debug.Log("Invalidated Placement Due To Event Type and Time");
                        validPlacement = false;
                    }
                    if (validPlacement)
                    {
                        Debug.Log("Valid Placement");
                        Debug.Log(prevItemNum);
                        objectHeldList.RemoveFirst();
                        // Check boundary conditions
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
                        if (placeWithMouse)
                        {
                            Vector3 mouse = InputManager.mainManager.mouseWorldPosition;
                            placePosition = new Vector3(mouse.x, mousePlaceHeight, mouse.z);
                        }
                        else
                        {
                            placePosition = new Vector3(placePosition.x, (float)ItemGenerator.itemHeight, placePosition.z);
                        }
                        if (currentItemTypeIndex == 2)
                            obj = ItemGenerator.GenerateFall(fallPrefabItem, prevParent, placePosition, prevTexture, prevClickTexture, time.time, time, prevItemNum, prevPlaySound);
                        else if (currentItemTypeIndex == 1)
                            obj = ItemGenerator.GenerateFly(flyPrefabItem, prevParent, placePosition, prevTexture, prevClickTexture, time.time, time, prevItemNum, prevPlaySound);
                        else
                            obj = ItemGenerator.GenerateFoil(foilPrefabItem, prevParent, placePosition, prevTexture, prevClickTexture, time.time, time, prevItemNum, prevPlaySound);
                        GameObject oldObj = clickableObjects[index].gameObject.transform.parent.gameObject;
                        clickableObjects[index] = obj.GetComponentInChildren<ClickableObject>();
                        DestroyImmediate(oldObj);
                        audioSrc.pitch = 1f;
                        audioSrc.clip = soundEffect;
                        audioSrc.Play();
                    }
                    else
                    {
                        Debug.Log("No valid placement.");
                    }
                }
            }
        }

        bool nextInputState = InputManager.mainManager.GetButton(InputManager.ButtonType.NextItem, InputManager.ButtonState.RisingEdge);
        if (nextInputState && objectHeldList.Count != 0) {
            objectHeldList.AddLast(objectHeldList.First.Value);
            objectHeldList.RemoveFirst();
        }

        if (objectHeldList.Count != 0)
            displayImage.material = clickableObjects[objectHeldList.First.Value].gameObject.GetComponent<MeshRenderer>().material;
        else
            displayImage.material = emptyMaterial;

        if (forceTransparency != 1f)
        {
            Material m = displayImage.material;
            m.SetFloat("_Mode", 2);
            m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            m.SetInt("_ZWrite", 0);
            m.DisableKeyword("_ALPHATEST_ON");
            m.EnableKeyword("_ALPHABLEND_ON");
            m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            m.renderQueue = 3000;
        }

        if (InputManager.mainManager.GetButton(InputManager.ButtonType.PickAll, InputManager.ButtonState.RisingEdge))
            for (int i = 0; i < clickableObjects.Length; i++)
            {
                if (clickableObjects[i].gameObject.transform.parent.gameObject.activeSelf)
                {
                    clickableObjects[i].gameObject.transform.parent.gameObject.SetActive(false);
                    objectHeldList.AddFirst(i);
                    
                    audioSrc.pitch = 1f;
                    audioSrc.clip = multiSoundEffect;
                    audioSrc.Play();
                }
            }

        
    }
}
