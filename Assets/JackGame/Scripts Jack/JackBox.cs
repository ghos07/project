using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class JackBox : MonoBehaviour
{
    // post processing
    public Volume volume;
    public float postProcessingIntensity = 0.0f;

    private VolumeProfile baseProfile;
    public VolumeProfile maxRiskProfile;

    public GameObject crank;
    public GameObject jack;
    public GameObject lid1;
    public GameObject lid2;
    public GameObject player;

    public float crankSpeed = 1.0f;
    public float crankProgress = 0.0f;
    public float baseRisk = 0.01f;
    public float riskIncrease = 0.005f;
    public float risk = 0.0f;

    public float jumpscareCheckCooldown = 1.0f;
    public float jumpscareCheckInterval = 1.0f;

    public float jumpScareBaseWindTime = 0.5f;
    public float jumpScareWindTime => jumpScareBaseWindTime * (1.0f - risk/2);
    public float jumpScareTimer = -1.0f;

    public int maxAnger = 10;
    public int angerDecrement = -1;
    public float angerDecrementInterval = 5.0f;
    public float angerDecrementCooldown = 5.0f;

    public int anger = 0;

    private float blendFactor = 0.0f;

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

    // Update is called once per frame
    void Update()
    {
        blendFactor = Mathf.Lerp(blendFactor, Mathf.Clamp01((risk + (anger/10f)) * postProcessingIntensity), Time.deltaTime * 2);
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

        if (Input.GetKey(KeyCode.Space) || (anger > maxAnger))
        {
            crankProgress += crankSpeed * Time.deltaTime;

            // Add rotation
            // crank.transform.localRotation *= Quaternion.Euler(crankSpeed * 360.0f * Time.deltaTime, 0, 0);

            bool jumpscare = false;

            if (crankProgress >= 1.0f)
            {
                crankProgress = 1.0f;
                jumpscare = true;
            }

            jumpscareCheckCooldown -= Time.deltaTime;
            risk += riskIncrease * Time.deltaTime;

            if (jumpscareCheckCooldown <= 0)
            {
                jumpscareCheckCooldown = jumpscareCheckInterval;

                if (Random.value < risk)
                {
                    jumpscare = true;
                }
            }

            if (jumpscare)
            {
                if (jumpScareTimer < 0)
                {
                    jumpScareTimer = jumpScareWindTime;
                }
            }

            if (jumpScareTimer >= 0)
            {
                lid1.GetComponent<Animator>().SetBool("Bump", true);
                lid2.GetComponent<Animator>().SetBool("Bump", true);

                risk += riskIncrease * Time.deltaTime;
                risk += riskIncrease * Time.deltaTime;

                jumpScareTimer -= Time.deltaTime;

                if (jumpScareTimer < 0)
                {
                    lid1.GetComponent<Animator>().SetBool("Bump", false);
                    lid2.GetComponent<Animator>().SetBool("Bump", false);
                    jack.GetComponent<Animator>().SetTrigger("Play");
                    lid1.GetComponent<Animator>().SetTrigger("Play");
                    lid2.GetComponent<Animator>().SetTrigger("Play");
                    player.GetComponent<Animator>().SetTrigger("PlayJackBoxScare");

                    jumpScareTimer = -1;
                }
            }
            else
            {
                lid1.GetComponent<Animator>().SetBool("Bump", false);
                lid2.GetComponent<Animator>().SetBool("Bump", false);
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            //crankProgress = 0.0f;
            //risk = 0.0f;
            //jumpscareCheckCooldown = jumpscareCheckInterval;
            jumpScareTimer = -1.0f;

            lid1.GetComponent<Animator>().SetBool("Bump", false);
            lid2.GetComponent<Animator>().SetBool("Bump", false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //anger += 1;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            anger += 2;
        }

        angerDecrementCooldown -= Time.deltaTime;

        if (angerDecrementCooldown <= 0)
        {
            angerDecrementCooldown = angerDecrementInterval;
            anger += angerDecrement;

            if (anger < 0)
            {
                anger = 0;
            }
        }

        if (anger > maxAnger)
        {
            risk += riskIncrease * Time.deltaTime * anger / maxAnger;

            if (Random.value < 0.1f * Time.deltaTime)
            {
                if (anger < 13)
                {
                    anger = 13;
                }
            }
        }
    }
}
