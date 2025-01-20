using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class footsteps : MonoBehaviour
{
    public AudioClip[] walkingSounds;
    public AudioClip[] sprintingSounds;
    public AudioClip[] crouchingSounds;
    public AudioClip[] stable_hearbeat;
    public Timer walkingTimer;
    public Timer sprintingTimer;
    public Timer crouchingTimer;
    public Timer heartBeats;
    public int heartBeat = 0;
    void Update()
    {
        if (heartBeats)
        {
            AmbientSoundManager.PlaySound(stable_hearbeat[heartBeat], 0.8f);
            if (heartBeat >= 0 && heartBeat != 8)
            {
                heartBeat += 1;
            }
            if (heartBeat == 8)
            {
                heartBeat = 0;
            }
            heartBeats.Trigger();
        }

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
                    AmbientSoundManager.PlaySound(crouchingSounds[Random.Range(0, crouchingSounds.Length - 1)], 0.2f);
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
        walkingTimer = new Timer(0.5f);
        sprintingTimer = new Timer(0.3f);
        crouchingTimer = new Timer(1.2f);
        heartBeats = new Timer(1.8f);

    }
}



