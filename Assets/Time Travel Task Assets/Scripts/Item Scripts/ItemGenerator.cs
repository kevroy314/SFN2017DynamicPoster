using Chronos;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{

    public GameObject fallPrefabItem;
    public GameObject flyPrefabItem;
    public GameObject foilPrefabItem;
    public static double itemHeight = 1.0;
    public static double itemFallTime = 1.0;
    public static double itemInactiveHeight = 10.0;
    public bool invertTimeline = false;
    public Timeline time;
    public BinaryLogger logger;
    public int expectedNumItems;

    enum ItemTypes
    {
        Fall = 0, Fly = 1, Foil = 2
    }

    // Use this for initialization
    void Start()
    {
        string configFile;
        if (CharacterConfigurationLoader.getConfigFileNameFromPlayerPrefs && PlayerPrefs.HasKey(CharacterConfigurationLoader.configFilePlayerPrefsString))
            configFile = PlayerPrefs.GetString(CharacterConfigurationLoader.configFilePlayerPrefsString);
        else
            configFile = CharacterConfigurationLoader.configFile;
        INIParser ini = new INIParser();
        ini.Open(Application.dataPath + '/' + configFile);
        string imgRootPath = Application.dataPath + "/ItemImages/";
        int numItems = ini.ReadValue("Global", "NumItems", 0);
        expectedNumItems = numItems;
        float y = (float)ini.ReadValue("Global", "ItemHeight", itemHeight);
        itemHeight = y;
        float fallTime = (float)ini.ReadValue("Global", "ItemFallTime", itemFallTime);
        itemFallTime = fallTime;
        float inactiveHeight = (float)ini.ReadValue("Global", "ItemInactiveHeight", itemInactiveHeight);
        itemInactiveHeight = inactiveHeight;
        float endTime = (float)ini.ReadValue("Global", "EndTime", 10.0);
        string itemKey = "Item";

        if (PlayerPrefs.HasKey("LoaderInversion"))
            invertTimeline = PlayerPrefs.GetInt("LoaderInversion") != 0;

        Vector3[] locations = new Vector3[numItems];
        float[] delays = new float[numItems];
        ItemTypes[] types = new ItemTypes[numItems];
        Texture2D[] images = new Texture2D[numItems];
        Texture2D[] clickImages = new Texture2D[numItems];
        bool[] playSoundEffect = new bool[numItems];
        for (int i = 0; i < numItems; i++)
        {
            float x = (float)ini.ReadValue("Items", itemKey + i + "X", 0.0);
            float z = (float)ini.ReadValue("Items", itemKey + i + "Z", 0.0);
            locations[i] = new Vector3(x, y, z);
            float delay = (float)ini.ReadValue("Items", itemKey + i + "Delay", 0.0);
            delays[i] = delay;
            string typeString = ini.ReadValue("Items", itemKey + i + "Type", "foil");
            switch (typeString)
            {
                case "foil":
                    types[i] = ItemTypes.Foil;
                    break;
                case "fly":
                    types[i] = ItemTypes.Fly;
                    break;
                case "fall":
                    types[i] = ItemTypes.Fall;
                    break;
                default:
                    types[i] = ItemTypes.Foil;
                    break;
            }
            string imageFilename = imgRootPath + ini.ReadValue("Items", itemKey + i + "Image", "error.png");
            string clickImageFilename = imgRootPath + ini.ReadValue("Items", itemKey + i + "ClickImage", "error.png");
            playSoundEffect[i] = ini.ReadValue("Items", itemKey + i + "AudioEnabled", 1) != 0;
            byte[] fileData;
            if (File.Exists(imageFilename))
            {
                fileData = File.ReadAllBytes(imageFilename);
                Texture2D itemTexture = new Texture2D(400, 400);
                itemTexture.LoadImage(fileData);
                images[i] = itemTexture;

                fileData = File.ReadAllBytes(clickImageFilename);
                Texture2D clickItemTexture = new Texture2D(400, 400);
                clickItemTexture.LoadImage(fileData);
                clickImages[i] = clickItemTexture;
            }
            else
                Debug.Log("Could not load texture (" + imageFilename + "). File does not exist.");
        }

        ini.Close();

        if (invertTimeline)
            for (int i = 0; i < locations.Length; i++)
            {
                if (types[i] == ItemTypes.Fall)
                    types[i] = ItemTypes.Fly;
                else if (types[i] == ItemTypes.Fly)
                    types[i] = ItemTypes.Fall;
                delays[i] = endTime - delays[i];
            }

        for (int i = 0; i < locations.Length; i++)
        {
            if (types[i] == ItemTypes.Fall)
                GenerateFall(fallPrefabItem, transform, locations[i], images[i], clickImages[i], delays[i], time, i, playSoundEffect[i]);
            else if (types[i] == ItemTypes.Fly)
                GenerateFly(flyPrefabItem, transform, locations[i], images[i], clickImages[i], delays[i], time, i, playSoundEffect[i]);
            else if (types[i] == ItemTypes.Foil)
                GenerateFoil(foilPrefabItem, transform, locations[i], images[i], clickImages[i], delays[i], time, i, playSoundEffect[i]);
        }
    }

    public static GameObject GenerateFall(GameObject fallPrefabItem, Transform parent, Vector3 location, Texture2D image, Texture2D clickImage, float delay, Timeline time, int itemNum, bool playSound)
    {
        GameObject tmp = Instantiate(fallPrefabItem);
        tmp.name = "Item" + itemNum.ToString().PadLeft(2, '0');
        tmp.layer = 5;
        tmp.transform.GetChild(0).gameObject.layer = 5;
        tmp.transform.GetChild(1).gameObject.layer = 5;
        DisableAllCollidersInObject(tmp);
        tmp.transform.parent = parent;
        tmp.transform.localPosition = location;
        tmp.GetComponentInChildren<BoxCollider>().enabled = true;
        tmp.GetComponentInChildren<BoxCollider>().isTrigger = true;
        if (image != null)
        {
            tmp.GetComponentInChildren<MeshRenderer>().material.mainTexture = image;
            flipTexture(tmp);
        }
        FallFromSky script = tmp.GetComponentInChildren<FallFromSky>();
        script.transitionDelay = delay;
        script.transitionDuration = (float)itemFallTime;
        script.startPos = new Vector3(script.startPos.x, (float)itemInactiveHeight, script.startPos.z);
        script.time = time;
        script.mainTexture = image;
        script.clickTexture = clickImage;
        script.playSoundEffect = playSound;
        return tmp;
    }

    public static GameObject GenerateFly(GameObject flyPrefabItem, Transform parent, Vector3 location, Texture2D image, Texture2D clickImage, float delay, Timeline time, int itemNum, bool playSound)
    {
        GameObject tmp = Instantiate(flyPrefabItem);
        tmp.name = "Item" + itemNum.ToString().PadLeft(2, '0');
        tmp.layer = 5;
        tmp.transform.GetChild(0).gameObject.layer = 5;
        tmp.transform.GetChild(1).gameObject.layer = 5;
        DisableAllCollidersInObject(tmp);
        tmp.transform.parent = parent;
        tmp.transform.localPosition = location;
        tmp.GetComponentInChildren<BoxCollider>().enabled = true;
        tmp.GetComponentInChildren<BoxCollider>().isTrigger = true;
        if (image != null)
        {
            tmp.GetComponentInChildren<MeshRenderer>().material.mainTexture = image;
            flipTexture(tmp);
        }
        FlyToSky script = tmp.GetComponentInChildren<FlyToSky>();
        script.transitionDelay = delay;
        script.transitionDuration = (float)itemFallTime;
        script.endPos = new Vector3(script.endPos.x, (float)itemInactiveHeight, script.endPos.z);
        script.time = time;
        script.mainTexture = image;
        script.clickTexture = clickImage;
        script.playSoundEffect = playSound;
        return tmp;
    }

    public static GameObject GenerateFoil(GameObject foilPrefabItem, Transform parent, Vector3 location, Texture2D image, Texture2D clickImage, float delay, Timeline time, int itemNum, bool playSound)
    {
        GameObject tmp = Instantiate(foilPrefabItem);
        tmp.name = "Item" + itemNum.ToString().PadLeft(2, '0');
        tmp.layer = 5;
        tmp.transform.GetChild(0).gameObject.layer = 5;
        tmp.transform.GetChild(1).gameObject.layer = 5;
        DisableAllCollidersInObject(tmp);
        tmp.GetComponentInChildren<BoxCollider>().enabled = true;
        tmp.GetComponentInChildren<BoxCollider>().isTrigger = true;
        if (image != null)
        {
            tmp.GetComponentInChildren<MeshRenderer>().material.mainTexture = image;
            flipTexture(tmp);
        }
        tmp.transform.parent = parent;
        tmp.transform.localPosition = location;
        Foil script = tmp.GetComponentInChildren<Foil>();
        script.transitionDelay = delay;
        script.transitionDuration = (float)itemFallTime;
        script.Time = time;
        script.mainTexture = image;
        script.clickTexture = clickImage;
        script.playSoundEffect = playSound;
        return tmp;
    }

    public static GameObject GenerateNull(GameObject nullPrefabItem, Transform parent, Vector3 location, Material overrideMaterial, Texture2D image, Texture2D clickImage, float delay, Timeline time, int itemNum, bool playSound)
    {
        GameObject tmp = Instantiate(nullPrefabItem);
        tmp.name = "Item" + itemNum.ToString().PadLeft(2, '0');
        tmp.layer = 5;
        tmp.transform.GetChild(0).gameObject.layer = 5;
        tmp.transform.GetChild(1).gameObject.layer = 5;
        DisableAllCollidersInObject(tmp);
        tmp.GetComponentInChildren<BoxCollider>().enabled = true;
        tmp.GetComponentInChildren<BoxCollider>().isTrigger = true;
        if (image != null)
        {
            tmp.GetComponentInChildren<MeshRenderer>().material.mainTexture = image;
            flipTexture(tmp);
        }
        tmp.transform.parent = parent;
        tmp.transform.localPosition = location;
        NullEvent script = tmp.GetComponentInChildren<NullEvent>();
        script.transitionDelay = delay;
        script.transitionDuration = (float)itemFallTime;
        script.localTime = time;
        script.time = time;
        script.mainTexture = image;
        script.clickTexture = clickImage;
        script.playSoundEffect = playSound;
        script.changeTexture = false;
        tmp.GetComponentInChildren<MeshRenderer>().material = overrideMaterial;
        return tmp;
    }

    private static void DisableAllCollidersInObject(GameObject obj)
    {
        Collider[] colliders = obj.GetComponentsInChildren<BoxCollider>();
        for (int i = 0; i < colliders.Length; i++)
            colliders[i].enabled = false;
        try { obj.GetComponent<BoxCollider>().enabled = false; } catch (System.Exception) { }
    }

    public static void flipTexture(GameObject obj)
    {
        //Get the mesh filter for this cube
        MeshFilter mf = obj.GetComponentInChildren<MeshFilter>();
        Mesh mesh = null;
        if (mf != null)
            mesh = mf.mesh;

        if (mesh == null || mesh.uv.Length != 24)
        {
            Debug.Log("Script needs to be attached to built-in cube");
            return;
        }

        //Get the current UVs (probably all 0,0;1,0;0,1;1,1)
        Vector2[] uvs = mesh.uv;

        // Back side UV flipped
        uvs[10] = new Vector2(0.0f, 0.0f);
        uvs[11] = new Vector2(-1f, 0.0f);
        uvs[6] = new Vector2(0.0f, -1f);
        uvs[7] = new Vector2(-1f, -1f);

        // Set the output UV once and it will be fixed for the rest of the object lifetime
        mesh.uv = uvs;
    }

    public ClickableObject[] getItems()
    {
        List<ClickableObject> objs = new List<ClickableObject>();
        int numItems = transform.childCount;
        for(int i = 0; i < numItems; i++)
        {
            ClickableObject c = transform.GetChild(i).GetComponentInChildren<ClickableObject>();
            if (c != null)
                objs.Add(c);
        }
        
        return objs.OrderBy(o => o.transform.parent.gameObject.name).ToArray();
    }
}
