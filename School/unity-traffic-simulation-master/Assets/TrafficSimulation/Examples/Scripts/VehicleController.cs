using UnityEngine;
using TrafficSimulation;

public class VehicleController : MonoBehaviour
{

    WheelDrive wheelDrive;

    void Start()
    {
        wheelDrive = this.GetComponent<WheelDrive>();
    }

    void Update()
    {   
        // 위 아래
        float acc = Input.GetAxis("Vertical");
        // 좌우
        float steering = Input.GetAxis("Horizontal");
        // Keyboard Space 불러오기
        float brake = Input.GetKey(KeyCode.Space) ? 1 : 0;

        wheelDrive.Move(acc, steering, brake);
    }
}