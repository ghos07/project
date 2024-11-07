using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface CameraController
{
    public Vector2 RotateVector2TowardLook(Vector2 vector);
    public Vector3 RotateVector3TowardLook(Vector3 vector);
}
