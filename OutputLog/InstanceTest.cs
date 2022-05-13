using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceTest : MonoBehaviour
{
    private void OnGUI()
    {

        if (GUI.Button(new Rect(100, 160, 100, 50), new GUIContent("Save")))
        {
            AssetsLoadCollection.SaveData();
        }
    }
}
