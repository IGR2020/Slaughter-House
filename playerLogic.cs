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
	public float speed = 3f;
	public float sprintBonus = 2f;
	public float attackDistance = 7f;
	public int damage = 2;
	public float knockback = 3f;
	public LayerMask attackableObjects;

	private bool isMoving = false;
	private bool isJumping = false;
	private bool isSprinting = false;
	private float jumpVel = 0;
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
		if (controller.isGrounded && isJumping)
		{
			jumpVel += jumpHeight;
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
	}

	public void OnLook(InputAction.CallbackContext value)
	{
		Vector2 lookDirection = value.ReadValue<Vector2>();
		float clampedXRotation = playerCamera.transform.localRotation.eulerAngles.x - lookDirection[1];
		print(clampedXRotation);
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
		if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out var hit, attackDistance, attackableObjects))
		{
			GameObject attackedObj = hit.collider.gameObject;
			enemyScript enemyLogic = attackedObj.GetComponentInParent<enemyScript>();
			enemyLogic.attack(damage, playerCamera.transform.forward, knockback);
		}
	}


	public void OnSprint(InputAction.CallbackContext value)
	{
		isSprinting = !value.canceled;
	}
}
