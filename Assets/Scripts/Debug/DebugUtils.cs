using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUtils : MonoBehaviour
{

    public bool active = false;

    public bool forceTimeScale = false;
    public float timeScale = 1;

    public bool forceFixedDeltaTime = false;
    public float fixedDeltaTime = 0.02f;
    
    public bool forceFrameRate = false;
    public int targetFrameRate = 60;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            if (forceTimeScale)
            {
                Time.timeScale = timeScale;
            }

            if (forceFixedDeltaTime)
            {
                Time.fixedDeltaTime = fixedDeltaTime;
            }

            if (forceFrameRate)
            {
                Application.targetFrameRate = targetFrameRate;
            }
        }
    }
}
