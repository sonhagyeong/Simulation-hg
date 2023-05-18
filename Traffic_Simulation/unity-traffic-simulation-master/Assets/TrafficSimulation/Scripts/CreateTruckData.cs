using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CreateTruckData : ScriptableObject
{
    public string Name { get; private set; }
    public string Route { get; private set; }

    public void CreateData(string name, string route)
    {
        Name = name;
        Route = route;
    }
}
