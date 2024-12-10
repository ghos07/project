using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackBox : MonoBehaviour
{
    public GameObject crank;
    public GameObject jack;
    public GameObject lid1;
    public GameObject lid2;
    public GameObject player;

    public float crankSpeed = 1.0f;
    public float crankProgress = 0.0f;
    public float baseRisk = 0.01f;
    public float riskIncrease = 0.05f;
    public float risk = 0.01f;

    public float jumpscareCheckCooldown = 1.0f;
    public float jumpscareCheckInterval = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            crankProgress += crankSpeed * Time.deltaTime;

            // Add rotation
            crank.transform.localRotation = Quaternion.Euler(crankProgress * 360.0f, 0, 0);

            bool jumpscare = false;

            if (crankProgress >= 1.0f)
            {
                crankProgress = 1.0f;
                jumpscare = true;
            }

            if (jumpscareCheckCooldown <= 0)
            {
                jumpscareCheckCooldown = jumpscareCheckInterval;

                if (Random.value < risk)
                {
                    jumpscare = true;
                }
            }
            else
            {
                jumpscareCheckCooldown -= Time.deltaTime;
            }

            if (jumpscare)
            {
                jack.GetComponent<Animator>().SetTrigger("Play");
                lid1.GetComponent<Animator>().SetTrigger("Play");
                lid2.GetComponent<Animator>().SetTrigger("Play");
                player.GetComponent<Animator>().SetTrigger("PlayJackBoxScare");
            }
        }
    }
}
