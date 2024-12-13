using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float intensity = 1f;
    private float appliedIntensity = 0f;

    public ModifierManager modifierManager;

    // Start is called before the first frame update
    void Start()
    {
        modifierManager = ModifierManager.GetModifierManager(gameObject);
        modifierManager.AddModifier(Modifier.CameraShake, intensity);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Random.Range(-appliedIntensity, appliedIntensity), Random.Range(-appliedIntensity, appliedIntensity), Random.Range(-appliedIntensity, appliedIntensity) / 2);

        if (modifierManager != null)
        {
            appliedIntensity = modifierManager.GetValue(Modifier.CameraShake);
        }
    }
}
