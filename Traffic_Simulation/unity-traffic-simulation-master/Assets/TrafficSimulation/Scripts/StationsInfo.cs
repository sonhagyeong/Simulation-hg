using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationsInfo : MonoBehaviour
{

    // 현재 작업 중인 트럭 개수
    public int stationStatus;
    // 작업 가능한 Max 트럭 개수
    public int stationCapacity;

    
    // 작업이 완료된 트럭 중 왼쪽으로 가는 트럭 리스트
    public int finishedVehicle_toLeft_Count;
    
    // 작업이 완료된 트럭 중 오른쪽으로 가는 트럭 리스트
    public int finishedVehicle_toRight_Count;

    // 작업 대기 중인 트럭 리스트
    public List<GameObject> queueList;

    void Start()
    {
        stationStatus = 0;
        stationCapacity = 2;
        finishedVehicle_toLeft_Count = 0;
        finishedVehicle_toRight_Count = 0;
    
        queueList = new List<GameObject>();
    }

    
}