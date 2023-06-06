using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteAlways]
public class CoordinateLabeler : MonoBehaviour
{
    TextMeshPro label;
    Vector2Int coordinates = new Vector2Int();
    void Awake()
    {
        label = GetComponent<TextMeshPro>();
        DisplayCoordinates();
        label.enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
        if(!Application.isPlaying)
        {
            // do only editor mode
            DisplayCoordinates();
            UpdateObjectName();
        }

        ToggleLabels();
    }

    void DisplayCoordinates()
    {   
        coordinates.x = Mathf.RoundToInt(transform.parent.position.x );
        coordinates.y = Mathf.RoundToInt(transform.parent.position.z );
        // coordinates.x = Mathf.RoundToInt(transform.parent.position.x / UnityEditor.EditorSnapSettings.move.x );
        // coordinates.y = Mathf.RoundToInt(transform.parent.position.z / UnityEditor.EditorSnapSettings.move.z );

        label.text = coordinates.x + ", " + coordinates.y;
    }

    void UpdateObjectName()
    {
        transform.parent.name = coordinates.ToString();
    }

    void ToggleLabels()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            label.enabled = !label.enabled;
        }
    }
}
