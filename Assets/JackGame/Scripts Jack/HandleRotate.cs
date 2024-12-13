using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleRotate : MonoBehaviour
{ 
    public float velocity = 30;
    private float velocityOriginal;
    public float acceleration = 15;
    // Start is called before the first frame update
    void Start()
    {
        velocityOriginal = velocity;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            transform.localRotation *= Quaternion.Euler(new(Time.deltaTime * velocity, 0, 0));
            velocity += Time.deltaTime * acceleration;
        }
        if (Input.GetKeyUp(KeyCode.Space) == true)
        {
            velocity = velocityOriginal;
        }
    }
}
