using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public class PathRenderer : MonoBehaviour
{
    public bool useTestData = false;

    public string pathFile;

    public Vector3 scaleOnLoad = new Vector3(1f, 1f, 1f);
    public Vector3 offsetOnLoad = new Vector3(0f, 0f, 0f);

    public float animationSpeed = 0.1f;
    public bool loop = true;
    
    public enum PlayDirection
    {
        Pause=0,
        Forward=1,
        Backward=2
    }

    struct PathData
    {
        public Vector3[] verticies;
        public Color[] colors;
        public int vCount;
    }

    public PlayDirection playDirection = PlayDirection.Forward;

    private float startRenderIdxApprox = 0;
    private float endRenderIdxApprox = -1;

    static Material lineMaterial;
    private int vertexCount;
    private Vector3[] vertexList;
    private Color[] colorList;

    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    PathData LoadPathFile(string path)
    {
        PathData dat = new PathData();

        BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open));

        string header = reader.ReadString();

        int numKeys = header.Split(new string[] { "key" }, System.StringSplitOptions.None).Length - 1;
        int numBtns = header.Split(new string[] { "button" }, System.StringSplitOptions.None).Length - 1;
        int numItems = header.Split(new string[] { "itemXYZActiveClickedEventTime" }, System.StringSplitOptions.None).Length - 1;

        List<DateTime> dateTimes = new List<DateTime>();
        List<float> times = new List<float>();
        List<float> timeScales = new List<float>();
        List<Vector3> positions = new List<Vector3>();
        List<Quaternion> rotations = new List<Quaternion>();
        List<bool[]> keys = new List<bool[]>();
        List<bool[]> btns = new List<bool[]>();
        List<Vector3[]> itemPositions = new List<Vector3[]>();
        List<bool[]> itemActives = new List<bool[]>();
        List<bool[]> itemClickeds = new List<bool[]>();
        List<int[]> itemEvents = new List<int[]>();
        List<float[]> itemTimes = new List<float[]>();
        List<int> boundaryNums = new List<int>();
        List<Color> boundaryColors = new List<Color>();
        List<int[]> inventoryItemNumbers = new List<int[]>();
        List<int> activeInventoryItemNumbers = new List<int>();
        List<int> activeInventoryEventIndicies = new List<int>();

        while (reader.BaseStream.Position != reader.BaseStream.Length)
        {
            
            dateTimes.Add(DateTime.FromBinary(reader.ReadInt64()));
            times.Add(reader.ReadSingle());
            timeScales.Add(reader.ReadSingle());
            positions.Add(new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
            rotations.Add(new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));

            bool[] k = new bool[numKeys];
            for (int i = 0; i < numKeys; i++) k[i] = reader.ReadBoolean();
            keys.Add(k);

            bool[] b = new bool[numBtns];
            for (int i = 0; i < numBtns; i++) b[i] = reader.ReadBoolean();
            keys.Add(b);

            Vector3[] iPos = new Vector3[numItems];
            bool[] iAct = new bool[numItems];
            bool[] iCli = new bool[numItems];
            int[] iEve = new int[numItems];
            float[] iTim = new float[numItems];
            for (int i = 0; i < numItems; i++)
            {
                iPos[i] = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                iAct[i] = reader.ReadBoolean();
                iCli[i] = reader.ReadBoolean();
                iEve[i] = reader.ReadInt32();
                iTim[i] = reader.ReadSingle();
            }
            itemPositions.Add(iPos);
            itemActives.Add(iAct);
            itemClickeds.Add(iCli);
            itemEvents.Add(iEve);
            itemTimes.Add(iTim);

            boundaryNums.Add(reader.ReadInt32());
            boundaryColors.Add(new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));

            int[] iItemNums = new int[numItems];
            for (int i = 0; i < numItems; i++) iItemNums[i] = reader.ReadInt32();
            inventoryItemNumbers.Add(iItemNums);

            activeInventoryItemNumbers.Add(reader.ReadInt32());
            activeInventoryEventIndicies.Add(reader.ReadInt32());
        }

        dat.vCount = times.Count;
        dat.verticies = new Vector3[times.Count + 1];
        dat.colors = new Color[times.Count];
        for (int i = 0; i < times.Count; i++)
        {
            if(timeScales[i] >= 0)
                dat.colors[i] = Color.white;
            else
                dat.colors[i] = Color.magenta;

            dat.verticies[i] = new Vector3(positions[i].x, positions[i].z, times[i]);
        }

        dat.verticies[dat.verticies.Length - 1] = dat.verticies[dat.verticies.Length - 2];

        return dat;
    }

    PathData GetTestPathData()
    {
        int lineCount = 100;
        Vector3[] vList = new Vector3[lineCount + 1];
        Color[] cList = new Color[lineCount];
        for (int i = 0; i < vList.Length; i++)
        {
            if (i < lineCount)
            {
                float a = i / (float)lineCount;
                float angle = a * Mathf.PI * 2;
                cList[i] = new Color(a, 1 - a, 0, 0.8F);
            }

            float x = Mathf.Sin((float)i * 0.5f) * 0.3f;
            float y = Mathf.Cos((float)i * 0.5f) * 0.3f;
            float z = (float)i * 0.01f - 0.5f;
            vList[i] = new Vector3(x, y, z);
        }

        PathData output = new PathData();
        output.vCount = lineCount;
        output.verticies = vList;
        output.colors = cList;

        return output;
    }

    PathData ScalePathData(PathData data, Vector3 scalingVector, Vector3 offsetVector)
    {
        for(int i = 0; i < data.verticies.Length; i++)
        {
            data.verticies[i].Scale(scalingVector);
            data.verticies[i] += offsetVector;
        }
        return data;
    }

    void InitializePath(PathData data)
    {
        vertexList = data.verticies;
        colorList = data.colors;
        vertexCount = data.vCount;
    }

    void Start()
    {
        if (useTestData)
        {
            PathData data = GetTestPathData();
            InitializePath(data);
        }
        else
        {
            if (pathFile == null || pathFile == "" || !File.Exists(pathFile))
                Debug.Log("No Valid Path File Provided or File Doesn't Exist.");
            else
            {
                PathData data = LoadPathFile(pathFile);
                data = ScalePathData(data, scaleOnLoad, offsetOnLoad);
                InitializePath(data);
            }
        }

        startRenderIdxApprox = 0f;
        endRenderIdxApprox = (float)vertexCount;
    }

    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        float updateInterval = 0f;
        if (playDirection == PlayDirection.Forward) updateInterval = animationSpeed;
        if (playDirection == PlayDirection.Backward) updateInterval = -animationSpeed;
        if (loop)
            endRenderIdxApprox = (endRenderIdxApprox + updateInterval + vertexCount) % vertexCount;
        else
        {
            endRenderIdxApprox = (endRenderIdxApprox + updateInterval);
            endRenderIdxApprox = Mathf.Clamp(endRenderIdxApprox, 0f, vertexCount - 1);
        }

        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        // Draw lines
        GL.Begin(GL.LINES);
        for (int i = Mathf.RoundToInt(startRenderIdxApprox); i < Mathf.RoundToInt(endRenderIdxApprox); ++i)
        {
            
            // Vertex colors change from red to green
            GL.Color(colorList[i]);
            // One vertex at transform position
            GL.Vertex3(vertexList[i].x, vertexList[i].y, vertexList[i].z);
            // Another vertex at edge of circle
            GL.Vertex3(vertexList[i + 1].x, vertexList[i + 1].y, vertexList[i + 1].z);
        }
        GL.End();
        GL.PopMatrix();
    }
}