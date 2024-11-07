using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHackFix : MonoBehaviour
{
    public GameObject playerModel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerModel.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
}
