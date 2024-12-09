using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMouseSway : MonoBehaviour
{
    // start rotation
    private Vector3 startRot;

    [SerializeField] private float swayAmount = 10;
    [SerializeField] private float swaySpeed = 10;


    // Start is called before the first frame update
    void Start()
    {
        
        startRot = transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        // get mouse position
        Vector3 mousePos = Input.mousePosition;
        // Clamp mouse position to screen
        mousePos.x = Mathf.Clamp(mousePos.x, 0, Screen.width * 0.9f);
        mousePos.y = Mathf.Clamp(mousePos.y, 0, Screen.height * 0.9f);

        // get screen center
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        // calculate difference
        Vector3 diff = mousePos - screenCenter;

        // normalize
        diff = new Vector3(diff.x / Screen.width, diff.y / Screen.height, 0);
        diff *= swayAmount;

        // adjust localEulerAngles for looping effect
        Vector3 fixedEulerAngles = transform.localEulerAngles;
        if (fixedEulerAngles.x > 180)
        {
            fixedEulerAngles.x -= 360;
        }
        if (fixedEulerAngles.y > 180)
        {
            fixedEulerAngles.y -= 360;
        }
        if (fixedEulerAngles.z > 180)
        {
            fixedEulerAngles.z -= 360;
        }

        // apply rotation
        transform.localEulerAngles = Vector3.Lerp(fixedEulerAngles, new Vector3(-diff.y * 10, diff.x * 10, 0), Time.deltaTime * swaySpeed);

        // Add slight position sway
        transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(diff.x, diff.y, 0), Time.deltaTime * swaySpeed);

        print(diff);

    }
}
