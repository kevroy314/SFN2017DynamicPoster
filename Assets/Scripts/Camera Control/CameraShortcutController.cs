using UnityEngine;
using System.Collections;
using System;

public class CameraShortcutController : MonoBehaviour {

    public KeyCode[] keys;
    public Vector3[] positions;
    public Vector3[] eulerRotations;
    public float[] times;

    public struct Command
    {
        public KeyCode key;
        public Vector3 position;
        public Vector3 rotation;
        public float time;
    }

    public Command[] commands;

	// Use this for initialization
	void Start () {
        int[] lengths = new int[] { keys.Length, positions.Length, eulerRotations.Length, times.Length };
        int minLength = int.MaxValue;
        for (int i = 0; i < lengths.Length; i++)
            if (lengths[i] < minLength)
                minLength = lengths[i];

        Array.Resize<KeyCode>(ref keys, minLength);
        Array.Resize<Vector3>(ref positions, minLength);
        Array.Resize<Vector3>(ref eulerRotations, minLength);
        Array.Resize<float>(ref times, minLength);

        commands = new Command[minLength];

        for (int i = 0; i < minLength; i++)
        {
            commands[i] = new Command();
            commands[i].key = keys[i];
            commands[i].position = positions[i];
            commands[i].rotation = eulerRotations[i];
            commands[i].time = times[i];
        }
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < commands.Length; i++)
            if (Input.GetKey(commands[i].key))
            {
                // Debug.Log("Key " + commands[i].key + " detected.");
                Hashtable cmd;

                cmd = new Hashtable();
                cmd.Add("position", commands[i].position);
                cmd.Add("time", commands[i].time);
                cmd.Add("islocal", true);
                // Debug.Log("Moving to " + commands[i].position.ToString() + " in " + commands[i].time + " seconds.");
                iTween.MoveTo(gameObject, cmd);

                cmd = new Hashtable();
                cmd.Add("rotation", commands[i].rotation);
                cmd.Add("time", commands[i].time);
                cmd.Add("islocal", true);
                // Debug.Log("Rotating to " + commands[i].rotation.ToString() + " in " + commands[i].time + " seconds.");
                iTween.RotateTo(gameObject, cmd);
            }
	}
}
