using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientLightsController : MonoBehaviour
{

    private static AmbientLightsController instance;
    public static AmbientLightsController Instance
    {
        get
        {
            if (instance == null)
            {
                //First try to find the AmbientLightsController in the scene
                instance = FindObjectOfType<AmbientLightsController>();
                if (instance != null)
                {
                    return instance;
                }
                // Create a new GameObject with the AmbientLightsController script attached
                GameObject go = new GameObject("AmbientLightsController");
                instance = go.AddComponent<AmbientLightsController>();
            }
            return instance;
        }
    }

    public List<GameAmbientLight> gameAmbientLights = new List<GameAmbientLight>();
    public float intensity = 1.0f;

    public void SetIntensity(float intensity, float timeToChange = 1f)
    {
        this.intensity = intensity;
        foreach (var light in gameAmbientLights)
        {
            light.SetIntensity(intensity, timeToChange);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
