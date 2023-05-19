using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;


public class Destination : MonoBehaviour
{   
    private Stopwatch watch;

    // Start is called before the first frame update
    void Start()
    {
        watch = new Stopwatch();
        watch.Start();
    }

    private void OnTriggerEnter(Collider other)
    {   
        if(other.name == "Destination")
        {
            watch.Stop();
            float elapsedSeconds = watch.ElapsedMilliseconds / 1000f;
            UnityEngine.Debug.Log("Time elapsed: " + elapsedSeconds + " s");
        }
    }
}
