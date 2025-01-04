using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private HealthController healthController;
    public SoundController soundController;
    public Camera playerCamera;

    [Header("Movement")]
    public float speed = 10f;
    public float airSpeed = 2f;
    public float acceleration = 20f;
    public float walkSpeedMultiplier = 0.5f;
    public float crouchSpeedMultiplier = 0.5f;

    [Header("Physics")]
    public float gravity = 18f;
    public float friction = 7f;
    public float airFriction = 0.4f;
    public float jumpForce = 8f;

    [Header("Camera")]
    public float mouseSensitivity = 3f;
    public float playerFOV = 90f;
    public float verticalRotation = 0f;

    public float standHeight = 2f;
    public float currentHeight = 2f;
    public float crouchHeight = 1f;
    private Vector2 moveValue, lookValue;
    public Vector3 characterVelocity;

    public bool isWalking = false;
    public bool isGrounded = true;
    public bool isCrouched = false;
    public bool wishJump = false;
    public bool inventoryOpen = false;
    public bool isDead = false;

    void Awake()
    {
        soundController = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundController>();
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        healthController = GetComponent<HealthController>();
        playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnMove(InputValue value)
    {
        moveValue = value.Get<Vector2>();
    }

    void OnLook(InputValue value)
    {
        lookValue = value.Get<Vector2>();
    }

    void OnWalk(InputValue value)
    {
        isWalking = value.isPressed;
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed) wishJump = true;
        else wishJump = false;
    }

    void OnCrouch(InputValue value)
    {
        if (value.isPressed)
        {
            controller.height = crouchHeight;
        }
        else
        {
            controller.height = standHeight;
        }

    }

    void Update()
    {
        if (!(inventoryOpen || isDead)) handleLook();

        if (!wishJump) ApplyFriction();

        if (isGrounded)
        {
            GroundMove();
        } else
        {
            AirMove();
        }

        // handleCrouch();

        controller.Move(characterVelocity * Time.deltaTime);
        isGrounded = controller.isGrounded;
    }

    void GroundMove()
    {
        Vector3 moveVector = new Vector3(moveValue.x, 0f, moveValue.y);
        Vector3 targetVelocity = transform.TransformVector(moveVector);
        float targetSpeed = targetVelocity.magnitude * speed;

        Accelerate(targetVelocity, targetSpeed, acceleration);

        characterVelocity.y = -1f;

        if (wishJump)
        {
            soundController.Play(soundController.jump);
            characterVelocity.y = jumpForce;
            wishJump = false;
        }
    }

    void AirMove()
    {
        Vector3 moveVector = new Vector3(moveValue.x, 0f, moveValue.y);
        Vector3 targetVelocity = transform.TransformVector(moveVector);
        float targetSpeed = targetVelocity.magnitude * speed;

        Accelerate(targetVelocity, targetSpeed, acceleration);

        characterVelocity.y -= gravity * Time.deltaTime;
    }

    void Accelerate(Vector3 wishdir, float wishspeed, float accel)
    {
        float addSpeed, accelSpeed, currSpeed;
        float wishSpeed = wishspeed;

        // Calculates the player's target speed
        if (isWalking) wishSpeed *= walkSpeedMultiplier;
        if (isCrouched) wishSpeed *= crouchSpeedMultiplier;

        if (!isGrounded && wishSpeed > airSpeed)
        {
            wishSpeed = airSpeed;
        }

        // Calculates the speed component in the xz plane
        currSpeed = Vector3.Dot(characterVelocity, wishdir); 

        addSpeed = wishSpeed - currSpeed;
        if (addSpeed <= 0) return;

        accelSpeed = accel * Time.deltaTime * wishSpeed;
        if (accelSpeed > addSpeed) accelSpeed = addSpeed;

        // Apply the acceleration to the player
        characterVelocity.x += accelSpeed * wishdir.x;
        characterVelocity.y += accelSpeed * wishdir.y;
        characterVelocity.z += accelSpeed * wishdir.z;
    }

    void ApplyFriction()
    {
        if (!isGrounded) return;

        Vector3 currVelocity = new Vector3(characterVelocity.x, 0f, characterVelocity.z);

        float currSpeed = currVelocity.magnitude;
        float drop = 0f;

        currVelocity.y = characterVelocity.y;
        float controlSpeed = currSpeed < 10f ? 10f : currSpeed;

        // Calculates the amount to decrease the player's speed by
        drop += controlSpeed * friction * Time.deltaTime; 

        float newSpeed = Mathf.Max(currSpeed - drop, 0f); // calculates new speed after decreasing drop

        if (currSpeed > 0f) newSpeed /= currSpeed;

        characterVelocity.x *= newSpeed;
        characterVelocity.z *= newSpeed;
    }

    void handleLook()
    {
        lookValue *= mouseSensitivity * 0.02f;
        transform.Rotate(new Vector3(0.0f, (lookValue.x), 0.0f));

        verticalRotation -= lookValue.y;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    /* Adjusts the player height based on whether they are crouched
     */
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

    public void SetDead() { isDead = true; }

    public void ApplyKnockback(Vector3 direction, float force, float duration)
    {
        StartCoroutine(KnockbackCoroutine(direction, force, duration));
    }

    private IEnumerator KnockbackCoroutine(Vector3 direction, float force, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            characterVelocity += direction * (force * Time.deltaTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        mouseSensitivity = sensitivity; // Update sensitivity
        Debug.Log($"Mouse Sensitivity set to: {mouseSensitivity}");
    }

    public void IncreaseSpeed(float amount)
    {
        speed += amount;
        Debug.Log($"Player speed increased by {amount}. New speed: {speed}");
    }

    public int getHealth()
    {
        return healthController.currentHealth;
    }

    public int getMaxHealth()
    {
        return healthController.maxHealth;
    }

    public void ActivateSpeedBoost(float boostAmount, float duration)
    {
        StartCoroutine(TemporarySpeedBoost(boostAmount, duration));
    }

    private IEnumerator TemporarySpeedBoost(float boostAmount, float duration)
    {
        float originalSpeed = speed;
        speed *= boostAmount;
        Debug.Log($"Speed boosted by {boostAmount}x. New speed: {speed}");

        yield return new WaitForSeconds(duration); // Wait for the boost duration

        speed = originalSpeed;
        Debug.Log($"Speed boost ended. Speed reverted to: {speed}");
    }


    public void ForceDirectionalLaunch(float upwardForce, float horizontalBoostMultiplier)
    {
        StartCoroutine(ForceDirectionalLaunchRoutine(upwardForce, horizontalBoostMultiplier));
    }

    private IEnumerator ForceDirectionalLaunchRoutine(float upwardForce, float horizontalBoostMultiplier)
    {
        isGrounded = false;
        Vector3 horizontalVelocity = new Vector3(characterVelocity.x, 0f, characterVelocity.z);
        horizontalVelocity *= horizontalBoostMultiplier;

        characterVelocity.x = horizontalVelocity.x;
        characterVelocity.z = horizontalVelocity.z;
        characterVelocity.y = upwardForce;

        yield return null;
    }

    public Vector2 GetMoveInputs()
    {
        return moveValue;
    }
}
