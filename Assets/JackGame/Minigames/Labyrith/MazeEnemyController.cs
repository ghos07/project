using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MazeEnemyController : MonoBehaviour
{
    [SerializeField]
    private float goatedDistance = 10.8f; 

    [SerializeField]
    private FieldofView fov;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {   
        if (Vector3.Distance(fov.playerRef.transform.position, transform.position) < goatedDistance)
        {
            Destroy(fov.playerRef.gameObject);
        }
        if (fov.canSeePlayer)
        {
            Vector3 moveDirection = (fov.playerRef.transform.position - transform.position).normalized;
        }
    }
}
