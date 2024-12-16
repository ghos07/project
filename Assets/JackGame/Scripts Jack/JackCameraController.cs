using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackCameraController : MonoBehaviour
{
    /// <summary>
    /// If an instance does not exist, it will create one. The created instance WILL BE DISABLED.
    /// </summary>
    public static JackCameraController instance
    {
        get
        {
            if (Camera.main == null)
            {
                return null;
            }
            else if (Camera.main.GetComponent<JackCameraController>() == null)
            {
                JackCameraController jcc = Camera.main.gameObject.AddComponent<JackCameraController>();
                jcc.enabled = false;
                return jcc;
            }
            else
            {
                return Camera.main.GetComponent<JackCameraController>();
            }
        }
    }

    Vector3 targetPosition = Vector3.zero;
    Quaternion targetRotation = Quaternion.identity;
    float timeToReachTarget = 1.0f;
    float timeElapsed = 2f;

    public void SetTarget(Vector3 targetPosition, Quaternion targetRotation, float timeToReachTarget = 1.0f)
    {
        this.targetPosition = targetPosition;
        this.targetRotation = targetRotation;
        this.timeToReachTarget = timeToReachTarget;
        timeElapsed = 0.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= timeToReachTarget)
        {
            return;
        }

        float t = timeElapsed / timeToReachTarget;

        transform.position = Vector3.Lerp(transform.position, targetPosition, t);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);
    }
}
