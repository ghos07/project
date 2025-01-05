using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardKey : MonoBehaviour
{
    public Material pressedMaterial;

    [HideInInspector] public Material unpressedMaterial;

    public AudioClip keySound;

    [HideInInspector]
    public float pressTime = 0.7f;
    public float volume = 1f;

    public float activeOnDifficulty = 0f;

    public bool isPressed = false;
    public float pressedTime = 0f;

    public void Press(float pressTime)
    {
        if (!isPressed)
        {
            this.pressTime = pressTime;
            isPressed = true;
            pressedTime = 0f;
            GetComponent<MeshRenderer>().material = pressedMaterial;
            try { AudioSource.PlayClipAtPoint(keySound, GetComponent<Collider>().bounds.center, volume); } catch { }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        unpressedMaterial = GetComponent<MeshRenderer>().material;

        FindObjectOfType<KeyboardGame>().keys.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPressed)
        {
            pressedTime += Time.deltaTime;
            if (pressedTime >= pressTime)
            {
                isPressed = false;
                GetComponent<MeshRenderer>().material = unpressedMaterial;
            }
        }
    }
}
