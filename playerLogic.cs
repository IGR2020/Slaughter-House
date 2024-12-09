using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class playerLogic : MonoBehaviour
{

	public PlayerInput playerInput;
	public CharacterController controller;
	public Camera playerCamera;
	public GameObject body;
	public float gravity = 4.5f;
	public float jumpHeight = 9f;
	public float jumpCoolDown = 0.1f;
	public float speed = 3f;
	public float sprintBonus = 2f;
	public float attackDistance = 3f;
	public int damage = 2;
	public int health = 200;
	public float knockback = 3f;
	public LayerMask attackableObjects;
	public float mouseSensitivity = 0.2f;
	public float buildDistance = 5f;
	public LayerMask validBuildAreas;
	public GameObject currentBuildObject;

	private bool isMoving = false;
	private bool isJumping = false;
	private bool isSprinting = false;
	private float jumpVel = 0;
	private float jumpCoolDownTimer = 0;
	private Vector2 movementDirection = Vector2.zero;
	private Vector3 velocity = Vector3.zero;

    void Start()
    {
		Cursor.lockState = CursorLockMode.Locked;
	}


	void Update()
    {
		velocity = Vector3.zero;
		if (isMoving)
		{
			velocity += (playerCamera.transform.right * movementDirection[0] * speed + playerCamera.transform.forward * movementDirection[1] * speed) * (isSprinting ? sprintBonus: 1);
		}
		if (!controller.isGrounded)
		{
			velocity += Vector3.down * gravity;
		}
		if (controller.isGrounded && isJumping && jumpCoolDownTimer > jumpCoolDown)
		{
			jumpVel = jumpHeight;
		}
		else if (jumpCoolDownTimer < jumpCoolDown)
		{
			jumpCoolDownTimer += Time.deltaTime;
		}

		jumpVel = Math.Max(jumpVel - gravity * Time.deltaTime, 0);
		velocity += Vector3.up * jumpVel;
		controller.Move(velocity * Time.deltaTime);
    }

	public void OnMove(InputAction.CallbackContext value)
	{
		isMoving = !value.canceled;
		movementDirection = value.ReadValue<Vector2>();
	}


	public void OnJump(InputAction.CallbackContext value)
	{
		isJumping = !value.canceled;
		jumpCoolDownTimer = 0;
	}

	public void OnLook(InputAction.CallbackContext value)
	{
		Vector2 lookDirection = value.ReadValue<Vector2>() * mouseSensitivity;
		float clampedXRotation = playerCamera.transform.localRotation.eulerAngles.x - lookDirection[1];
		if (clampedXRotation > 250)
		{
			clampedXRotation = Math.Clamp(clampedXRotation, 270, 360);
		}
		else if (clampedXRotation <  110)
		{
			clampedXRotation = Math.Clamp(clampedXRotation, -90, 90);
		}
		else
		{
			return;
		}
		playerCamera.transform.localRotation = Quaternion.Euler(clampedXRotation, playerCamera.transform.localRotation.eulerAngles.y + lookDirection[0], 0);
	}

	public void OnAttack(InputAction.CallbackContext value)
	{
		if (!value.started) { return; }
		if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out var ray, attackDistance, attackableObjects))
		{
			GameObject attackedObj = ray.collider.gameObject;
			enemyScript enemyLogic = attackedObj.GetComponentInParent<enemyScript>();
			enemyLogic.attack(damage, playerCamera.transform.forward, knockback);
		}
	}

	public void OnInteract(InputAction.CallbackContext value)
	{
		if (!value.started) { return; }
		if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out var ray, buildDistance, validBuildAreas))
		{
			GameObject selectedObject = ray.collider.gameObject;
			Instantiate(currentBuildObject, ray.point, Quaternion.Euler(0, playerCamera.transform.eulerAngles.y, 0));
		}
	}


	public void OnSprint(InputAction.CallbackContext value)
	{
		isSprinting = !value.canceled;
	}
}
