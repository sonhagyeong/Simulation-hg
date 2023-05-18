using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTruckData : MonoBehaviour
{
    public string Name { get; private set; }
    public string Route { get; private set; }

    public void CreateData(string name, string route)
    {
        Name = name;
        Route = route;
    }
}
