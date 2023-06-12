using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficSimulation{
    public class Corner : MonoBehaviour
    {
        // Start is called before the first frame update
        void OnTriggerEnter(Collider other)
        {
            VehicleAI vehicleAI = other.GetComponent<VehicleAI>();
            other.GetComponent<TruckInfo>().nowStatus = NowStatus.WAITING;
            vehicleAI.vehicleStatus = Status.SLOW_DOWN;
        }

        void OnTriggerExit(Collider other)
        {
            other.GetComponent<VehicleAI>().vehicleStatus = Status.GO;
            other.GetComponent<TruckInfo>().nowStatus = NowStatus.NONE;

        }
    }
}
