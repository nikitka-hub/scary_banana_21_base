using UnityEngine;

namespace GorillaLocomotion
{
	public class Player : MonoBehaviour
	{
		private static Player _instance;

		public SphereCollider headCollider;

		public CapsuleCollider bodyCollider;

		public Transform leftHandFollower;

		public Transform rightHandFollower;

		public Transform rightHandTransform;

		public Transform leftHandTransform;

		private Vector3 lastLeftHandPosition;

		private Vector3 lastRightHandPosition;

		private Vector3 lastHeadPosition;

		private Rigidbody playerRigidBody;

		public int velocityHistorySize;

		public float maxArmLength = 1.5f;

		public float unStickDistance = 1f;

		public float velocityLimit;

		public float maxJumpSpeed;

		public float jumpMultiplier;

		public float minimumRaycastDistance = 0.05f;

		public float defaultSlideFactor = 0.03f;

		public float defaultPrecision = 0.995f;

		private Vector3[] velocityHistory;

		private int velocityIndex;

		private Vector3 currentVelocity;

		private Vector3 denormalizedVelocityAverage;

		private bool jumpHandIsLeft;

		private Vector3 lastPosition;

		public Vector3 rightHandOffset;

		public Vector3 leftHandOffset;

		public LayerMask locomotionEnabledLayers;

		public bool wasLeftHandTouching;

		public bool wasRightHandTouching;

		public bool disableMovement;

		public static Player Instance => _instance;

		private void Awake()
		{
			if (_instance != null && _instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				_instance = this;
			}
			InitializeValues();
		}

		public void InitializeValues()
		{
			playerRigidBody = GetComponent<Rigidbody>();
			velocityHistory = new Vector3[velocityHistorySize];
			lastLeftHandPosition = leftHandFollower.transform.position;
			lastRightHandPosition = rightHandFollower.transform.position;
			lastHeadPosition = headCollider.transform.position;
			velocityIndex = 0;
			lastPosition = base.transform.position;
		}

		private Vector3 CurrentLeftHandPosition()
		{
			if ((PositionWithOffset(leftHandTransform, leftHandOffset) - headCollider.transform.position).magnitude < maxArmLength)
			{
				return PositionWithOffset(leftHandTransform, leftHandOffset);
			}
			return headCollider.transform.position + (PositionWithOffset(leftHandTransform, leftHandOffset) - headCollider.transform.position).normalized * maxArmLength;
		}

		private Vector3 CurrentRightHandPosition()
		{
			if ((PositionWithOffset(rightHandTransform, rightHandOffset) - headCollider.transform.position).magnitude < maxArmLength)
			{
				return PositionWithOffset(rightHandTransform, rightHandOffset);
			}
			return headCollider.transform.position + (PositionWithOffset(rightHandTransform, rightHandOffset) - headCollider.transform.position).normalized * maxArmLength;
		}

		private Vector3 PositionWithOffset(Transform transformToModify, Vector3 offsetVector)
		{
			return transformToModify.position + transformToModify.rotation * offsetVector;
		}

		private void Update()
		{
			bool flag = false;
			bool flag2 = false;
			Vector3 zero = Vector3.zero;
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.zero;
			bodyCollider.transform.eulerAngles = new Vector3(0f, headCollider.transform.eulerAngles.y, 0f);
			Vector3 movementVector = CurrentLeftHandPosition() - lastLeftHandPosition + Vector3.down * 2f * 9.8f * Time.deltaTime * Time.deltaTime;
			if (IterativeCollisionSphereCast(lastLeftHandPosition, minimumRaycastDistance, movementVector, defaultPrecision, out var endPosition, singleHand: true))
			{
				vector = ((!wasLeftHandTouching) ? (endPosition - CurrentLeftHandPosition()) : (lastLeftHandPosition - CurrentLeftHandPosition()));
				playerRigidBody.velocity = Vector3.zero;
				flag = true;
			}
			movementVector = CurrentRightHandPosition() - lastRightHandPosition + Vector3.down * 2f * 9.8f * Time.deltaTime * Time.deltaTime;
			if (IterativeCollisionSphereCast(lastRightHandPosition, minimumRaycastDistance, movementVector, defaultPrecision, out endPosition, singleHand: true))
			{
				vector2 = ((!wasRightHandTouching) ? (endPosition - CurrentRightHandPosition()) : (lastRightHandPosition - CurrentRightHandPosition()));
				playerRigidBody.velocity = Vector3.zero;
				flag2 = true;
			}
			zero = (((!flag && !wasLeftHandTouching) || (!flag2 && !wasRightHandTouching)) ? (vector + vector2) : ((vector + vector2) / 2f));
			RaycastHit hitInfo;
			if (IterativeCollisionSphereCast(lastHeadPosition, headCollider.radius, headCollider.transform.position + zero - lastHeadPosition, defaultPrecision, out endPosition, singleHand: false))
			{
				zero = endPosition - lastHeadPosition;
				if (Physics.Raycast(lastHeadPosition, headCollider.transform.position - lastHeadPosition + zero, out hitInfo, (headCollider.transform.position - lastHeadPosition + zero).magnitude + headCollider.radius * defaultPrecision * 0.999f, locomotionEnabledLayers.value))
				{
					zero = lastHeadPosition - headCollider.transform.position;
				}
			}
			if (zero != Vector3.zero)
			{
				base.transform.position = base.transform.position + zero;
			}
			lastHeadPosition = headCollider.transform.position;
			movementVector = CurrentLeftHandPosition() - lastLeftHandPosition;
			if (IterativeCollisionSphereCast(lastLeftHandPosition, minimumRaycastDistance, movementVector, defaultPrecision, out endPosition, (!flag && !wasLeftHandTouching) || (!flag2 && !wasRightHandTouching)))
			{
				lastLeftHandPosition = endPosition;
				flag = true;
			}
			else
			{
				lastLeftHandPosition = CurrentLeftHandPosition();
			}
			movementVector = CurrentRightHandPosition() - lastRightHandPosition;
			if (IterativeCollisionSphereCast(lastRightHandPosition, minimumRaycastDistance, movementVector, defaultPrecision, out endPosition, (!flag && !wasLeftHandTouching) || (!flag2 && !wasRightHandTouching)))
			{
				lastRightHandPosition = endPosition;
				flag2 = true;
			}
			else
			{
				lastRightHandPosition = CurrentRightHandPosition();
			}
			StoreVelocities();
			if ((flag2 || flag) && !disableMovement && denormalizedVelocityAverage.magnitude > velocityLimit)
			{
				if (denormalizedVelocityAverage.magnitude * jumpMultiplier > maxJumpSpeed)
				{
					playerRigidBody.velocity = denormalizedVelocityAverage.normalized * maxJumpSpeed;
				}
				else
				{
					playerRigidBody.velocity = jumpMultiplier * denormalizedVelocityAverage;
				}
			}
			if (flag && (CurrentLeftHandPosition() - lastLeftHandPosition).magnitude > unStickDistance && !Physics.SphereCast(headCollider.transform.position, minimumRaycastDistance * defaultPrecision, CurrentLeftHandPosition() - headCollider.transform.position, out hitInfo, (CurrentLeftHandPosition() - headCollider.transform.position).magnitude - minimumRaycastDistance, locomotionEnabledLayers.value))
			{
				lastLeftHandPosition = CurrentLeftHandPosition();
				flag = false;
			}
			if (flag2 && (CurrentRightHandPosition() - lastRightHandPosition).magnitude > unStickDistance && !Physics.SphereCast(headCollider.transform.position, minimumRaycastDistance * defaultPrecision, CurrentRightHandPosition() - headCollider.transform.position, out hitInfo, (CurrentRightHandPosition() - headCollider.transform.position).magnitude - minimumRaycastDistance, locomotionEnabledLayers.value))
			{
				lastRightHandPosition = CurrentRightHandPosition();
				flag2 = false;
			}
			leftHandFollower.position = lastLeftHandPosition;
			rightHandFollower.position = lastRightHandPosition;
			wasLeftHandTouching = flag;
			wasRightHandTouching = flag2;
		}

		private bool IterativeCollisionSphereCast(Vector3 startPosition, float sphereRadius, Vector3 movementVector, float precision, out Vector3 endPosition, bool singleHand)
		{
			if (CollisionsSphereCast(startPosition, sphereRadius * precision, movementVector, precision, out endPosition, out var hitInfo))
			{
				Vector3 vector = endPosition;
				Surface component = hitInfo.collider.GetComponent<Surface>();
				float num = ((component != null) ? component.slipPercentage : ((!singleHand) ? defaultSlideFactor : 0.001f));
				Vector3 vector2 = Vector3.ProjectOnPlane(startPosition + movementVector - vector, hitInfo.normal) * num;
				if (CollisionsSphereCast(endPosition, sphereRadius, vector2, precision * precision, out endPosition, out hitInfo))
				{
					return true;
				}
				if (CollisionsSphereCast(vector2 + vector, sphereRadius, startPosition + movementVector - (vector2 + vector), precision * precision * precision, out endPosition, out hitInfo))
				{
					return true;
				}
				endPosition = vector;
				return true;
			}
			if (CollisionsSphereCast(startPosition, sphereRadius * precision * 0.66f, movementVector.normalized * (movementVector.magnitude + sphereRadius * precision * 0.34f), precision * 0.66f, out endPosition, out hitInfo))
			{
				endPosition = startPosition;
				return true;
			}
			endPosition = Vector3.zero;
			return false;
		}

		private bool CollisionsSphereCast(Vector3 startPosition, float sphereRadius, Vector3 movementVector, float precision, out Vector3 finalPosition, out RaycastHit hitInfo)
		{
			if (Physics.SphereCast(startPosition, sphereRadius * precision, movementVector, out hitInfo, movementVector.magnitude + sphereRadius * (1f - precision), locomotionEnabledLayers.value))
			{
				finalPosition = hitInfo.point + hitInfo.normal * sphereRadius;
				if (Physics.SphereCast(startPosition, sphereRadius * precision * precision, finalPosition - startPosition, out var hitInfo2, (finalPosition - startPosition).magnitude + sphereRadius * (1f - precision * precision), locomotionEnabledLayers.value))
				{
					finalPosition = startPosition + (finalPosition - startPosition).normalized * Mathf.Max(0f, hitInfo.distance - sphereRadius * (1f - precision * precision));
					hitInfo = hitInfo2;
				}
				else if (Physics.Raycast(startPosition, finalPosition - startPosition, out hitInfo2, (finalPosition - startPosition).magnitude + sphereRadius * precision * precision * 0.999f, locomotionEnabledLayers.value))
				{
					finalPosition = startPosition;
					hitInfo = hitInfo2;
					return true;
				}
				return true;
			}
			if (Physics.Raycast(startPosition, movementVector, out hitInfo, movementVector.magnitude + sphereRadius * precision * 0.999f, locomotionEnabledLayers.value))
			{
				finalPosition = startPosition;
				return true;
			}
			finalPosition = Vector3.zero;
			return false;
		}

		public bool IsHandTouching(bool forLeftHand)
		{
			if (forLeftHand)
			{
				return wasLeftHandTouching;
			}
			return wasRightHandTouching;
		}

		public void Turn(float degrees)
		{
			base.transform.RotateAround(headCollider.transform.position, base.transform.up, degrees);
			denormalizedVelocityAverage = Quaternion.Euler(0f, degrees, 0f) * denormalizedVelocityAverage;
			for (int i = 0; i < velocityHistory.Length; i++)
			{
				velocityHistory[i] = Quaternion.Euler(0f, degrees, 0f) * velocityHistory[i];
			}
		}

		private void StoreVelocities()
		{
			velocityIndex = (velocityIndex + 1) % velocityHistorySize;
			Vector3 vector = velocityHistory[velocityIndex];
			currentVelocity = (base.transform.position - lastPosition) / Time.deltaTime;
			denormalizedVelocityAverage += (currentVelocity - vector) / velocityHistorySize;
			velocityHistory[velocityIndex] = currentVelocity;
			lastPosition = base.transform.position;
		}
	}
}
