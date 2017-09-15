using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.IO;
using System.Text;


class ExportSplatmap : EditorWindow {
    [MenuItem("Assets/Export Texture")]
    static void Apply()
    {
        Texture2D texture = (Texture2D)Selection.activeObject;
        if (texture == null)
        {
            EditorUtility.DisplayDialog("Select Texture", "You Must Select a Texture first!", "Ok");
            return;
        }


        var bytes = texture.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/exported_texture.png", bytes);
    }
}