using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TrafficSimulation{

    public class CreateCar : MonoBehaviour
    {   
        public float Y_Position = 1.5f;
        // 뽑을 Segment
        public GameObject segment;
        // 뽑을 트럭
        public GameObject Truck;
        // Segment별 Position
        public Vector3 path_Position;
        public Quaternion path_Rotation;

        string selectedPath;
        string nameTag;

        List<string> pathName = new List<string>();

        Dictionary<string, string> tagsDic = new Dictionary<string, string>();
        // Dictionary<string, Vector3> pathPositionsDic = new Dictionary<string, Vector3>();
        Dictionary<string, Quaternion> rotationsDic = new Dictionary<string, Quaternion>();
        
        
        
        
        // Start is called before the first frame update
        void Start()
        {   
            AddPathName();
            AddTags();
            AddRotation();
            // AddPosition();
            GetPath();
            GetPathPosition();
            GetPathRotation();
            // Create();
        }

        void AddPathName()
        {
            pathName.Add("path_0");
            pathName.Add("path_1");
            pathName.Add("path_2");
            pathName.Add("path_3");
        }
        
        void AddTags()
        {
            //path에 tag는 string 값으로 추가
            tagsDic.Add(pathName[0], "place1");
            tagsDic.Add(pathName[1], "place2");
            tagsDic.Add(pathName[2], "place3");
            tagsDic.Add(pathName[3], "place4");
        }
    
        void AddRotation()
        {
            //path종류는 String, 좌표는 V3 값으로 추가
            rotationsDic.Add(pathName[0], Quaternion.Euler(0, 0, 0));
            rotationsDic.Add(pathName[1], Quaternion.Euler(0, 180, 0));
            rotationsDic.Add(pathName[2], Quaternion.Euler(0, 180, 90));
            rotationsDic.Add(pathName[3], Quaternion.Euler(0, 0, 0));
        }

        // void AddPosition()
        // {
        //     //path종류는 String, 좌표는 V3 값으로 추가
        //     pathPositionsDic.Add(pathName[0], GameObject.Find(pathName[0]).transform.position);
        //     pathPositionsDic.Add(pathName[1], GameObject.Find(pathName[1]).transform.position);
        //     pathPositionsDic.Add(pathName[2], GameObject.Find(pathName[2]).transform.position);
        //     pathPositionsDic.Add(pathName[3], GameObject.Find(pathName[3]).transform.position);
        // }

        void GetPath()
        {
            int randomNum = Random.Range(0, 4);
            selectedPath = pathName[randomNum];
            // Debug.Log("selected path : " + selectedPath);
        }

        // 뽑은 Segment position 얻기
        void GetPathPosition()
        {   
            path_Position = GameObject.Find(selectedPath).transform.position;
            // Debug.Log("path_Position : "+ path_Position);
        }

        void GetPathRotation()
        {
            path_Rotation = rotationsDic[selectedPath];
        }

        void GetTag()
        {
            nameTag = tagsDic[selectedPath];
        }
        // Truck 인스턴스화 하기
        void Create()
        {   
            Truck.GetComponent<VehicleAI>().trafficSystem = FindObjectOfType<TrafficSystem>();
            Instantiate(Truck, path_Position, path_Rotation);
            
            Debug.Log("Truck traffic system is " + Truck.GetComponent<VehicleAI>().trafficSystem);
        }


    }
}
