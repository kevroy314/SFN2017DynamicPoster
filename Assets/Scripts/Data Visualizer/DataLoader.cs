using UnityEngine;
using System.Collections;
using System;

public class DataLoader : MonoBehaviour {

    public PathRenderer pathRenderer;

    public KeyCode[] loadKeys;
    public string[] loadPaths;
    public bool[] loadTestDataToggles;

    public struct Command
    {
        public KeyCode key;
        public string path;
        public bool testDataToggle;
        public PathRenderer.PathData data;
    }

    public Command[] commands;

    void Start()
    {
        int[] lengths = new int[] { loadKeys.Length, loadPaths.Length, loadTestDataToggles.Length };
        int minLength = int.MaxValue;
        for (int i = 0; i < lengths.Length; i++)
            if (lengths[i] < minLength)
                minLength = lengths[i];

        Array.Resize<KeyCode>(ref loadKeys, minLength);
        Array.Resize<string>(ref loadPaths, minLength);
        Array.Resize<bool>(ref loadTestDataToggles, minLength);

        commands = new Command[minLength];

        for (int i = 0; i < commands.Length; i++)
        {
            commands[i] = new Command();
            commands[i].key = loadKeys[i];
            commands[i].path = loadPaths[i];
            commands[i].testDataToggle = loadTestDataToggles[i];
        }

        PrefetchData();
    }

    void PrefetchData()
    {
        string tmpPath = pathRenderer.pathFile;
        bool tmpToggle = pathRenderer.loadItemDataFromTest;
        for (int i = 0; i < commands.Length; i++)
        {
            pathRenderer.pathFile = commands[i].path;
            pathRenderer.loadItemDataFromTest = commands[i].testDataToggle;
            commands[i].data = PathRenderer.LoadPathFile(commands[i].path);
            commands[i].data = PathRenderer.ScalePathData(commands[i].data, pathRenderer.scaleOnLoad, pathRenderer.offsetOnLoad);
        }
        pathRenderer.pathFile = tmpPath;
        pathRenderer.loadItemDataFromTest = tmpToggle;
    }

    // Update is called once per frame
    void Update () {
        for (int i = 0; i < commands.Length; i++)
            if (Input.GetKeyUp(commands[i].key))
            {
                pathRenderer.pathFile = commands[i].path;
                pathRenderer.loadItemDataFromTest = commands[i].testDataToggle;
                pathRenderer.Reload(commands[i].data);
            }
	}
}
