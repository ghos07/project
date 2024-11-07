using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTPCameraController : MonoBehaviour, CameraController
{
    [SerializeField] private Camera cam;
    [SerializeField] private float mouseSensitivityX = 10f;
    [SerializeField] private float mouseSensitivityY = 10f;
    [SerializeField] private Transform body;
    [SerializeField] private Transform neck;
    [SerializeField] private Transform head;
    [SerializeField] private Transform camAxisX;
    [SerializeField] private Transform camAxisY;
    [SerializeField] private ThirdPersonPlayerController playerController;
    [SerializeField] private bool forceBodyFollowLook = false;
    [SerializeField] private bool restrictHeadRotation = true;
    [SerializeField] private float headRotationLimit = 70f;
    [SerializeField] private bool freeMovement = true;

    private float xRotation = 0f;
    private float yRotation = 0f;
    private float currentBodyYRotation;

    public float XRotation => xRotation;
    public float YRotation => yRotation;

    private void Awake()
    {
        if (cam == null)
            cam = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;

        if (playerController == null)
            playerController = GetComponent<ThirdPersonPlayerController>();

        if (GameManager.Instance.cameraController == null)
            GameManager.Instance.cameraController = this;
    }

    private void Update()
    {
        HandleMouseInput();
        RotateBody();
        RotateNeckAndHead();
        AdjustCameraAxis();
    }

    private void HandleMouseInput()
    {
        Vector2 mouseInput = playerController.InputActions.PlayerMovement.Camera.ReadValue<Vector2>();

        xRotation += -mouseInput.y * mouseSensitivityY * Time.deltaTime;
        yRotation += mouseInput.x * mouseSensitivityX * Time.deltaTime;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }

    private float NormalizeAngle(float angle)
    {
        angle = angle % 360;
        if (angle > 180) angle -= 360;
        if (angle < -180) angle += 360;
        return angle;
    }

    private void RotateBody()
    {
        currentBodyYRotation = NormalizeAngle(body.transform.eulerAngles.y);
        float targetBodyYRotation = NormalizeAngle(yRotation);

        if (playerController.IsMoving)
        {
            if (!freeMovement)
            {
                body.transform.rotation = Quaternion.Euler(0f, Mathf.LerpAngle(currentBodyYRotation, targetBodyYRotation, 10 * Time.deltaTime), 0f);
            }
            else
            {
                Vector3 moveDirection = new Vector3(playerController.InputActions.PlayerMovement.Movement.ReadValue<Vector2>().x, 0, playerController.InputActions.PlayerMovement.Movement.ReadValue<Vector2>().y);
                moveDirection = camAxisY.transform.TransformDirection(moveDirection);
                moveDirection.y = 0;
                moveDirection.Normalize();
                float targetBodyRotation = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
                body.transform.rotation = Quaternion.Euler(0f, Mathf.LerpAngle(currentBodyYRotation, targetBodyRotation, 10 * Time.deltaTime), 0f);
            }
        }
    }

    private void RotateNeckAndHead()
    {
        float neckRotationSpeed = playerController.IsMoving ? 60 : 30;

        float currentNeckYRotation = NormalizeAngle(neck.transform.eulerAngles.y);
        float targetNeckYRotation = NormalizeAngle(yRotation);

        float currentHeadXRotation = NormalizeAngle(head.transform.localEulerAngles.x);
        float targetHeadXRotation = NormalizeAngle(xRotation);

        if (forceBodyFollowLook)
        {
            neck.transform.rotation = Quaternion.Euler(0f, Mathf.LerpAngle(currentNeckYRotation, targetNeckYRotation, neckRotationSpeed * Time.deltaTime), 0f);
            head.transform.localRotation = Quaternion.Euler(Mathf.LerpAngle(currentHeadXRotation, targetHeadXRotation, 30 * Time.deltaTime), 0f, 0f);

            float fixedLocalYRotation = NormalizeAngle(neck.transform.localEulerAngles.y);

            if (restrictHeadRotation && (fixedLocalYRotation > headRotationLimit || fixedLocalYRotation < -headRotationLimit))
            {
                float diff = fixedLocalYRotation > headRotationLimit ? fixedLocalYRotation - headRotationLimit : fixedLocalYRotation + headRotationLimit;
                float newBodyRotation = currentBodyYRotation + diff;
                body.transform.rotation = Quaternion.Euler(0f, Mathf.LerpAngle(currentBodyYRotation, newBodyRotation, 30 * Time.deltaTime), 0f);
                neck.transform.localRotation = Quaternion.Euler(0f, fixedLocalYRotation - diff, 0f);
            }
        }
        else if (restrictHeadRotation)
        {
            // Calculate localTargetNeckYRotation
            float localTargetNeckYRotation = targetNeckYRotation - currentBodyYRotation;
            float localCurrentNeckYRotation = currentNeckYRotation - currentBodyYRotation;
            if (localTargetNeckYRotation > headRotationLimit || localTargetNeckYRotation < -headRotationLimit)
            {
                head.transform.localRotation = Quaternion.Euler(Mathf.LerpAngle(currentHeadXRotation, 0, 30 * Time.deltaTime), 0f, 0f);
                neck.transform.localRotation = Quaternion.Euler(0f, Mathf.LerpAngle(localCurrentNeckYRotation, 0, neckRotationSpeed * Time.deltaTime), 0f);
            }
            else
            {
                neck.transform.rotation = Quaternion.Euler(0f, Mathf.LerpAngle(currentNeckYRotation, targetNeckYRotation, neckRotationSpeed * Time.deltaTime), 0f);
                head.transform.localRotation = Quaternion.Euler(Mathf.LerpAngle(currentHeadXRotation, targetHeadXRotation, 30 * Time.deltaTime), 0f, 0f);
            }
        }
    }

    private void AdjustCameraAxis()
    {
        camAxisX.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        camAxisY.transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    public Vector2 RotateVector2TowardLook(Vector2 vector)
    {
        Vector3 vector3 = new Vector3(vector.x, 0, vector.y);
        Vector3 transformedVector = camAxisY.transform.TransformDirection(vector3);
        return new Vector2(transformedVector.x, transformedVector.z);
    }

    public Vector3 RotateVector3TowardLook(Vector3 vector)
    {
        return transform.TransformDirection(vector);
    }
}
