using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class MonsterNavigation : MonoBehaviour
{
	public float DetectionRange = 5f;

	public float MonsterSpeedWander = 5f;

	public float MonsterSpeedChase = 7.5f;

	public Transform[] points;

	public Transform[] teleportPoints;

	public string tagString = "Player";

	public float stuckCheckInterval = 5f;

	public float teleportAfterSeconds = 30f;

	public float minimalMovementThreshold = 0.5f;

	public float switchTargetThreshold = 2f;

	private NavMeshAgent agent;

	private float stuckTimer;

	private Vector3 lastPosition;

	private float stuckCheckTimer;

	private bool isChasing;

	private GameObject currentTarget;

	private void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		agent.speed = MonsterSpeedWander;
		lastPosition = base.transform.position;
		Wander();
	}

	private void Update()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			agent.enabled = false;
			return;
		}
		agent.enabled = true;
		GameObject[] players = GameObject.FindGameObjectsWithTag(tagString);
		GameObject gameObject = FindClosestPlayer(players);
		if (gameObject != null)
		{
			float num = ((currentTarget != null) ? Vector3.Distance(base.transform.position, currentTarget.transform.position) : float.PositiveInfinity);
			float num2 = Vector3.Distance(base.transform.position, gameObject.transform.position);
			if (currentTarget == null || num2 < num - switchTargetThreshold)
			{
				currentTarget = gameObject;
			}
			agent.speed = MonsterSpeedChase;
			isChasing = true;
			agent.SetDestination(currentTarget.transform.position);
			if (agent.pathStatus == NavMeshPathStatus.PathPartial || agent.pathStatus == NavMeshPathStatus.PathInvalid)
			{
				Debug.Log("Player unreachable, resuming wander.");
				Wander();
			}
		}
		else
		{
			currentTarget = null;
			if (!agent.pathPending && agent.remainingDistance < 0.5f)
			{
				agent.speed = MonsterSpeedWander;
				isChasing = false;
				Wander();
			}
		}
		stuckCheckTimer += Time.deltaTime;
		if (!(stuckCheckTimer >= stuckCheckInterval))
		{
			return;
		}
		if (Vector3.Distance(base.transform.position, lastPosition) < minimalMovementThreshold)
		{
			stuckTimer += stuckCheckInterval;
			if (stuckTimer >= teleportAfterSeconds)
			{
				TeleportToRandomPoint();
				stuckTimer = 0f;
			}
		}
		else
		{
			stuckTimer = 0f;
		}
		lastPosition = base.transform.position;
		stuckCheckTimer = 0f;
	}

	private GameObject FindClosestPlayer(GameObject[] players)
	{
		GameObject result = null;
		float num = float.PositiveInfinity;
		foreach (GameObject gameObject in players)
		{
			float num2 = Vector3.Distance(base.transform.position, gameObject.transform.position);
			if (num2 <= DetectionRange && num2 < num)
			{
				num = num2;
				result = gameObject;
			}
		}
		return result;
	}

	private void Wander()
	{
		if (points.Length != 0)
		{
			int num = Random.Range(0, points.Length);
			agent.SetDestination(points[num].position);
			isChasing = false;
		}
	}

	private void TeleportToRandomPoint()
	{
		if (teleportPoints.Length != 0)
		{
			int num = Random.Range(0, teleportPoints.Length);
			base.transform.position = teleportPoints[num].position;
			agent.Warp(teleportPoints[num].position);
			Debug.Log("Monster was stuck. Teleporting to: " + teleportPoints[num].name);
			Wander();
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, DetectionRange);
	}
}
