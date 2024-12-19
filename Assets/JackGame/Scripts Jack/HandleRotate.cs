using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleRotate : MonoBehaviour
{ 
    public float velocity = 30;
    private float velocityOriginal;
    public float acceleration = 15;


    public AudioSource spinningSound;

    public float soundSpeed = 1;
    public float spinningSoundInterval = 0;

    // Start is called before the first frame update
    void Start()
    {
        velocityOriginal = velocity;
    }

    // Update is called once per frame
    void Update()
    {
        if (JackBox.Instance.isSpinning)
        {
            transform.localRotation *= Quaternion.Euler(new(Time.deltaTime * velocity, 0, 0));
            velocity += Time.deltaTime * acceleration;
        }
        if (JackBox.Instance.isSpinning == false)
        {
            velocity = velocityOriginal;
        }

        if (spinningSound != null)
        {
            if (JackBox.Instance.isSpinning)
            {
                if (spinningSoundInterval <= 0)
                {
                    spinningSoundInterval = 1 / (velocity * soundSpeed);
                    spinningSound.Play();
                }
                spinningSoundInterval -= Time.deltaTime;
            }
        }
    }
}
