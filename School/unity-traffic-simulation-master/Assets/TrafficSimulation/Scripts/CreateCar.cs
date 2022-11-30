using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TrafficSimulation{

    public class CreateCar : MonoBehaviour
    {   
        public float Y_Position = 1.5f;
        // 뽑을 Segment
        // public GameObject Segment;
        // 뽑을 트럭
        public GameObject Truck;
        // Segment별 Position
        public Vector3 segment_position;
        // Truck Position
        public Vector3 Truck_position;
        private TrafficSystem trafficSystem = null;
        

        // Start is called before the first frame update
        void Start()
        {   
            GetSegmentPoint();
            // Create();
        }

        // 뽑은 Segment position 얻기
        void GetSegmentPoint()
        {   
            List<TrafficSystem> trafficSystem_list = new List<TrafficSystem>();
            trafficSystem = FindObjectOfType<TrafficSystem>();
            // foreach(TrafficSystem ts in trafficSystem)
            // {
            //     trafficSystem_list.AddRange(ts);
            // }
            // Debug.Log(trafficSystem.name);
            // for(TrafficSystem i; i in trafficSystem;)             
            // {
            //     Debug.Log("trafficSystem : "+ i);
            // }
            // Debug.Log("traffic system count :" +trafficSystem);
            // Debug.Log("TrafficSystem : " + FindObjectOfType<TrafficSystem>());

            // trafficSystem = GameObject.Find("Path1") as TrafficSystem;
            // Truck.GetComponent<VehicleAI>().trafficSystem = trafficSystem;
            
            
            // segment_position = Segment.transform.position;
            // Debug.Log("segment point : "+ segment_position);
        }

        // Truck 인스턴스화 하기
        void Create()
        {   
            // Segment-5일경우
            Truck_position = new Vector3(-43.16f, 1.5f, 35.01f);
            // Truck.GetComponent<VehicleAI>().trafficSystem = GameObject.Find("Path1") as trafficSystem;
            // Truck_trafficSystem = GameObject.Find("Path1");
            Instantiate(Truck, Truck_position, Quaternion.Euler(0, 180, 0));
            
            Debug.Log("Truck traffic system is " + Truck.GetComponent<VehicleAI>().trafficSystem);
        }


    }
}
