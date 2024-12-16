using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour
{
    public Vector3 defaultPosition;
    public Quaternion defaultRotation;

    public Transform defaultTransform;

    public Transform camParent;

    public float timeToReachTarget = 1.0f;
    
    public bool worldSpace = true;

    public void GoToDefaultPosition()
    {
        if (defaultTransform != null)
        {
            defaultPosition = defaultTransform.position;
            defaultRotation = defaultTransform.rotation;
        }

        if (worldSpace)
        {
            JackCameraController.instance.SetTarget(defaultPosition, defaultRotation, timeToReachTarget);
        }
        else
        {
            // Convert to local space using the camera's parent
            Vector3 localPosition = JackCameraController.instance.transform.parent.InverseTransformPoint(defaultPosition);
            Quaternion localRotation = Quaternion.Inverse(JackCameraController.instance.transform.parent.rotation) * defaultRotation;
            JackCameraController.instance.SetTarget(localPosition, localRotation, timeToReachTarget);
        }

        if (camParent != null)
        {
            JackCameraController.instance.transform.SetParent(camParent);
        }
    }
}
