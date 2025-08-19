using GorillaLocomotion;
using UnityEngine;
using UnityEngine.InputSystem;

public class WASDPlayer : MonoBehaviour
{
	public Player GorillaPlayer;

	public Transform cameraTransform;

	public float moveSpeed = 7f;

	public float rotationSpeed = 2f;

	private Rigidbody playerRigidbody;

	private Vector3 rotationVector = Vector3.zero;

	private void Start()
	{
		playerRigidbody = GorillaPlayer.GetComponent<Rigidbody>();
	}

	private void Update()
	{
		Vector3 zero = Vector3.zero;
		if (Keyboard.current.wKey.isPressed)
		{
			zero += cameraTransform.forward;
		}
		if (Keyboard.current.sKey.isPressed)
		{
			zero -= cameraTransform.forward;
		}
		if (Keyboard.current.aKey.isPressed)
		{
			zero -= cameraTransform.right;
		}
		if (Keyboard.current.dKey.isPressed)
		{
			zero += cameraTransform.right;
		}
		zero.y = 0f;
		zero.Normalize();
		if (Keyboard.current.leftShiftKey.isPressed)
		{
			moveSpeed = 30f;
		}
		else
		{
			moveSpeed = 7f;
		}
		if (Keyboard.current.spaceKey.isPressed)
		{
			zero -= cameraTransform.up;
			zero.y = 2f;
		}
		playerRigidbody.MovePosition(playerRigidbody.position + zero * moveSpeed * Time.deltaTime);
		if (Mouse.current.rightButton.isPressed)
		{
			float num = Mouse.current.delta.x.ReadValue() * rotationSpeed;
			float num2 = Mouse.current.delta.y.ReadValue() * rotationSpeed;
			rotationVector.x -= num2;
			rotationVector.y += num;
			rotationVector.x = Mathf.Clamp(rotationVector.x, -90f, 90f);
			cameraTransform.localRotation = Quaternion.Euler(rotationVector);
		}
	}
}
