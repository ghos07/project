using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class JackBox : MonoBehaviour
{
    // How many of these am I gonna add DX
    public static JackBox Instance { get; private set; }



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
    public float jumpScareWindTime => jumpScareBaseWindTime / (risk / 2 + 1);
    public float jumpScareTimer = -1.0f;

    public int maxAnger = 10;
    public int realMaxAnger = 15;
    public int angerDecrement = -1;
    public float angerDecrementInterval = 5.0f;
    public float angerDecrementCooldown = 5.0f;

    public int anger = 0;
    public float resentment = 0;
    public float maxResentment = 3;
    public float resentmentDivide = 2;
    public float lastResentment = 0;

    public AudioClip resentmentSound;
    public AudioClip maxResentmentSound;
    public AudioClip angerThresholdSound;
    public AudioClip angerIncreaseSound;



    public bool isSpinning = false;
    public bool lastIsSpinning = false;

    public void OnReset()
    {
        crankProgress = 0.0f;
        risk = 0.0f;
        jumpscareCheckCooldown = jumpscareCheckInterval;
        jumpScareTimer = -1.0f;
        anger = 0;
        resentment /= resentmentDivide;

        CamPPBlend.Instance.Unlock(this);

        MinigameManager.ResetMinigames(new() { MinigameManager.GetMinigame(MinigameNames.JackBox) });

        MinigameManager.GetMinigame(MinigameNames.JackBox).SetActive(false);
    }

    public void OnActivate()
    {
        crankProgress = 0.0f;
        risk = 0.0f;
        jumpscareCheckCooldown = jumpscareCheckInterval;
        jumpScareTimer = -1.0f;
        anger = 0;

        CamPPBlend.Instance.Lock(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        Instance = this;
    }
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnReset();
        }

        // uhhh it works :/
        if (CamPPBlend.Instance.IsMine(this))
            CamPPBlend.Instance.targetBlendFactor = Mathf.Clamp01(risk + (anger / 10f));

        isSpinning = false;
        if (Input.GetKey(KeyCode.Space) || (anger > maxAnger))
        {
            crankProgress += crankSpeed * Time.deltaTime;
            isSpinning = true;

            // Add rotation
            // crank.transform.localRotation *= Quaternion.Euler(crankSpeed * 360.0f * Time.deltaTime, 0, 0);

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

        if (lastIsSpinning && !isSpinning)
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
            //anger += 1;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            anger += 2;
            // TODO: Play sound effect on resentment full 1 increase
            resentment += 0.08f;
        }

        angerDecrementCooldown -= Time.deltaTime;

        if (angerDecrementCooldown <= 0)
        {
            angerDecrementCooldown = angerDecrementInterval;
            anger += angerDecrement;

            if (anger < 0)
            {
                anger = 0;
            }
        }

        if (anger > maxAnger)
        {
            risk += riskIncrease * Time.deltaTime * anger / maxAnger * 10;

            if (Random.value < 0.02f * Time.deltaTime)
            {
                if (anger < 13)
                {
                    anger = 13;
                }
            }
        }

        if (anger > realMaxAnger)
        {
            anger = realMaxAnger;
        }

        if (resentment > maxResentment)
        {
            if (Random.value < 0.3f * Time.deltaTime)
            {
                anger += 5;
            }
        }

        lastIsSpinning = isSpinning;

        if (lastResentment < 1 && resentment >= 1)
        {

        }

        lastResentment = resentment;
    }
}
