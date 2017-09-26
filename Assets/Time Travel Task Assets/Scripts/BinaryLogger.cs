using UnityEngine;
using System.Collections;
using System.IO;
using System;
using Chronos;
using System.Collections.Generic;

public class BinaryLogger : MonoBehaviour {

    public string dateTimeFormat = "";
    public string filenameFormat = "<sub>_<trial>_<phase>_<inv>_<datetime>.dat";
    private string header = "";
    private static int headerLength = 1024;

    public Timeline time;
    public Camera cam;

    public List<KeyCode> keys;
    public List<string> buttons;

    public ItemGenerator generator;
    public InventoryManager manager;
    private ClickableObject[] items;
    public BoundaryManager boundaries;

    private BinaryWriter writer;

    private bool firstUpdate = true;

    private int expectedNumItems;

    public bool followMousePosition = false;

	// Use this for initialization
	void Start () {
        string filename = filenameFormat;
        if (PlayerPrefs.HasKey("sub"))
            filename = filename.Replace("<sub>", PlayerPrefs.GetString("sub"));
        else
            filename = filename.Replace("<sub>", "unk");
        if(PlayerPrefs.HasKey("trial"))
            filename = filename.Replace("<trial>", "" + PlayerPrefs.GetInt("trial"));
        else
            filename = filename.Replace("<trial>", "u");
        if(PlayerPrefs.HasKey("phase"))
            filename = filename.Replace("<phase>", "" + PlayerPrefs.GetInt("phase"));
        else
            filename = filename.Replace("<phase>", "u");
        if (PlayerPrefs.HasKey("inv"))
            filename = filename.Replace("<inv>", "" + PlayerPrefs.GetInt("inv"));
        else
            filename = filename.Replace("<inv>", "u");
        DateTime time = DateTime.Now;
        string timeString = time.ToString(dateTimeFormat);
        filename = filename.Replace("<datetime>", timeString);
        Stream stream = new StreamWriter(filename).BaseStream;
        writer = new BinaryWriter(stream);

        if (PlayerPrefs.GetInt("phase") >= 6) followMousePosition = true;
	}
	
	// Update is called once per frame
	void Update () {
        items = generator.getItems();
        if (firstUpdate)
        {
            expectedNumItems = generator.expectedNumItems;
            header = "version,2,time,f,timeScale,f,posXYZ,fff,rotXYZW,ffff,";
            for (int i = 0; i < keys.Count; i++)
                header += "key" + keys[i].ToString() + "_" + i.ToString().PadLeft(2, '0') + ",b,";
            for (int i = 0; i < buttons.Count; i++)
                header += "button" + buttons[i].ToString() + "_" + i.ToString().PadLeft(2, '0') + ",b,";
            for (int i = 0; i < items.Length; i++)
                header += "itemXYZActiveClickedEventTime" + i.ToString().PadLeft(2, '0') + ",fffbbif,";
            header += "boundaryNum,i,boundaryColor,fff,";
            header += "inventoryItemNumbers,";
            for (int i = 0; i < items.Length; i++)
                header += "i";
            header += ",activeInventoryItemNumber,i,activeInventoryEventIndex,i";

            if (header.Length > headerLength)
            {
                Debug.LogError("Error: Header input length is longer than the 1k Maximum.");
                Application.Quit();
            }

            if (header.Length < headerLength)
                header = header.PadRight(headerLength);

            writer.Write(header);

            firstUpdate = false;
        }
        writer.Write(DateTime.Now.ToBinary());
        writer.Write(time.time);
        writer.Write(time.timeScale);
        if (followMousePosition)
        {
            Vector3 mouse = cam.ScreenToWorldPoint(InputManager.mainManager.mouseScreenPosition);
            writer.Write(mouse.x);
            writer.Write(mouse.y);
            writer.Write(mouse.z);
            writer.Write(InputManager.mainManager.isButtonDown(InputManager.ButtonType.Place) ? 1f : 0f);
            writer.Write(InputManager.mainManager.isButtonDown(InputManager.ButtonType.NextEvent) ? 1f : 0f);
            writer.Write(InputManager.mainManager.isButtonDown(InputManager.ButtonType.NextItem) ? 1f : 0f);
            writer.Write(InputManager.mainManager.isButtonDown(InputManager.ButtonType.NextState) ? 1f : 0f);
        }
        else
        {
            writer.Write(cam.transform.position.x);
            writer.Write(cam.transform.position.y);
            writer.Write(cam.transform.position.z);
            writer.Write(cam.transform.rotation.x);
            writer.Write(cam.transform.rotation.y);
            writer.Write(cam.transform.rotation.z);
            writer.Write(cam.transform.rotation.w);
        }
        for(int i = 0; i < keys.Count; i++)
            writer.Write(Input.GetKey(keys[i]));
        for (int i = 0; i < buttons.Count; i++)
        {
            bool state = false;
            try { state = Input.GetButton(buttons[i]); } catch (ArgumentException) { }
            writer.Write(state);
        }
        for (int i = 0; i < expectedNumItems; i++)
        {
            float _x = float.MinValue;
            float _y = float.MinValue;
            float _z = float.MinValue;
            bool _activeSelf = false;
            bool _hasBeenClicked = false;
            int _eventType = -1;
            float _eventTime = float.MinValue;
            try
            {
                _x = items[i].transform.position.x;
                _y = items[i].transform.position.y;
                _z = items[i].transform.position.z;
                _activeSelf = items[i].gameObject.transform.parent.gameObject.activeSelf;
                _hasBeenClicked = items[i].HasBeenClicked();
                if (items[i].gameObject.GetComponent<Foil>() != null)
                {
                    _eventType = 0;
                    _eventTime = 0f;
                }
                else if (items[i].gameObject.GetComponent<FlyToSky>() != null)
                {
                    _eventType = 1;
                    _eventTime = items[i].gameObject.GetComponent<FlyToSky>().transitionDelay;
                }
                else if (items[i].gameObject.GetComponent<FallFromSky>() != null)
                {
                    _eventType = 2;
                    _eventTime = items[i].gameObject.GetComponent<FallFromSky>().transitionDelay;
                }
            }
            catch (Exception) { };
            writer.Write(_x);
            writer.Write(_y);
            writer.Write(_z);
            writer.Write(_activeSelf);
            writer.Write(_hasBeenClicked);
            writer.Write(_eventType);
            writer.Write(_eventTime);
        }
        writer.Write(boundaries.getCurrentState());
        Color c = boundaries.getCurrentColor();
        writer.Write(c.r);
        writer.Write(c.g);
        writer.Write(c.b);

        int[] _inventoryItemNumbers = new int[expectedNumItems];
        for (int i = 0; i < _inventoryItemNumbers.Length; i++)
            _inventoryItemNumbers[i] = -1;
        int _activeInventoryItemNumber = -1;
        int _activeInventoryEventIndex = -1;
        if(manager != null)
        {
            _inventoryItemNumbers = manager.InventoryState;
            _activeInventoryItemNumber = manager.ActiveInventoryItemNumber;
            _activeInventoryEventIndex = manager.ActiveEventIndex;
        }
        for (int i = 0; i < _inventoryItemNumbers.Length; i++)
            writer.Write(_inventoryItemNumbers[i]);
        writer.Write(_activeInventoryItemNumber);
        writer.Write(_activeInventoryEventIndex);
    }

    void OnApplicationQuit()
    {
        writer.Close();
    }

    void OnDisable()
    {
        writer.Close();
    }
}
