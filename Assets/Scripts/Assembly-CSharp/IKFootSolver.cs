using System;
using UnityEngine;

public class IKFootSolver : MonoBehaviour
{
	public bool isMovingForward;

	[SerializeField]
	private LayerMask terrainLayer;

	[SerializeField]
	private Transform body;

	[SerializeField]
	private IKFootSolver otherFoot;

	[SerializeField]
	private float speed = 4f;

	[SerializeField]
	private float stepDistance = 0.2f;

	[SerializeField]
	private float stepLength = 0.2f;

	[SerializeField]
	private float sideStepLength = 0.1f;

	[SerializeField]
	private float stepHeight = 0.3f;

	[SerializeField]
	private Vector3 footOffset;

	public Vector3 footRotOffset;

	public float footYPosOffset = 0.1f;

	public float rayStartYOffset;

	public float rayLength = 1.5f;

	private float footSpacing;

	private Vector3 oldPosition;

	private Vector3 currentPosition;

	private Vector3 newPosition;

	private Vector3 oldNormal;

	private Vector3 currentNormal;

	private Vector3 newNormal;

	private float lerp;

	private void Start()
	{
		footSpacing = base.transform.localPosition.x;
		currentPosition = (newPosition = (oldPosition = base.transform.position));
		currentNormal = (newNormal = (oldNormal = base.transform.up));
		lerp = 1f;
	}

	private void Update()
	{
		base.transform.position = currentPosition + Vector3.up * footYPosOffset;
		base.transform.localRotation = Quaternion.Euler(footRotOffset);
		Ray ray = new Ray(body.position + body.right * footSpacing + Vector3.up * rayStartYOffset, Vector3.down);
		Debug.DrawRay(body.position + body.right * footSpacing + Vector3.up * rayStartYOffset, Vector3.down);
		if (Physics.Raycast(ray, out var hitInfo, rayLength, terrainLayer.value) && Vector3.Distance(newPosition, hitInfo.point) > stepDistance && !otherFoot.IsMoving() && lerp >= 1f)
		{
			lerp = 0f;
			Vector3 normalized = Vector3.ProjectOnPlane(hitInfo.point - currentPosition, Vector3.up).normalized;
			float num = Vector3.Angle(body.forward, body.InverseTransformDirection(normalized));
			isMovingForward = num < 50f || num > 130f;
			if (isMovingForward)
			{
				newPosition = hitInfo.point + normalized * stepLength + footOffset;
				newNormal = hitInfo.normal;
			}
			else
			{
				newPosition = hitInfo.point + normalized * sideStepLength + footOffset;
				newNormal = hitInfo.normal;
			}
		}
		if (lerp < 1f)
		{
			Vector3 vector = Vector3.Lerp(oldPosition, newPosition, lerp);
			vector.y += Mathf.Sin(lerp * MathF.PI) * stepHeight;
			currentPosition = vector;
			currentNormal = Vector3.Lerp(oldNormal, newNormal, lerp);
			lerp += Time.deltaTime * speed;
		}
		else
		{
			oldPosition = newPosition;
			oldNormal = newNormal;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(newPosition, 0.1f);
	}

	public bool IsMoving()
	{
		return lerp < 1f;
	}
}
