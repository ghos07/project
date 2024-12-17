using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAmbientLight : MonoBehaviour
{
    public float baseIntensity = 1.0f;
    public float lastIntensity = 1.0f;
    public float intensity = 1.0f;
    public float timeToChange = 1.0f;
    public float timeToChangeModifier = 1.0f;
    public float timeElapsed = 0.0f;

    public void SetIntensity(float intensity, float timeToChange = 1f)
    {
        lastIntensity = this.intensity;
        this.intensity = intensity;
        this.timeToChange = timeToChange;
        timeElapsed = 0.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        AmbientLightsController.Instance.gameAmbientLights.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (timeElapsed >= timeToChange)
        {
            return;
        }

        timeElapsed += Time.deltaTime;
        if (timeElapsed < timeToChange)
        {
            float t = timeElapsed / (timeToChange * timeToChangeModifier);
            float newIntensity = Mathf.Lerp(baseIntensity * lastIntensity, baseIntensity * intensity, t);
            GetComponent<Light>().intensity = newIntensity;
        }
    }
}
