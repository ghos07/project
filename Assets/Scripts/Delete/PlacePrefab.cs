using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacePrefab : MonoBehaviour
{
    public GameObject prefab;
    public float spawnDistance = 2;
    public float spawnForce = 10;
    public string key = "e";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            GameObject go = Instantiate(prefab, transform.parent);
            go.transform.position = Camera.main.transform.position + Camera.main.transform.forward * spawnDistance;

            if (go.TryGetComponent(out Rigidbody rb))
            {
                rb.AddForce(Camera.main.transform.forward * spawnForce, ForceMode.VelocityChange);
            }
        }
    }
}
