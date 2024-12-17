using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class CamPPBlend : MonoBehaviour
{
    public static CamPPBlend Instance;

    public Volume volume;
    public float postProcessingIntensity;
    public VolumeProfile baseProfile;
    public VolumeProfile maxRiskProfile;

    
    private float blendFactor = 0.0f;
    public float targetBlendFactor = 0.0f;

    public bool locked = false;
    public object locker;

    public bool IsMine(object obj)
    {
        return obj == locker;
    }

    public bool Lock(object obj)
    {
        if (locked)
        {
            return false;
        }
        locker = obj;
        locked = true;
        return true;
    }

    public void Unlock(object obj, bool force = false)
    {
        if (IsMine(obj) || force)
        {
            locked = false;
            locker = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Create a new VolumeProfile
        baseProfile = ScriptableObject.CreateInstance<VolumeProfile>();

        // Copy each effect from volume.profile to baseProfile
        foreach (var component in volume.profile.components)
        {
            var copy = Instantiate(component);
            baseProfile.components.Add(copy);
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    public void BlendPP()
    {
        blendFactor = Mathf.Lerp(blendFactor, targetBlendFactor, Time.deltaTime);
        baseProfile.TryGet<Bloom>(out var baseBloom);
        maxRiskProfile.TryGet<Bloom>(out var maxRiskBloom);
        volume.profile.TryGet<Bloom>(out var currentBloom);
        currentBloom.intensity.value = Mathf.Lerp(baseBloom.intensity.value, maxRiskBloom.intensity.value, blendFactor);

        baseProfile.TryGet<Vignette>(out var baseVignette);
        maxRiskProfile.TryGet<Vignette>(out var maxRiskVignette);
        volume.profile.TryGet<Vignette>(out var currentVignette);
        currentVignette.intensity.value = Mathf.Lerp(baseVignette.intensity.value, maxRiskVignette.intensity.value, blendFactor);
        currentVignette.color.value = Color.Lerp(baseVignette.color.value, maxRiskVignette.color.value, blendFactor);
        currentVignette.smoothness.value = Mathf.Lerp(baseVignette.smoothness.value, maxRiskVignette.smoothness.value, blendFactor);

        baseProfile.TryGet<ChromaticAberration>(out var baseChromaticAberration);
        maxRiskProfile.TryGet<ChromaticAberration>(out var maxRiskChromaticAberration);
        volume.profile.TryGet<ChromaticAberration>(out var currentChromaticAberration);
        currentChromaticAberration.intensity.value = Mathf.Lerp(baseChromaticAberration.intensity.value, maxRiskChromaticAberration.intensity.value, blendFactor);

        baseProfile.TryGet<LensDistortion>(out var baseLensDistortion);
        maxRiskProfile.TryGet<LensDistortion>(out var maxRiskLensDistortion);
        volume.profile.TryGet<LensDistortion>(out var currentLensDistortion);
        currentLensDistortion.intensity.value = Mathf.Lerp(baseLensDistortion.intensity.value, maxRiskLensDistortion.intensity.value, blendFactor);

        baseProfile.TryGet<FilmGrain>(out var baseFilmGrain);
        maxRiskProfile.TryGet<FilmGrain>(out var maxRiskFilmGrain);
        volume.profile.TryGet<FilmGrain>(out var currentFilmGrain);
        currentFilmGrain.intensity.value = Mathf.Lerp(baseFilmGrain.intensity.value, maxRiskFilmGrain.intensity.value, blendFactor);

        baseProfile.TryGet<MotionBlur>(out var baseMotionBlur);
        maxRiskProfile.TryGet<MotionBlur>(out var maxRiskMotionBlur);
        volume.profile.TryGet<MotionBlur>(out var currentMotionBlur);
        currentMotionBlur.intensity.value = Mathf.Lerp(baseMotionBlur.intensity.value, maxRiskMotionBlur.intensity.value, blendFactor);
    }

    // Update is called once per frame
    void Update()
    {
        BlendPP();
    }
}
