using System;
using System.Collections;
using UnityEngine;

namespace Lolopupka
{
	public class proceduralAnimation : MonoBehaviour
	{
		[Tooltip("Step distance is used to calculate step height. When character makes a short step there is no need to rase foot all the way up so if the current step distance is less then this step distance value step height will be lover then usual.")]
		[SerializeField]
		private float stepDistance = 1f;

		[SerializeField]
		private float stepHeight = 1f;

		[SerializeField]
		private float stepSpeed = 5f;

		[Tooltip("Velocity multiplier used to make step wider when moving on high speed (if you toggle the show gizmoz below and move your model around you could clearly see what this does. The blue spheres represent the target step points and will move further ahead if you increase velocity multiplier)")]
		[SerializeField]
		private float velocityMultiplier = 0.4f;

		[SerializeField]
		private float cycleSpeed = 1f;

		[Tooltip("how often in seconds legs will move (every one second by default)")]
		[SerializeField]
		private float cycleLimit = 1f;

		[Tooltip("•\tIf you want some legs to move together enable the Set Timings Manually. And add as many timings as your model has legs. The first Manual Timing is relative to the first leg in the leg IK targets array etc. For example: if your character has four legs and you want two left legs move first and two right to move second you need to set timings to [0.5, 0.5, 0, 0]. That means that first two legs will move and only 0.5 second later the second two will move. ")]
		[SerializeField]
		private bool SetTimingsManually;

		[SerializeField]
		private float[] manualTimings;

		[Tooltip("If you want only one leg to move at a time then set Timings offset as one divided by the number of legs. For example: if your character has four legs you need to set this as ¼ = 0.25. The script will offset the cycle of every leg by 0.25 seconds. ")]
		[SerializeField]
		private float timigsOffset = 0.25f;

		[Tooltip("Velocity clamp limits the step distance while moving on high speed.")]
		[SerializeField]
		private float velocityClamp = 4f;

		[SerializeField]
		private LayerMask layerMask;

		[SerializeField]
		private AnimationCurve legArcPathY = new AnimationCurve(new Keyframe(0f, 0f, 0f, 2.5f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f, -2.5f, 0f));

		[SerializeField]
		private AnimationCurve easingFunction = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		[SerializeField]
		private Transform[] legIktargets;

		[Header("Raycasts")]
		public bool showGizmoz = true;

		[SerializeField]
		private float legRayoffset = 3f;

		[SerializeField]
		private float legRayLength = 6f;

		[Tooltip("Ground check range for every leg")]
		[SerializeField]
		private float sphereCastRadius = 1f;

		[Header("Advansed")]
		[Tooltip("Refresh Timings rate updates timings and sets it to default value to make sure every leg is making step at the right time. If your character moves slowly, you can set it as some big value like 100 so it updates only every 100 seconds but if not, you need to lower this value. For example: fast pink robot in demo scene has this value set as 10. ")]
		[SerializeField]
		private float refreshTimingRate = 60f;

		public EventHandler<Vector3> OnStepFinished;

		private Vector3[] lastLegPositions;

		private Vector3[] defaultLegPositions;

		private Vector3[] raycastPoints;

		private Vector3[] targetStepPosition;

		private Vector3 velocity;

		private Vector3 lastVelocity;

		private Vector3 lastBodyPos;

		private float[] footTimings;

		private float[] targetTimings;

		private float[] totalDistance;

		private float clampDevider;

		private float[] arcHeitMultiply;

		private int nbLegs;

		private int indexTomove;

		private bool[] isLegMoving;

		private void Start()
		{
			indexTomove = -1;
			nbLegs = legIktargets.Length;
			defaultLegPositions = new Vector3[nbLegs];
			lastLegPositions = new Vector3[nbLegs];
			targetStepPosition = new Vector3[nbLegs];
			isLegMoving = new bool[nbLegs];
			footTimings = new float[nbLegs];
			arcHeitMultiply = new float[nbLegs];
			totalDistance = new float[nbLegs];
			if (SetTimingsManually && manualTimings.Length != nbLegs)
			{
				Debug.LogError("manual footTimings length should be equal to the leg count");
			}
			for (int i = 0; i < nbLegs; i++)
			{
				if (SetTimingsManually)
				{
					footTimings[i] = manualTimings[i];
				}
				else
				{
					footTimings[i] = (float)i * timigsOffset;
				}
				lastLegPositions[i] = legIktargets[i].position;
				defaultLegPositions[i] = legIktargets[i].localPosition;
			}
			StartCoroutine(UpdateTimings(refreshTimingRate));
		}

		private void Update()
		{
			velocity = (base.transform.position - lastBodyPos) / Time.deltaTime;
			velocity = Vector3.MoveTowards(lastVelocity, velocity, Time.deltaTime * 45f);
			clampDevider = 1f / Remap(velocity.magnitude, 0f, velocityClamp, 1f, 2f);
			lastVelocity = velocity;
			indexTomove = -1;
			for (int i = 0; i < nbLegs; i++)
			{
				if (i != indexTomove)
				{
					legIktargets[i].position = TargetPoint.FitToTheGround(lastLegPositions[i], layerMask, legRayoffset, legRayLength, sphereCastRadius);
				}
			}
			float num = Remap(velocity.magnitude, 0f, velocityClamp, 1f, 2f);
			for (int j = 0; j < nbLegs; j++)
			{
				footTimings[j] += Time.deltaTime * cycleSpeed * num;
				if (footTimings[j] >= cycleLimit)
				{
					footTimings[j] = 0f;
					indexTomove = j;
					SetUp(j);
				}
			}
			lastBodyPos = base.transform.position;
		}

		public void SetUp(int index)
		{
			Vector3 origin = base.transform.TransformPoint(defaultLegPositions[index]) + velocity.normalized * Mathf.Clamp(velocity.magnitude, 0f, velocityClamp * clampDevider) * velocityMultiplier;
			targetStepPosition[index] = TargetPoint.FitToTheGround(origin, layerMask, legRayoffset, legRayLength, sphereCastRadius);
			totalDistance[index] = GetDistanceToTarget(index);
			float num = Vector3.Distance(legIktargets[index].position, targetStepPosition[index]);
			arcHeitMultiply[index] = num / stepDistance;
			if (targetStepPosition[index] != Vector3.zero && TargetPoint.IsValidStepPoint(targetStepPosition[index], layerMask, legRayoffset, legRayLength, sphereCastRadius))
			{
				StartCoroutine(MakeStep(targetStepPosition[index], indexTomove));
			}
		}

		private IEnumerator MakeStep(Vector3 targetPosition, int index)
		{
			float current = 0f;
			while (current < 1f)
			{
				current += Time.deltaTime * stepSpeed;
				float num = legArcPathY.Evaluate(current) * stepHeight * Mathf.Clamp(arcHeitMultiply[index], 0f, 1f);
				Vector3 b = new Vector3(targetPosition.x, num + targetPosition.y, targetPosition.z);
				legIktargets[index].position = Vector3.Lerp(lastLegPositions[index], b, easingFunction.Evaluate(current));
				yield return null;
			}
			LegReachedTargetPosition(targetPosition, index);
		}

		private void LegReachedTargetPosition(Vector3 targetPosition, int index)
		{
			indexTomove = -1;
			legIktargets[index].position = targetPosition;
			lastLegPositions[index] = legIktargets[index].position;
			if (totalDistance[index] > 0.3f)
			{
				OnStepFinished?.Invoke(this, targetPosition);
			}
			isLegMoving[index] = false;
		}

		private IEnumerator UpdateTimings(float time)
		{
			yield return new WaitForSecondsRealtime(time);
			for (int i = 0; i < nbLegs; i++)
			{
				if (SetTimingsManually)
				{
					footTimings[i] = manualTimings[i];
				}
				else
				{
					footTimings[i] = (float)i * timigsOffset;
				}
			}
			StartCoroutine(UpdateTimings(refreshTimingRate));
		}

		public Transform[] GetLegArray()
		{
			return legIktargets;
		}

		private float GetDistanceToTarget(int index)
		{
			return Vector3.Distance(legIktargets[index].position, base.transform.TransformPoint(defaultLegPositions[index]));
		}

		public float GetDistanceToGround(int index)
		{
			Vector3 position = legIktargets[index].position;
			if (Physics.Raycast(new Ray(position + Vector3.up * 0.1f, -Vector3.up), out var hitInfo, (int)layerMask))
			{
				return Vector3.Distance(position, hitInfo.point);
			}
			return 0f;
		}

		public bool IsLegMoving(int index)
		{
			return isLegMoving[index];
		}

		public LayerMask GetLayerMask()
		{
			return layerMask;
		}

		public float GetAverageLegHeight()
		{
			float num = 0f;
			for (int i = 0; i < nbLegs; i++)
			{
				num += GetDistanceToGround(i);
			}
			return num / (float)nbLegs;
		}

		public static float Remap(float input, float oldLow, float oldHigh, float newLow, float newHigh)
		{
			float t = Mathf.InverseLerp(oldLow, oldHigh, input);
			return Mathf.Lerp(newLow, newHigh, t);
		}

		private void OnDrawGizmosSelected()
		{
			if (showGizmoz && Application.IsPlaying(this))
			{
				for (int i = 0; i < nbLegs; i++)
				{
					Vector3 center = TargetPoint.FitToTheGround(base.transform.TransformPoint(defaultLegPositions[i]) + velocity.normalized * Mathf.Clamp(velocity.magnitude, 0f, velocityClamp * clampDevider) * velocityMultiplier, layerMask, legRayoffset, legRayLength, sphereCastRadius);
					Gizmos.color = Color.blue;
					Gizmos.DrawSphere(center, 0.2f);
					Gizmos.color = Color.green;
					Gizmos.DrawRay(base.transform.TransformPoint(defaultLegPositions[i]) + Vector3.up * legRayoffset, -Vector3.up * legRayLength);
					Gizmos.DrawWireSphere(base.transform.TransformPoint(defaultLegPositions[i]), sphereCastRadius);
				}
			}
		}
	}
}
