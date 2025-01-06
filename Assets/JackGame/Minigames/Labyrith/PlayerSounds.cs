using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class footsteps : MonoBehaviour
{
    public AudioClip[] walkingSounds;
    public AudioClip[] sprintingSounds;
    public AudioClip[] crouchingSounds;
    public Timer walkingTimer;
    public Timer sprintingTimer;
    public Timer crouchingTimer;
    void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (sprintingTimer)
                {
                    AmbientSoundManager.PlaySound(sprintingSounds[Random.Range(0, sprintingSounds.Length - 1)], 1);
                    sprintingTimer.Trigger();
                }
            }
            else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C))
            {
                if (crouchingTimer)
                {
                    AmbientSoundManager.PlaySound(crouchingSounds[Random.Range(0, crouchingSounds.Length - 1)], 1);
                    crouchingTimer.Trigger();
                }
            }
            else
            {
                if (walkingTimer)
                {
                    AmbientSoundManager.PlaySound(walkingSounds[Random.Range(0, walkingSounds.Length - 1)], 1);
                    walkingTimer.Trigger();
                }
            }
        }
    }
    private void Start()
    {
        walkingTimer = new Timer(0.4f);
        sprintingTimer = new Timer(0.3f);
        crouchingTimer = new Timer(0.5f);

    }
}



