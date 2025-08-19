using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace ExitGames.Demos.DemoPunVoice
{
	public class CharacterInstantiation : MonoBehaviourPunCallbacks, IOnEventCallback
	{
		public enum SpawnSequence
		{
			Connection = 0,
			Random = 1,
			RoundRobin = 2
		}

		public delegate void OnCharacterInstantiated(GameObject character);

		public Transform SpawnPosition;

		public float PositionOffset = 2f;

		public GameObject[] PrefabsToInstantiate;

		public List<Transform> SpawnPoints;

		public bool AutoSpawn = true;

		public bool UseRandomOffset = true;

		public SpawnSequence Sequence;

		[SerializeField]
		private byte manualInstantiationEventCode = 1;

		protected int lastUsedSpawnPointIndex = -1;

		[SerializeField]
		private bool manualInstantiation;

		[SerializeField]
		private bool differentPrefabs;

		[SerializeField]
		private string localPrefabSuffix;

		[SerializeField]
		private string remotePrefabSuffix;

		public static event OnCharacterInstantiated CharacterInstantiated;

		public override void OnJoinedRoom()
		{
			if (!AutoSpawn || PrefabsToInstantiate == null)
			{
				return;
			}
			int num = PhotonNetwork.LocalPlayer.ActorNumber;
			if (num < 1)
			{
				num = 1;
			}
			int num2 = (num - 1) % PrefabsToInstantiate.Length;
			GetSpawnPoint(out var spawnPos, out var spawnRot);
			Camera.main.transform.position += spawnPos;
			if (manualInstantiation)
			{
				ManualInstantiation(num2, spawnPos, spawnRot);
				return;
			}
			GameObject gameObject = PrefabsToInstantiate[num2];
			gameObject = PhotonNetwork.Instantiate(gameObject.name, spawnPos, spawnRot, 0);
			if (CharacterInstantiation.CharacterInstantiated != null)
			{
				CharacterInstantiation.CharacterInstantiated(gameObject);
			}
		}

		private void ManualInstantiation(int index, Vector3 position, Quaternion rotation)
		{
			GameObject gameObject = PrefabsToInstantiate[index];
			GameObject gameObject2 = ((!differentPrefabs) ? Object.Instantiate(gameObject, position, rotation) : Object.Instantiate(Resources.Load($"{gameObject.name}{localPrefabSuffix}") as GameObject, position, rotation));
			PhotonView component = gameObject2.GetComponent<PhotonView>();
			if (PhotonNetwork.AllocateViewID(component))
			{
				object[] eventContent = new object[4]
				{
					index,
					gameObject2.transform.position,
					gameObject2.transform.rotation,
					component.ViewID
				};
				RaiseEventOptions raiseEventOptions = new RaiseEventOptions
				{
					Receivers = ReceiverGroup.Others,
					CachingOption = EventCaching.AddToRoomCache
				};
				PhotonNetwork.RaiseEvent(manualInstantiationEventCode, eventContent, raiseEventOptions, SendOptions.SendReliable);
				if (CharacterInstantiation.CharacterInstantiated != null)
				{
					CharacterInstantiation.CharacterInstantiated(gameObject2);
				}
			}
			else
			{
				Debug.LogError("Failed to allocate a ViewId.");
				Object.Destroy(gameObject2);
			}
		}

		public void OnEvent(EventData photonEvent)
		{
			if (photonEvent.Code == manualInstantiationEventCode)
			{
				object[] array = photonEvent.CustomData as object[];
				int num = (int)array[0];
				GameObject gameObject = PrefabsToInstantiate[num];
				Vector3 position = (Vector3)array[1];
				Quaternion rotation = (Quaternion)array[2];
				GameObject gameObject2 = ((!differentPrefabs) ? Object.Instantiate(gameObject, position, Quaternion.identity) : Object.Instantiate(Resources.Load($"{gameObject.name}{remotePrefabSuffix}") as GameObject, position, rotation));
				gameObject2.GetComponent<PhotonView>().ViewID = (int)array[3];
			}
		}

		protected virtual void GetSpawnPoint(out Vector3 spawnPos, out Quaternion spawnRot)
		{
			Transform spawnPoint = GetSpawnPoint();
			if (spawnPoint != null)
			{
				spawnPos = spawnPoint.position;
				spawnRot = spawnPoint.rotation;
			}
			else
			{
				spawnPos = new Vector3(0f, 0f, 0f);
				spawnRot = new Quaternion(0f, 0f, 0f, 1f);
			}
			if (UseRandomOffset)
			{
				Random.InitState((int)(Time.time * 10000f));
				Vector3 insideUnitSphere = Random.insideUnitSphere;
				insideUnitSphere.y = 0f;
				insideUnitSphere = insideUnitSphere.normalized;
				spawnPos += PositionOffset * insideUnitSphere;
			}
		}

		protected virtual Transform GetSpawnPoint()
		{
			if (SpawnPoints == null || SpawnPoints.Count == 0)
			{
				return null;
			}
			switch (Sequence)
			{
			case SpawnSequence.Connection:
			{
				int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
				return SpawnPoints[(actorNumber != -1) ? (actorNumber % SpawnPoints.Count) : 0];
			}
			case SpawnSequence.RoundRobin:
				lastUsedSpawnPointIndex++;
				if (lastUsedSpawnPointIndex >= SpawnPoints.Count)
				{
					lastUsedSpawnPointIndex = 0;
				}
				return SpawnPoints[lastUsedSpawnPointIndex];
			case SpawnSequence.Random:
				return SpawnPoints[Random.Range(0, SpawnPoints.Count)];
			default:
				return null;
			}
		}
	}
}
