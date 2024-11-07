using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowSmooth : MonoBehaviour
{
    public Transform followObject;
    public float smoothSpeed = 5;
    public bool followRotation = false;

    // Start is called before the first frame update
    void Start()
    {
        if (followObject == null)
        {
            followObject = Camera.main.transform;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Smoothly follow
        Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        followObject.position = Vector3.Lerp(followObject.position, targetPosition, smoothSpeed * Time.deltaTime);

        // Rotate
        if (followRotation)
        {
            followObject.rotation = Quaternion.Lerp(followObject.rotation, transform.rotation, smoothSpeed * Time.deltaTime);
        }
    }
}
