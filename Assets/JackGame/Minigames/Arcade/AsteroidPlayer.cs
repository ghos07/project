using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidPlayer : MonoBehaviour
{
    public static AsteroidPlayer instance;

    public GameObject player;
    public GameObject bullet;
    public float speed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            player.transform.position += player.transform.forward * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            player.transform.position -= player.transform.forward * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            player.transform.rotation *= Quaternion.Euler(0, -1, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            player.transform.rotation *= Quaternion.Euler(0, 1, 0);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject newBullet = Instantiate(bullet, player.transform.position, player.transform.rotation);
        }
    }
}
