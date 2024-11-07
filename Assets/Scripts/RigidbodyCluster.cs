using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodyCluster : MonoBehaviour
{
    public Rigidbody[] rigidbodies => GetComponentsInChildren<Rigidbody>();
    public Transform parent = null;

    void Start()
    {
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.transform.SetParent(parent);
        }
    }
}
