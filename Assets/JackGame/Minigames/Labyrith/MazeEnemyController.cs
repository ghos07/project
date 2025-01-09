using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeEnemyController : MonoBehaviour
{
    [SerializeField]
    private MovementBehaviour mb;
    [SerializeField]
    private FieldofView fov;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (fov.canSeePlayer)
        {
            Vector3 moveDirection = (fov.playerRef.transform.position - transform.position).normalized;
            mb.Move(new (moveDirection.x, moveDirection.z));
        }
    }
}
