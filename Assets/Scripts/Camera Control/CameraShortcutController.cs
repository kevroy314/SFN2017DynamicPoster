using UnityEngine;
using System.Collections;
using System;

public class CameraShortcutController : MonoBehaviour {

    public KeyCode[] keys;
    public GameObject[] targets;
    public Vector3[] positions;
    public Vector3[] eulerRotations;
    public float[] times;
    public bool[] togglesActives;

    public struct Command
    {
        public KeyCode key;
        public GameObject target;
        public Vector3 position;
        public Vector3 rotation;
        public float time;
        public bool toggleActive;
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
        Array.Resize<GameObject>(ref targets, minLength);
        Array.Resize<Vector3>(ref positions, minLength);
        Array.Resize<Vector3>(ref eulerRotations, minLength);
        Array.Resize<float>(ref times, minLength);
        Array.Resize<bool>(ref togglesActives, minLength);

        commands = new Command[minLength];

        for (int i = 0; i < minLength; i++)
        {
            commands[i] = new Command();
            commands[i].key = keys[i];
            commands[i].target = targets[i];
            commands[i].position = positions[i];
            commands[i].rotation = eulerRotations[i];
            commands[i].time = times[i];
            commands[i].toggleActive = togglesActives[i];
        }
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < commands.Length; i++)
            if (Input.GetKeyUp(commands[i].key))
            {
                // Debug.Log("Key " + commands[i].key + " detected.");
                Hashtable cmd;

                cmd = new Hashtable();
                cmd.Add("position", commands[i].position);
                cmd.Add("time", commands[i].time);
                cmd.Add("islocal", true);
                // Debug.Log("Moving to " + commands[i].position.ToString() + " in " + commands[i].time + " seconds.");
                iTween.MoveTo(commands[i].target, cmd);

                cmd = new Hashtable();
                cmd.Add("rotation", commands[i].rotation);
                cmd.Add("time", commands[i].time);
                cmd.Add("islocal", true);
                // Debug.Log("Rotating to " + commands[i].rotation.ToString() + " in " + commands[i].time + " seconds.");
                iTween.RotateTo(commands[i].target, cmd);

                if (commands[i].toggleActive)
                    commands[i].target.SetActive(!commands[i].target.activeSelf);
            }
	}
}
