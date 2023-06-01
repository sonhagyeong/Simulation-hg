using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CreateTruckData : ScriptableObject
{
    public string Name { get; private set; }
    public string Route { get; private set; }
    public List<Vector3> WorkStations { get; private set; }
    // public List<Vector3> PickupStations { get; private set; }
    // public List<Vector3> DropStations { get; private set; }

    public void CreateData(string name, string route, List<Vector3> stations)
    {
        Name = name;
        Route = route;
        WorkStations = stations; 
    }

    // public void CreateData(string name, string route, List<Vector3> pickup, List<Vector3> drop)
    // {
    //     Name = name;
    //     Route = route;
    //     PickupStations = pickup;
    //     DropStations = drop;
    // }
  
}
