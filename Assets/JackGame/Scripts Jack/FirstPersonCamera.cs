using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public float sensitivity = 100f;
    public Transform playerBody;

    public float interactionDistance = 2f;

    public bool enableInteraction = true;

    public void SetActive(bool active)
    {
        this.enabled = active;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        playerBody.Rotate(Vector3.up * mouseX);

        // Prevent the player from looking up or down more than 90 degrees
        float xRotation = transform.localRotation.eulerAngles.x - mouseY;
        xRotation = Mathf.Clamp(xRotation, 0, 90);
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        if (enableInteraction)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, interactionDistance))
                {
                    if (hit.collider.gameObject.TryGetComponent(out Interactible interactable))
                    {
                        interactable.Interact();
                    }
                }
            }
        }
    }
}
