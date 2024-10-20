using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private HealthController healthController;
    public Camera playerCamera;

    [Header("Movement")]
    public float speed = 6f;
    public float airSpeed = 4f;
    public float sprintSpeedMultiplier = 1.4f;
    public float crouchSpeedMultiplier = 0.5f;

    public float gravity = 18f;
    public float friction = 3f;
    public float airFriction = 0.4f;
    public float jumpForce = 8f;

    public float mouseSensitivity = 0.1f;
    public float playerFOV = 90f;
    public float verticalRotation = 0f;

    public float standHeight = 2f;
    public float currentHeight = 2f;
    public float crouchHeight = 1f;
    public Vector2 moveValue, lookValue;
    public Vector3 characterVelocity;
    public float verticalVelocity;

    public bool isSprinting = false;
    public bool isGrounded = true;
    public bool isCrouched = false;
    public bool isJumping = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        healthController = GetComponent<HealthController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnMove(InputValue value)
    {
        moveValue = value.Get<Vector2>();
    }

    void OnLook(InputValue value)
    {
        lookValue = value.Get<Vector2>();
    }

    void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed;
    }

    void OnJump(InputValue value)
    {
        isJumping = value.isPressed;
    }

    void OnCrouch(InputValue value)
    {
        isCrouched = value.isPressed;
    }

    void Update()
    {
        handleLook();

        isGrounded = controller.isGrounded;
        if (isGrounded)
        {
            if (verticalVelocity < 0f)
            {
                verticalVelocity = -1f;
            } 
        } else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        Vector3 moveVector = new Vector3(moveValue.x, 0f, moveValue.y);
        Vector3 targetVelocity = transform.TransformVector(moveVector);

        if (isGrounded) Accelerate(targetVelocity, speed, 10f);
        else Accelerate(targetVelocity, speed, 3f);

        ApplyFriction();


        handleSprint();
        handleCrouch();
        handleJump();

        characterVelocity.y = verticalVelocity;
        controller.Move(characterVelocity * Time.deltaTime);
    }

    void Accelerate(Vector3 wishdir, float wishspeed, float accel)
    {
        float addSpeed, accelSpeed, currSpeed;
        float wishSpeed = wishspeed;

        if (isSprinting) wishSpeed *= sprintSpeedMultiplier;
        if (isCrouched)
        {
            wishSpeed *= crouchSpeedMultiplier;
        }

        currSpeed = Vector3.Dot(characterVelocity, wishdir);

        addSpeed = wishSpeed - currSpeed;
        if (addSpeed <= 0) return;

        accelSpeed = accel * Time.deltaTime * wishSpeed;
        if (accelSpeed > addSpeed) accelSpeed = addSpeed;

        characterVelocity.x += accelSpeed * wishdir.x;
        characterVelocity.z += accelSpeed * wishdir.z;
    }

    void ApplyFriction()
    {
        Vector3 currVelocity = new Vector3(characterVelocity.x, 0f, characterVelocity.z);

        float currSpeed = currVelocity.magnitude; // calculates speed in xz plane
        float drop = 0f;
        float controlSpeed;

        if (isGrounded)
        {
            currVelocity.y = characterVelocity.y;
            controlSpeed = currSpeed < 10f ? 10f : currSpeed;
            drop += controlSpeed * (isCrouched ? friction * 0.5f : friction) * Time.deltaTime;
        } else
        {
            controlSpeed = currSpeed < 10f ? 10f : currSpeed;
            drop += controlSpeed * airFriction * Time.deltaTime;
        }

        float newSpeed = Mathf.Max(currSpeed - drop, 0f); // calculates new speed after decreasing drop

        if (currSpeed > 0f) newSpeed /= currSpeed;

        characterVelocity *= newSpeed;
    }

    void handleLook()
    {
        lookValue *= mouseSensitivity;
        transform.Rotate(new Vector3(0.0f, (lookValue.x), 0.0f));

        verticalRotation -= lookValue.y;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    void handleSprint()
    {
        float targetFov = isSprinting ? playerFOV + 5f : playerFOV;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFov, 10f * Time.deltaTime);
    }

    void handleCrouch()
    {
        if (isCrouched)
        {
            currentHeight = Mathf.Lerp(currentHeight, crouchHeight, 10f * Time.deltaTime);
            controller.height = controller.GetComponent<CapsuleCollider>().height = currentHeight;
        } else
        {
            currentHeight = Mathf.Lerp(currentHeight, standHeight, 10f * Time.deltaTime);
            controller.height = controller.GetComponent<CapsuleCollider>().height = currentHeight;
        }
    }

    void handleJump()
    {
        if (isJumping && isGrounded)
        {
            verticalVelocity += jumpForce;
        }
    }
}
