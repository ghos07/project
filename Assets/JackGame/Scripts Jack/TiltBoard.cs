using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TiltBoard : MonoBehaviour
{

    [SerializeField] private float tiltSpeed = 2;
    [SerializeField] private float maxTilt = 2;

    [SerializeField] private PlayerInputActions InputActions;

    private Vector3 baseRotation = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        InputActions = new();
        InputActions.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = InputActions.PlayerMovement.Camera.ReadValue<Vector2>();

        Vector3 rotation = transform.eulerAngles;
        transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, new(rotation.x + mousePosition.y, 0, rotation.z + -mousePosition.x), Time.deltaTime);
    }
}
