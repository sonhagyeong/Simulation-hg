using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ExitPlayMode : MonoBehaviour
{
    // Update is called once per frame
    private void Update()
    {
#if UNITY_EDITOR
        // Check if there are no objects in the hierarchy starting with "Truck"
        bool noTruckObjects = true;
        foreach (Transform obj in GameObject.FindObjectsOfType<Transform>())
        {
            if (obj.name.StartsWith("Truck"))
            {
                noTruckObjects = false;
                break;
            }
        }

        // Exit play mode if there are no Truck objects
        if (noTruckObjects)
        {
            EditorApplication.ExitPlaymode();
        }
#endif
    }
}
