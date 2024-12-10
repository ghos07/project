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
    public float riskIncrease = 0.005f;
    public float risk = 0.0f;

    public float jumpscareCheckCooldown = 1.0f;
    public float jumpscareCheckInterval = 1.0f;

    public float jumpScareBaseWindTime = 0.5f;
    public float jumpScareWindTime => jumpScareBaseWindTime * (1.0f - risk/2);
    public float jumpScareTimer = -1.0f;

    public float angerDecrement = -1.0f;
    public float angerDecrementInterval = 5.0f;

    public int anger = 0;

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
            crank.transform.localRotation = Quaternion.Euler(crankProgress / crankSpeed * 360.0f, 0, 0);

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

        }

        if (Input.GetKeyUp(KeyCode.Space))
        {

        }
    }
}
