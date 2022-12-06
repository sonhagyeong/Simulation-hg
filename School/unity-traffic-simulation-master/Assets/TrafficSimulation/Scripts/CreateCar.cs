using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TrafficSimulation{

    public class CreateCar : MonoBehaviour
    {   
        // 뽑을 Segment
        public GameObject segment;
        // 뽑을 트럭
        public GameObject Truck;
        public string selectedTruck;

        // Segment별 Position
        public Vector3 path_Position;
        public Quaternion path_Rotation;

        public string selectedPath;
        public static string nameTag;

        List<string> pathName = new List<string>();
        List<string> truckName = new List<string>();

        Dictionary<string, string> tagsDic = new Dictionary<string, string>();
        Dictionary<string, Quaternion> rotationsDic = new Dictionary<string, Quaternion>();

        public int SegmentCount = 6;
        public int TruckPrefabCount = 4;
        public int carCount = 0;
        // 생성할 총 트럭 개수
        public int carEndCount;
        public float waitSecond = 3f;
        // private bool _isDone = false;

        // Start is called before the first frame update
        void Start()
        {   
            AddPathName();
            AddTruckName();
            AddTags();
            AddRotation();

            // Test할때
            // 출발지 고정할 때
            // carEndCount 6까지 Test 
            TestCreateTruck();

            //랜덤으로 트럭 생기게
            carEndCount = 1;
            StartCoroutine(createVehicle());
        }

        IEnumerator createVehicle()
        {
            // AddPathName();
            // AddTruckName();
            // AddTags();
            // AddRotation();
            // CreateProcess();
            while (true){
                // yield return new WaitForSecondsRealtime( waitSecond );
                carCount += 1;
                Debug.Log("carCount : " + carCount);
                CreateProcess();
                if(carCount == carEndCount)
                {   
                    Debug.Log("carCount : " + carCount);
                    Debug.Log("carEndCount : " + carEndCount);
                    Debug.Log("carCount and carEndCount are same");
                    break;
                }

                yield return new WaitForSecondsRealtime(waitSecond);
            }
        }

        // private void Update()
        // {
        //     if(GameObject.Find("Trucks").transform.childCount == 0)
        //     {
        //         UnityEditor.EditorApplication.isPlaying = false;
        //     }
        // }

        public void CreateProcess()
        {
            GetPathName();
            GetPathPosition();
            GetPathRotation();
            GetTruck();
            GetTag();
            Create();
        }

        // 출발지
        public void AddPathName()
        {
            pathName.Add("Segment-0");
            pathName.Add("Segment-1");
            // Segment 2랑 7연결
            pathName.Add("Segment-7");
            pathName.Add("Segment-3");
            pathName.Add("Segment-4");
            pathName.Add("Segment-5");
            pathName.Add("Segment-6");
        }
        
        // 목적지
        public void AddTags()
        {
            //path에 tag는 string 값으로 추가
            tagsDic.Add(pathName[0], "place0");
            tagsDic.Add(pathName[1], "place2");
            tagsDic.Add(pathName[2], "place0");
            tagsDic.Add(pathName[3], "place4");
            tagsDic.Add(pathName[4], "place1");
            tagsDic.Add(pathName[5], "place3");
            tagsDic.Add(pathName[6], "place4");
        }

        // 회전값
        public void AddRotation()
        {
            //path종류는 String, 좌표는 V3 값으로 추가
            rotationsDic.Add(pathName[0], Quaternion.Euler(0, 0, 0));
            rotationsDic.Add(pathName[1], Quaternion.Euler(0, 180, 0));
            rotationsDic.Add(pathName[2], Quaternion.Euler(0, 270, 0));
            rotationsDic.Add(pathName[3], Quaternion.Euler(0, 180, 0));
            rotationsDic.Add(pathName[4], Quaternion.Euler(0, 180, 0));
            rotationsDic.Add(pathName[5], Quaternion.Euler(0, 0, 0));
            rotationsDic.Add(pathName[6], Quaternion.Euler(0, 180, 0));

        }

        public void AddTruckName()
        {
            truckName.Add("Truck1");
            truckName.Add("Truck2");
            truckName.Add("Truck3");
            truckName.Add("Truck4");
        }

        public void GetPathName()
        {
            int pathRandomNum = Random.Range(0, SegmentCount + 1);
            selectedPath = pathName[pathRandomNum];
            // Debug.Log("selected path : " + selectedPath);
        }

        // 뽑은 Segment position 얻기
        public void GetPathPosition()
        {   
            path_Position = GameObject.Find(selectedPath).transform.position;
            // Debug.Log("path_Position : "+ path_Position);
        }

        public void GetPathRotation()
        {
            path_Rotation = rotationsDic[selectedPath];
        }

        public void GetTag()
        {
            nameTag = tagsDic[selectedPath];
        }

        public void GetTruck()
        {   
            int truckRandomNum = Random.Range(0, TruckPrefabCount + 1);
            selectedTruck = truckName[truckRandomNum];
        }

        // Truck 인스턴스화 하기
        public void Create()
        {   
            Truck = Resources.Load<GameObject>(selectedTruck);
            Truck.GetComponent<VehicleAI>().trafficSystem = FindObjectOfType<TrafficSystem>();

            // 자동화 할때
            Truck.GetComponent<SetNameTag>().truckNameTag = nameTag;
            Truck.GetComponent<SetNameTag>().segmentNameTag = selectedPath;

            Debug.Log("selectedPath : "+ selectedPath);
            Debug.Log("nameTag : " + nameTag);
    
            Instantiate(Truck, path_Position, path_Rotation);
            // newTruck.transform.SetParent(GameObject.Find("Trucks").transform);

        }

        public void TestCreateTruck()
        {
            //Test할 때

            GameObject testingTruck = Resources.Load<GameObject>("Truck4");
            testingTruck.GetComponent<VehicleAI>().trafficSystem = FindObjectOfType<TrafficSystem>();

            // 출발지, 목적지 설정 이유 : result.csv에 저장하려고
            
            // 출발지
            testingTruck.GetComponent<SetNameTag>().segmentNameTag = "Segment-6";
            
            // 목적지
            testingTruck.GetComponent<SetNameTag>().truckNameTag = "place4";
            
            //출발지에서 트럭 생성 => 출발지, 회전값 수정

            // Instantiate(testingTruck, GameObject.Find("Segment-0").transform.position, Quaternion.Euler(0, 0, 0));
            // Instantiate(testingTruck, GameObject.Find("Segment-4").transform.position, Quaternion.Euler(0, 180, 0));
            // Instantiate(testingTruck, GameObject.Find("Segment-7").transform.position, Quaternion.Euler(0, 270, 0));
            // Instantiate(testingTruck, GameObject.Find("Segment-3").transform.position, Quaternion.Euler(0, 180, 0));
            // Instantiate(testingTruck, GameObject.Find("Segment-4").transform.position, Quaternion.Euler(0, 180, 0));
            // Instantiate(testingTruck, GameObject.Find("Segment-5").transform.position, Quaternion.Euler(0, 0, 0));
            Instantiate(testingTruck, GameObject.Find("Segment-6").transform.position, Quaternion.Euler(0, 180, 0));
        }
    }
}
