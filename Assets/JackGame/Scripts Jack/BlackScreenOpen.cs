using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BlackScreenOpen : MonoBehaviour
{
    public Volume volume;
    public VolumeProfile wakeupProfile;
    public VolumeProfile maxRiskProfile;

    private VolumeProfile baseProfile;

    public float secondsToFade = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        baseProfile = CamPPBlend.Instance.baseProfile;
        maxRiskProfile = CamPPBlend.Instance.altProfile;
        CamPPBlend.Instance.baseProfile = wakeupProfile;

        CamPPBlend.Instance.altProfile = baseProfile;

        CamPPBlend.Instance.targetBlendFactor = 1.0f;

        CamPPBlend.Instance.Lock(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeSinceLevelLoad > secondsToFade)
        {
            CamPPBlend.Instance.baseProfile = baseProfile;
            CamPPBlend.Instance.altProfile = maxRiskProfile;
            CamPPBlend.Instance.targetBlendFactor = 0.0f;
            CamPPBlend.Instance.blendFactor = 0.0f;

            CamPPBlend.Instance.Unlock(this);
            Destroy(this);

        }

    }
}
